#if AllowDecryption

using System;
using System.Collections.Generic;
using System.Text;
using System.Security.Cryptography;
namespace KttK.HspDecompiler.DpmToAx.HspCrypto
{
	class HspCryptoTransform
	{

		XorAddTransform xorAdd;

		internal XorAddTransform XorAdd
		{
			get { return xorAdd; }
			set { xorAdd = value; }
		}

		public override string ToString()
		{
			return xorAdd.ToString();
		}

		internal byte[] Encryption(byte[] plain)
		{
			byte[] encrypted = new byte[plain.Length];
			byte prevByte = 0;
			for (int i = 0; i < encrypted.Length; i++)
			{
				encrypted[i] = xorAdd.Encode(XorAddTransform.Dif(plain[i],prevByte));
				prevByte = plain[i];
			}
			return encrypted;
		}


		internal byte[] Decryption(byte[] encrypted)
		{
			byte[] plain = new byte[encrypted.Length];
			byte prevByte = 0;
			for (int i = 0; i < encrypted.Length; i++)
			{
				byte plainByte = xorAdd.Decode(encrypted[i]);
				plain[i] = XorAddTransform.Sum(plainByte, prevByte);
				prevByte = plain[i];
			}
			return plain;
		}

		internal static HspCryptoTransform CrackEncryption(byte[] encrypted)
		{
			byte[] plain3 = new byte[4];
			plain3[0] = 0x48;//H
			plain3[1] = 0x53;//S
			plain3[2] = 0x50;//P
			plain3[3] = 0x33;//3
			HspCryptoTransform hsp3crypto = CrackEncryption(plain3, encrypted);
			if (hsp3crypto != null)
				return hsp3crypto;
			byte[] plain2 = new byte[4];
			plain2[0] = 0x48;//H
			plain2[1] = 0x53;//S
			plain2[2] = 0x50;//P
			plain2[3] = 0x32;//2
			HspCryptoTransform hsp2crypto = CrackEncryption(plain2, encrypted);
			return hsp2crypto;

		}

		internal static HspCryptoTransform CrackEncryption(byte[] plain, byte[] encrypted)
		{
			int count = Math.Min(plain.Length, encrypted.Length);
			if (count < 2)
				throw new Exception("情報サイズが不足");
			byte[] difBuffer = new byte[count];
			//byte baseXor = plain[0];
			byte prevByte = 0;
			byte andByte = 0xFF;
			byte orByte = 0x00;
			for (int i = 0; i < count; i++)
			{
				difBuffer[i] = XorAddTransform.Dif(plain[i] , prevByte);
				prevByte = plain[i];
				//difBuffer[i] ^= baseXor;
				andByte &= difBuffer[i];
				orByte |= difBuffer[i];
			}
			if ((andByte != 0x00) || (orByte != 0xFF))
				throw new Exception("平文の情報が足りません");

			List<XorAddTransform> transformList = new List<XorAddTransform>();
			//deHSP100　総当りテスト。
			for (int i = 0; i < 0x100; i++)
			{
				XorAddTransform xoradd;
				bool ok = true;
				byte add = (byte)(i & 0x7F);
				xoradd.XorSum = (i >= 0x80);
				xoradd.AddByte = add;
				xoradd.XorByte = XorAddTransform.GetXorByte(add, difBuffer[0], encrypted[0], xoradd.XorSum);
				//チェック
				for (int index = 1; index < count; index++)
				{
					if (encrypted[index] != xoradd.Encode(difBuffer[index]))
					{
						ok = false;
						break;
					}
				}
				if (ok)
					transformList.Add(xoradd);
			}
			//候補が2つ以上ならアルゴリズムに問題アリ。
			if (transformList.Count > 1)
				throw new Exception("復号器の異常");
			if (transformList.Count == 1)
			{
				HspCryptoTransform ret = new HspCryptoTransform();
				ret.xorAdd = transformList[0];
				return ret;
			}
			return null;

		}

	}
}
#endif