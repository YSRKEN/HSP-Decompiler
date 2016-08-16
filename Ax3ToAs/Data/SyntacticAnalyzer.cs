using System;
using System.Collections.Generic;
using System.Text;
using KttK.HspDecompiler.Ax3ToAs.Data.Analyzer;
using KttK.HspDecompiler.Ax3ToAs.Data.Line;
namespace KttK.HspDecompiler.Ax3ToAs.Data
{
	/// <summary>
	/// TokenFactory
	/// </summary>
	class SyntacticAnalyzer
	{
		int readingLine = 0;
		internal List<LogicalLine> Analyze(TokenCollection stream, AxData data)
		{
			List<LogicalLine> ret = new List<LogicalLine>();
			subAnalyzePreprocessor(ret, data);
			readingLine = ret.Count;
			//�\�����
			while (!stream.NextIsEndOfStream)
			{
				System.Windows.Forms.Application.DoEvents();
				System.Threading.Thread.Sleep(0);
				readingLine++;
				LogicalLine line = LogicalLineFactory.GetCodeToken(stream.GetLine());
				if (line != null)
					ret.Add(line);
			}

			//�S�[�X�g�̍폜 foreach2�Ƃ�stop�̌��goto���Ƃ��B
			//repeat�Ȃǂ̌�ɏo�郉�x���͍\����͂ō폜
			for (int i = 0; i < ret.Count; i++)
			{
				if (ret[i].HasFlagIsGhost)
					ret[i].Visible = false;
				if ( (ret[i].HasFlagGhostGoto) && (i != (ret.Count -1) ) )
					ret[i + 1].Visible = false;
			}
			ret = ret.FindAll(IsVisible);
			for (int i = 0; i < ret.Count; i++)
			{
				if(!ret[i].CheckRpn())
					ret[i].AddError("���F�����̕ϊ��Ɏ��s");
			}


			//subAnalyzeScoop�AsubAnalyzeLabel�̏��ł���
			//if(value){
			//�@�`�`
			//}
			//*label
			//else{
			//�@�`�`
			//}
			//�ƂȂ������ɃG���[
			//�������Ȃ���subAnalyzeLabel�AsubAnalyzeScoop�̏��ł���
			//if(value){
			//�@�`�`
			//*label
			//}
			//return
			//�ƂȂ��Ă݂��Ƃ��Ȃ��B
			//������subAnalyzeScoop�AsubAnalyzeLabel�̏��ōs���AsubAnalyzeLabel�̍ۂɒ��オelse�߂Œ��O��ScoopEnd�Ȃ��߂�����������B

			subAnalyzeScoop(ret);
			subAnalyzeLabel(ret,data);
			

			//�^�u���`��]���ȍs�̍폜���s��
			int tabCount = 1;
			for (int i = 0; i < ret.Count; i++)
			{
				if (ret[i].TabDecrement)
					tabCount--;
				ret[i].TabCount = tabCount;
				if (ret[i].TabIncrement)
					tabCount++;
			}


			for (int i = 0; i < ret.Count; i++)
			{
				if (ret[i].GetErrorMes().Count != 0)
					foreach (string errMes in ret[i].GetErrorMes())
						global::KttK.HspDecompiler.HspConsole.Warning(errMes, i + 1);

			}
			ret[ret.Count - 1].Visible = false;//�����Ɏ������������stop���̍폜
			ret = ret.FindAll(IsVisible);
			return ret;


		}

		private void subAnalyzePreprocessor(List<LogicalLine> ret, AxData data)
		{
			if (data.Runtime != null)
			{
				ret.Add(new PreprocessorDeclaration(data.Runtime));
				ret.Add(new CommentLine());
			}
			if (data.Modules.Count != 0)
			{
				foreach (Function module in data.Modules)
				{
					LogicalLine line = new PreprocessorDeclaration(module);
					line.AddError("deHSP�̏o�͂���#struct��HSP�̌��J����Ă��錾��d�l�ɂ͊܂܂�܂���");
					ret.Add(line);
					//ret.Add(new EndOfModule());//#global�C���l
				}

			}


			foreach (Usedll dll in data.Usedlls)
			{
				ret.Add(new PreprocessorDeclaration(dll));
				List<Function> funcs = dll.GetFunctions();
				if(funcs != null)
					foreach(Function func in funcs)
						ret.Add(new PreprocessorDeclaration(func));
				ret.Add(new CommentLine());
			}

			foreach (PlugIn plugin in data.PlugIns)
			{
				ret.Add(new PreprocessorDeclaration(plugin));
				Dictionary<int, Cmd> cmds = plugin.GetCmds();
				foreach (Cmd cmd in cmds.Values)
				{
					ret.Add(new PreprocessorDeclaration(cmd));
				}
				ret.Add(new CommentLine());
			}
		}


