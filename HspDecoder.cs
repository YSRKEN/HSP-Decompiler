using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Windows.Forms;
using System.Threading;
using KttK.HspDecompiler.DpmToAx;
using KttK.HspDecompiler.Ax2ToAs;
using KttK.HspDecompiler.Ax3ToAs;
namespace KttK.HspDecompiler
{
	internal sealed class HspDecoder
	{
		private const string dictionaryFileName = "Dictionary.csv";
		private static Hsp3Dictionary dictionary = null;
		internal static bool Initialize()
		{
			string dictionaryPath = Program.ExeDir + dictionaryFileName;
			try
			{
				dictionary = Hsp3Dictionary.FromFile(dictionaryPath);
			}
			catch
			{
				goto err;
			}
			if (dictionary != null)
			{
				HspConsole.WriteLog("load " + dictionaryFileName + ":succeeded");
				return true;
			}
		err:
			HspConsole.WriteLog("load " + dictionaryFileName + ":failed");
			System.Windows.Forms.MessageBox.Show(dictionaryFileName + "�̓Ǎ��Ɏ��s���܂���");
			dictionary = null;
			return false;
		}

		internal void DecompressDpm(BinaryReader reader, ListView dpmFileListView, string outputDir)
		{
			if (reader == null)
				throw new ArgumentNullException();
			if (dpmFileListView == null)
				throw new ArgumentNullException();
			dpmFileListView.Items.Clear();
			global::KttK.HspDecompiler.HspConsole.Write("DPM�w�b�_�[�̊J�n�ʒu��������...");
			Undpm undpm = Undpm.FromBinaryReader(reader);
			if (undpm == null)
				throw new HspDecoderException("DPM�w�b�_�[��������܂���(HSP�̎��s�t�@�C���ł͂���܂���)");
			if ((undpm.FileList == null) || (undpm.FileList.Count == 0))
				throw new HspDecoderException("DPM�Ƀt�@�C�����܂܂�Ă��܂���");
			
			int encryptCount = 0;
			int fileCount = undpm.FileList.Count;
			foreach (DpmFileState file in undpm.FileList)
			{
				string[] itemParams = new string[4];
				itemParams[0] = file.FileName;
				if (file.IsEncrypted)
				{
					itemParams[1] = "�L";
					encryptCount++;
				}
				else
					itemParams[1] = "�|";
				itemParams[2] = string.Format("0x{0:X08}",file.FileOffset);
				itemParams[3] = string.Format("0x{0:X08}",file.FileSize);
				dpmFileListView.Items.Add(new ListViewItem(itemParams));
			}
			Thread.Sleep(0);
#if AllowDecryption
#else
			if ((fileCount - encryptCount) <= 0)
			{
				MessageBox.Show("���ׂẴt�@�C�����Í�������Ă��܂�", fileCount.ToString() + "�t�@�C�����A" + encryptCount.ToString() + "�t�@�C�����Í�������Ă��܂��B", MessageBoxButtons.OK);
				global::KttK.HspDecompiler.HspConsole.Write("�W�J���f");
				return;
			}
#endif
			if (encryptCount > 0)
			{
				DialogResult result = MessageBox.Show("�Í������ꂽ�t�@�C��������܂��B" + Environment.NewLine + "�Í������ꂽ�t�@�C���𖳎����ēW�J�𑱂��܂����H", fileCount.ToString() + "�t�@�C�����A" + encryptCount.ToString() + "�t�@�C�����Í�������Ă��܂��B", MessageBoxButtons.YesNo);
				if (result != DialogResult.Yes)
				{
					global::KttK.HspDecompiler.HspConsole.Write("�W�J���f");
					return;
				}
			}
			if (!Directory.Exists(outputDir))
			{
				try
				{
					Directory.CreateDirectory(outputDir);
				}
				catch
				{
					throw new HspDecoderException("�f�B���N�g��" + outputDir + "�̍쐬�Ɏ��s���܂���");

				}
			}
			byte[] buffer = null;
			FileStream saveStream = null;
			foreach (DpmFileState file in undpm.FileList)
			{
				if (file.IsEncrypted)
				{
#if AllowDecryption
					if (file.FileName != "start.ax")
#endif
					{
						global::KttK.HspDecompiler.HspConsole.Write(file.FileName + "�͈Í�������Ă��܂�");
						continue;
					}
				}
				string outputPath = outputDir + file.FileName;
				if (File.Exists(outputPath))
				{
					global::KttK.HspDecompiler.HspConsole.Write(file.FileName + "�Ɠ����̃t�@�C�������ɑ��݂��܂�");
					continue;
				}
				if (!undpm.Seek(file))
				{
					global::KttK.HspDecompiler.HspConsole.Write(file.FileName + "�̊J�n�ʒu�Ɉړ��ł��܂���ł���");
					continue;
				}
				buffer = reader.ReadBytes(file.FileSize);
#if AllowDecryption
				if (file.IsEncrypted)
				{
					global::KttK.HspDecompiler.HspConsole.Write(file.FileName + "�̕�����...");

					KttK.HspDecompiler.DpmToAx.HspCrypto.HspCryptoTransform decrypter = KttK.HspDecompiler.DpmToAx.HspCrypto.HspCryptoTransform.CrackEncryption(buffer);
					if (decrypter == null){
						global::KttK.HspDecompiler.HspConsole.Write(file.FileName + "�̕����Ɏ��s���܂���");
						
						continue;
					}
					buffer = decrypter.Decryption(buffer);
				}
#endif
				try{
					saveStream = new FileStream(outputPath, FileMode.CreateNew, FileAccess.Write);
					saveStream.Write(buffer,0,buffer.Length);
				}
				catch
				{
					global::KttK.HspDecompiler.HspConsole.Warning(file.FileName + "�̕ۑ��Ɏ��s���܂���");
				}
				finally{
					if(saveStream != null)
						saveStream.Close();
				}
					
			}
			global::KttK.HspDecompiler.HspConsole.Write("�W�J�I��");
		}

