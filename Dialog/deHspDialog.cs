using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using KttK.HspDecompiler.DpmToAx;
using System.IO;
namespace KttK.HspDecompiler
{
	internal sealed partial class deHspDialog : Form
	{
		internal deHspDialog()
		{
			InitializeComponent();
			global::KttK.HspDecompiler.HspConsole.Flush += new HspConsole.WriteDown(HspConsole_Flush);
		}

		internal deHspDialog(string arg)
		{
			InitializeComponent();
			global::KttK.HspDecompiler.HspConsole.Flush += new HspConsole.WriteDown(HspConsole_Flush);
			nextFilePath = arg;
			this.MaximumSize = this.Size;
			this.MinimumSize = this.Size;
		}
		string nextFilePath;
		#region drag & drop
		private void Form1_DragEnter(object sender, DragEventArgs e)
		{
			//�R���g���[�����Ƀh���b�O���ꂽ�Ƃ����s�����
			if (e.Data.GetDataPresent(DataFormats.FileDrop))
				//�h���b�O���ꂽ�f�[�^�`���𒲂ׁA�t�@�C���̂Ƃ��̓R�s�[�Ƃ���
				e.Effect = DragDropEffects.Copy;
			else
				//�t�@�C���ȊO�͎󂯕t���Ȃ�
				e.Effect = DragDropEffects.None;
			

		}

		private void Form1_DragDrop(object sender, DragEventArgs e)
		{
			//�R���g���[�����Ƀh���b�v���ꂽ�Ƃ����s�����
			//�h���b�v���ꂽ���ׂẴt�@�C�������擾����
			string[] fileName =
				(string[])e.Data.GetData(DataFormats.FileDrop, false);
			if((fileName == null) ||(fileName.Length == 0))
				return;
			if	(string.IsNullOrEmpty(fileName[0]))
				return;
			Do(fileName[0]);

		}

		#endregion
		private void Do(string filePath)
		{
			txtBoxMainInfo.Text = "";
			global::KttK.HspDecompiler.HspConsole.DecompStart(filePath);
			HspDecoder decoder = new HspDecoder();
			string dirName = Path.GetDirectoryName(filePath) + @"\";
			string inputFileName = Path.GetFileNameWithoutExtension(filePath);
			int i = 1;
			string errorPath = filePath + ".log";
			FileStream stream = null;
			BinaryReader reader = null;
			StreamWriter errorlog = null;
			try
			{
				global::KttK.HspDecompiler.HspConsole.Write(Path.GetFileName(filePath) + "��ǂݍ���");
				stream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
				reader = new BinaryReader(stream, Encoding.GetEncoding("SHIFT-JIS"));

				char[] buffer = reader.ReadChars(4);
				string bufStr = new string(buffer);
				reader.BaseStream.Seek(0, SeekOrigin.Begin);
				if (bufStr.StartsWith("MZ", StringComparison.Ordinal) || bufStr.StartsWith("DPM", StringComparison.Ordinal))
				{
					dirName = Path.GetDirectoryName(filePath) + @"\" + inputFileName;
					i = 1;
					while (Directory.Exists(dirName))
					{
						dirName = string.Format(@"{0}\{1} ({2})", Path.GetDirectoryName(filePath), inputFileName, i);
						i++;
					}
					errorPath = dirName + ".log";
					dirName = dirName + @"\";
					decoder.DecompressDpm(reader, this.dpmFileList, dirName);
				}
				else if (bufStr.StartsWith("HSP2", StringComparison.Ordinal)||bufStr.StartsWith("HSP3", StringComparison.Ordinal))
				{
					string outputFileExtention = null;
					if (bufStr.StartsWith("HSP2", StringComparison.Ordinal))
						outputFileExtention = ".as";
					else
						outputFileExtention = ".hsp";

					string outputFileName = inputFileName;
					string outputPath = dirName + outputFileName + outputFileExtention;
					while (File.Exists(outputPath))
					{
						outputFileName = string.Format("{0} ({1})", inputFileName, i);
						outputPath = dirName + outputFileName + outputFileExtention;
						i++;
					}
					decoder.Decode(reader, outputPath);
					errorPath = outputPath + ".log";

				}
				else
					throw new HspDecoderException("�����ł��Ȃ��t�@�C���`���ł�");
				int warCount = global::KttK.HspDecompiler.HspConsole.Warnings.Count;
				if (warCount != 0)
				{
					MessageBox.Show(Path.GetFileName(errorPath) + "�ɃG���[���o�͂��܂�", "�R�[�h�����S�ɂ͕����ł��܂���ł���");
					errorlog = new StreamWriter(errorPath, false, Encoding.GetEncoding("SHIFT-JIS"));
					foreach (string line in global::KttK.HspDecompiler.HspConsole.Warnings)
						errorlog.WriteLine(line);
				}
				
			}
			catch(Exception e)
			{
				global::KttK.HspDecompiler.HspConsole.FatalError(e);
				return;
			}
			finally
			{
				if (reader != null)
					reader.Close();
				else if (stream != null)
					stream.Close();
				if (errorlog != null)
					errorlog.Close();
			}

			
		}

		void HspConsole_Flush()
		{
			string line = global::KttK.HspDecompiler.HspConsole.NewLine;
			if (line != null)
				txtBoxMainInfo.Text += line + System.Environment.NewLine;
			this.Refresh();
		}

		private void ToolStripMenuItemOpen_Click(object sender, EventArgs e)
		{
			if (openFileDialog.ShowDialog() == DialogResult.OK)
			{
				Do(openFileDialog.FileName);
			}
		}

		private void ToolStripMenuItemExit_Click(object sender, EventArgs e)
		{
			this.Close();
		}

		private void ToolStripMenuItemAbout_Click(object sender, EventArgs e)
		{
			AboutDialog about = new AboutDialog();
			about.SetInitialLocation(this);
			about.ShowDialog();
		}

		private void deHspDialog_Load(object sender, EventArgs e)
		{
		}

		private void deHspDialog_Activated(object sender, EventArgs e)
		{
			if (nextFilePath == null)
				return;
			string filepath = nextFilePath;
			nextFilePath = null;
			Do(filepath);
		}

	}
}