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
			//構文解析
			while (!stream.NextIsEndOfStream)
			{
				System.Windows.Forms.Application.DoEvents();
				System.Threading.Thread.Sleep(0);
				readingLine++;
				LogicalLine line = LogicalLineFactory.GetCodeToken(stream.GetLine());
				if (line != null)
					ret.Add(line);
			}

			//ゴーストの削除 foreach2とかstopの後のgoto文とか。
			//repeatなどの後に出るラベルは構文解析で削除
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
					ret[i].AddError("式：数式の変換に失敗");
			}


			//subAnalyzeScoop、subAnalyzeLabelの順でやると
			//if(value){
			//　～～
			//}
			//*label
			//else{
			//　～～
			//}
			//となった時にエラー
			//しかしながらsubAnalyzeLabel、subAnalyzeScoopの順でやると
			//if(value){
			//　～～
			//*label
			//}
			//return
			//となってみっともない。
			//そこでsubAnalyzeScoop、subAnalyzeLabelの順で行い、subAnalyzeLabelの際に直後がelse節で直前がScoopEndなら一つ戻す処理を入れる。

			subAnalyzeScoop(ret);
			subAnalyzeLabel(ret,data);
			

			//タブ整形や余分な行の削除を行う
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
			ret[ret.Count - 1].Visible = false;//末尾に自動生成されるstop文の削除
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
					line.AddError("deHSPの出力する#structはHSPの公開されている言語仕様には含まれません");
					ret.Add(line);
					//ret.Add(new EndOfModule());//#globalイラネ
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
		/// if,elseスコープの終点を決める。
		/// </summary>
		/// <param defaultName="ret"></param>
		private void subAnalyzeScoop(List<LogicalLine> ret)
		{			
			for (int i = 0; i < ret.Count; i++)
			{
				//ifelse以外は関係ない。
				IfStatement scoopStart = ret[i] as IfStatement;
				if (scoopStart == null)
					continue;
				if (scoopStart.JumpToOffset < 0)
				{
					scoopStart.ScoopEndIsDefined = false;
					scoopStart.AddError("条件分岐行:終点が正しく保存されていません");
					continue;
				}
				//まず着地する行(Jumpのとび先)を探す。
				//行の途中に着地してはならない。
				//TokenOffsetが0xFFFFを超える場合、HSP3.0aはオバーフローを起こすが、コンパイルは通る。
				//行の途中に着地するのはこのときぐらいか。
				//<<2007/4/17追記>>
				//if (～～){
				//として、}を書かなかった場合、飛び先が0にセットされる。このときも行の途中（自分自身の途中になる）に着地するようだ
				int jumpToOffset = scoopStart.JumpToOffset;
				int jumpToLineNo = -1;
				for (int j = (i + 1); j < ret.Count; j++)
				{
					if (ret[j].TokenOffset == jumpToOffset)
					{
						jumpToLineNo = j;
						break;
					}
					//行き過ぎちゃったらおしまい。
					if ((ret[j].TokenOffset != -1) && (ret[j].TokenOffset > jumpToOffset))
					{
						jumpToLineNo = -2;
						break;
					}
				}
				if (jumpToLineNo == -1)
				{
					scoopStart.ScoopEndIsDefined = false;
					scoopStart.AddError(string.Format("条件分岐行:{0:X08}:スコープの終わりがコード終端を超えています",jumpToOffset));
					continue;
				}
				if (jumpToLineNo == -2)
				{
					scoopStart.ScoopEndIsDefined = false;
					scoopStart.AddError(string.Format("条件分岐行:{0:X08}:スコープの終わりが行の途中です", jumpToOffset));
					continue;
				}
				//通常は着地の直前にScoopEndを挿入する。
				//ifからelseに飛ぶ場合、jump先はelseの直後の行になっているのでさらにひとつさかのぼる必要がある。
				IfStatement elseStatement = ret[jumpToLineNo - 1] as IfStatement;
				if (elseStatement != null)
					if ((scoopStart.isIfStatement) && (elseStatement.isElseStatement))
						jumpToLineNo--;
				ret.Insert(jumpToLineNo, new ScoopEnd());
				scoopStart.ScoopEndIsDefined = true;
			}
		}

		/// <summary>
		/// ラベル追加
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
				//　～～
				//}
				//*label
				//else{
				//　～～
				//}
				//となるのを回避する処理。
				//入れようとしている場所(i)がelse節の直前でScoopEndの直後ならScoopEndの前に移動。
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
