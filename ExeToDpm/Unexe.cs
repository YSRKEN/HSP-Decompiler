using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
namespace KttK.HspDecompiler.ExeToDpm
{
	class Unexe
	{
		internal void GetDpmFile(string exeFilePath, string dpmFilePath)
		{
			try
			{
				FileStream stream = new FileStream(exeFilePath, FileMode.Open, FileAccess.Read);
				using (stream)
				{
					FileStream dpmStream = new FileStream(dpmFilePath, FileMode.Create, FileAccess.Write);
					using (dpmStream)
					{
						GetDpmFile(stream, dpmStream);
					}
				}
			}
			catch (Exception)
			{
				return;
			}
		}

		internal void GetDpmFile(Stream exeStream, Stream dpmStream)
		{
			try
			{
				long dpmOffset = seekDpmStart(exeStream);
				if (dpmOffset < 0)
					return;
				exeStream.Seek(dpmOffset, SeekOrigin.Begin);
				int dpmSize = (int)(exeStream.Length - dpmOffset);
				byte[] data = new byte[dpmSize];
				exeStream.Read(data, 0, dpmSize);
				dpmStream.Write(data, 0, dpmSize);
			}
			catch (Exception)
			{
				return;
			}
		}

		private long seekDpmStart(Stream exeStream)
		{
			byte[] header = new byte[4];
			if (exeStream.Length >= 0x25004)
			{
				exeStream.Seek(0x25000, SeekOrigin.Begin);
				exeStream.Read(header, 0, 4);
				if ((header[0] == 0x44) && (header[1] == 0x50) && (header[2] == 0x4d) && (header[3] == 0x58))
					return 0x25000;
			}

			if (exeStream.Length >= 0x1BE04)
			{
				exeStream.Seek(0x1BE00, SeekOrigin.Begin);
				exeStream.Read(header, 0, 4);
				if ((header[0] == 0x44) && (header[1] == 0x50) && (header[2] == 0x4d) && (header[3] == 0x58))
					return 0x25000;
			}

			exeStream.Seek(0, SeekOrigin.Begin);
			long index = 0;
			long length = exeStream.Length;
			while (index < length)
			{

				exeStream.Seek(index, SeekOrigin.Begin);
				if (exeStream.Read(header, 0, 4) < 4)
					break;
				if ((header[0] == 0x44) && (header[1] == 0x50) && (header[0] == 0x4d) && (header[0] == 0x58))
					return index;

				//functionIndex += 0x10;
				index += 0x04;
			}
			return -1;
		}
	}
}
