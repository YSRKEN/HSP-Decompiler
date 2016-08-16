
#if AllowDecryption
using System;
using System.Collections.Generic;
using System.Text;

namespace KttK.HspDecompiler.DpmToAx.HspCrypto
{
	internal struct XorAddTransform
	{
		internal byte XorByte;
		internal byte AddByte;

		public override string ToString()
		{
			return "xor:0x" + XorByte.ToString("X02") + "    " + "add:0x" + AddByte.ToString("X02");
		}

		internal byte Encode(byte b)
		{
			return Sum(Xor(b, XorByte), AddByte);
		}

		internal byte Decode(byte b)
		{
			return Xor(Dif(b, AddByte), XorByte);
		}

		internal static byte Xor(byte b1, byte b2)
		{
			return (byte)(b1 ^ b2);
		}

		internal static byte Sum(byte b1, byte b2)
		{
			return (byte)((b1 + b2) & 0xFF);
		}

		internal static byte Dif(byte b1, byte b2)
		{
			return (byte)((0x100 + b1 - b2) & 0xFF);
		}
	}
}
#endif