		/// <summary>
		/// if,else�X�R�[�v�̏I�_�����߂�B
		/// </summary>
		/// <param defaultName="ret"></param>
		private void subAnalyzeScoop(List<LogicalLine> ret)
		{			
			for (int i = 0; i < ret.Count; i++)
			{
				//ifelse�ȊO�͊֌W�Ȃ��B
				IfStatement scoopStart = ret[i] as IfStatement;
				if (scoopStart == null)
					continue;
				if (scoopStart.JumpToOffset < 0)
				{
					scoopStart.ScoopEndIsDefined = false;
					scoopStart.AddError("��������s:�I�_���������ۑ�����Ă��܂���");
					continue;
				}
				//�܂����n����s(Jump�̂Ƃѐ�)��T���B
				//�s�̓r���ɒ��n���Ă͂Ȃ�Ȃ��B
				//TokenOffset��0xFFFF�𒴂���ꍇ�AHSP3.0a�̓I�o�[�t���[���N�������A�R���p�C���͒ʂ�B
				//�s�̓r���ɒ��n����̂͂��̂Ƃ����炢���B
				//<<2007/4/17�ǋL>>
				//if (�`�`){
				//�Ƃ��āA}�������Ȃ������ꍇ�A��ѐ悪0�ɃZ�b�g�����B���̂Ƃ����s�̓r���i�������g�̓r���ɂȂ�j�ɒ��n����悤��
				int jumpToOffset = scoopStart.JumpToOffset;
				int jumpToLineNo = -1;
				for (int j = (i + 1); j < ret.Count; j++)
				{
					if (ret[j].TokenOffset == jumpToOffset)
					{
						jumpToLineNo = j;
						break;
					}
					//�s���߂���������炨���܂��B
					if ((ret[j].TokenOffset != -1) && (ret[j].TokenOffset > jumpToOffset))
					{
						jumpToLineNo = -2;
						break;
					}
				}
				if (jumpToLineNo == -1)
				{
					scoopStart.ScoopEndIsDefined = false;
					scoopStart.AddError(string.Format("��������s:{0:X08}:�X�R�[�v�̏I��肪�R�[�h�I�[�𒴂��Ă��܂�",jumpToOffset));
					continue;
				}
				if (jumpToLineNo == -2)
				{
					scoopStart.ScoopEndIsDefined = false;
					scoopStart.AddError(string.Format("��������s:{0:X08}:�X�R�[�v�̏I��肪�s�̓r���ł�", jumpToOffset));
					continue;
				}
				//�ʏ�͒��n�̒��O��ScoopEnd��}������B
				//if����else�ɔ�ԏꍇ�Ajump���else�̒���̍s�ɂȂ��Ă���̂ł���ɂЂƂ����̂ڂ�K�v������B
				IfStatement elseStatement = ret[jumpToLineNo - 1] as IfStatement;
				if (elseStatement != null)
					if ((scoopStart.isIfStatement) && (elseStatement.isElseStatement))
						jumpToLineNo--;
				ret.Insert(jumpToLineNo, new ScoopEnd());
				scoopStart.ScoopEndIsDefined = true;
			}
		}

		/// <summary>
		/// ���x���ǉ�
		/// </summary>
		/// <param defaultName="ret"></param>
		/// <param defaultName="data"></param>
		private void subAnalyzeLabel(List<LogicalLine> ret, AxData data)
		{
			foreach (LogicalLine line in ret)
				line.CheckLabel();
			data.DeleteInvisibleLables();
			data.RenameLables();
			int i = 0;
			foreach (Label label in data.Labels)
			{
				if (label.TokenOffset == -1)
					continue;
				while ((i < ret.Count) && ((ret[i].TokenOffset == -1) || (label.TokenOffset > ret[i].TokenOffset)))
				{
					i++;
				}

				//if(value){
				//�@�`�`
				//}
				//*label
				//else{
				//�@�`�`
				//}
				//�ƂȂ�̂�������鏈���B
				//����悤�Ƃ��Ă���ꏊ(i)��else�߂̒��O��ScoopEnd�̒���Ȃ�ScoopEnd�̑O�Ɉړ��B
				if ((i> 0) &&(ret[i] is IfStatement))
				{
					IfStatement ifStatement = ret[i] as IfStatement;
					if ((ret[i - 1] is ScoopEnd) && (ifStatement.isElseStatement))
					{
						i--;
					}

				}
				ret.Insert(i,new PreprocessorDeclaration(label));
				continue;
			}
		}

		private bool IsVisible(LogicalLine line)
		{
			return line.Visible;
		}



	}
}
