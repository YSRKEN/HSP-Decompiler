using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace KttK.HspDecompiler.DpmToAx
{
	internal sealed class Undpm : IDisposable
	{
		private Undpm()
		{
		}

		internal static Undpm FromBinaryReader(BinaryReader reader)
		{
			Undpm ret = new Undpm();
			try{
				ret.reader = reader;
				if (ret.readHeader())
					return ret;
			}
			catch (Exception)
			{
				return null;
			}
			return null;


		}

		long startPosition;
		long streamLength;
		long fileOffsetStart;
		private bool readHeader()
		{
			startPosition = reader.BaseStream.Position;
			streamLength = reader.BaseStream.Length - startPosition;
			char[] identifier = reader.ReadChars(4);
			if (identifier.Length < 4)
				return false;
			reader.BaseStream.Seek(startPosition, SeekOrigin.Begin);
			if ((identifier[0] == 'M') && (identifier[1] == 'Z'))
			{
				Win32ExeHeader winHeader = Win32ExeHeader.FromBinaryReader(reader);
				if (winHeader == null)
				{
					return false;
				}
				startPosition += winHeader.EndOfExecutableRegion;
				streamLength = reader.BaseStream.Length - startPosition;
				reader.BaseStream.Seek(startPosition, SeekOrigin.Begin);
				identifier = reader.ReadChars(4);
				if (identifier.Length < 4)
					return false;
			}
			if (! ((identifier[0] == 'D') && (identifier[1] == 'P') && (identifier[2] == 'M') && (identifier[3] == 'X')))
			{
				return false;
			}
			reader.BaseStream.Seek(startPosition, SeekOrigin.Begin);
			reader.ReadInt32();
			reader.ReadInt32();
			int fileCount = reader.ReadInt32();
			reader.ReadInt32();
			files.Capacity = fileCount;
			fileOffsetStart = startPosition + 0x10 + fileCount * 0x20;
			for (int i = 0; i < fileCount; i++)
			{
				DpmFileState file = new DpmFileState();
				Char[] chars = reader.ReadChars(16);
				int stringLength = 16;
				for (int j = 0; j < 16; j++)
				{
					if (chars[j] == '\0')
					{
						stringLength = j;
						break;
					}
				}
				file.FileName = new string(chars, 0, stringLength);
				file.unknown = reader.ReadInt32();
				file.Encryptionkey = reader.ReadInt32();
				file.FileOffset = reader.ReadInt32();
				file.FileSize = reader.ReadInt32();
				file.Parent = this;
				if ((file.FileOffset + file.FileSize) > (streamLength))
					return false;
				files.Add(file);
			}


			return true;

		}

		private BinaryReader reader;
		private List<DpmFileState> files = new List<DpmFileState>();
		internal List<DpmFileState> FileList
		{
			get
			{
				return files;
			}
		}


		internal byte[] GetFile(int fileOffset, int fileSize)
		{
			reader.BaseStream.Seek(fileOffset, SeekOrigin.Begin);
			byte[] buffer = new byte[fileSize];
			reader.BaseStream.Read(buffer, 0, fileSize);
			return buffer;
		}

		internal DpmFileState? GetStartAx()
		{
			foreach (DpmFileState file in files)
			{
				if (file.FileName == "start.ax")
					return file;
			}
			return null;
		}

		internal bool SeekStartAx()
		{
			foreach (DpmFileState file in files)
			{
				if (file.FileName.Equals("start.ax", StringComparison.Ordinal))
				{
					reader.BaseStream.Seek(file.FileOffset + this.fileOffsetStart, SeekOrigin.Begin);
					return true;
				}
			}
			return false;
			
		}

		internal bool Seek(DpmFileState file)
		{
			try
			{
				reader.BaseStream.Seek(file.FileOffset + this.fileOffsetStart, SeekOrigin.Begin);
			}
			catch
			{
				return false;
			}
			return true;
		}

		#region IDisposable ÉÅÉìÉo

		public void Dispose()
		{

			throw new Exception("The method or operation is not implemented.");
		}

		#endregion
	}
}
