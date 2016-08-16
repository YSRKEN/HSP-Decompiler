using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using KttK.HspDecompiler.Ax2ToAs.Data;

namespace KttK.HspDecompiler.Ax2ToAs
{
	class Ax2Decoder : AbstractAxDecoder
	{
		public override List<string> Decode(BinaryReader reader)
		{
			AxData data = null; AxData.FromStream(reader.BaseStream);
			try
			{
				global::KttK.HspDecompiler.HspConsole.Write("�w�b�_�[��͒�...");
				data = AxData.FromStream(reader.BaseStream);

				global::KttK.HspDecompiler.HspConsole.Write("��͒�...");
				data.Decompile();
			}
			catch (SystemException e)
			{
				throw new HspDecoderException("AxData", "�z��O�̃G���[", e);
			}
			List<string> lines = data.GetLines();

			return data.GetLines();
		}
	}
}
