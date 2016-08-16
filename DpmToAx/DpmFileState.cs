using System;
using System.Collections.Generic;
using System.Text;

namespace KttK.HspDecompiler.DpmToAx
{
	struct DpmFileState
	{
		internal string FileName;
		internal Int32 unknown;
		internal Int32 Encryptionkey;
		internal Int32 FileOffset;
		internal Int32 FileSize;
		internal bool IsEncrypted
		{
			get
			{
				return Encryptionkey != 0;
			}
		}

		internal Undpm Parent;
		internal byte[] GetFile()
		{
			if (IsEncrypted)
				throw new Exception("�Í����t�@�C���ɂ͑Ή����Ă��܂���");
			if (Parent == null)
				throw new Exception("�e�t�@�C�����ݒ�");
			return Parent.GetFile(FileOffset, FileSize);
		}


	}
}