		internal void Decode(BinaryReader reader,string outputPath)
		{
			if (reader == null)
				throw new ArgumentNullException();
			if (dictionary == null)
				throw new InvalidOperationException();
			global::KttK.HspDecompiler.HspConsole.StartParagraph();
			List<string> lines = null;
			global::KttK.HspDecompiler.HspConsole.Write("�t�R���p�C����...");
			global::KttK.HspDecompiler.HspConsole.StartParagraph();
			lines = getDecoder(reader).Decode(reader);

			global::KttK.HspDecompiler.HspConsole.EndParagraph();
			global::KttK.HspDecompiler.HspConsole.Write("�t�R���p�C���I��");
			global::KttK.HspDecompiler.HspConsole.EndParagraph();
			global::KttK.HspDecompiler.HspConsole.Write(Path.GetFileName(outputPath) + "�ɏo��");

			StreamWriter writer = null;
			try
			{
				writer = new StreamWriter(outputPath, false, Encoding.GetEncoding("SHIFT-JIS"));
				foreach (string line in lines)
					writer.WriteLine(line);
				global::KttK.HspDecompiler.HspConsole.Write("��͏I��");
			}
			finally
			{
				if (writer != null)
					writer.Close();
			}
		}

		AbstractAxDecoder getDecoder(BinaryReader reader)
		{
			long startPosition = reader.BaseStream.Position;
			char[] buffer = reader.ReadChars(4);
			string bufStr = new string(buffer);
			reader.BaseStream.Seek(startPosition, SeekOrigin.Begin);
			if (bufStr.Equals("HSP2", StringComparison.Ordinal))
			{
				return new Ax2Decoder();
			}
			else if (bufStr.Equals("HSP3", StringComparison.Ordinal))
			{
				Ax3Decoder decoder = new Ax3Decoder();
				decoder.Dictionary = dictionary;
				return decoder;
			}
			throw new HspDecoderException("HSP2�ł�HSP3�ł��Ȃ��`��");
			


		}
	}

}
