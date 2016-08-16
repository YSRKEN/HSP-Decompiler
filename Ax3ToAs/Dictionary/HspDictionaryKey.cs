using System;
using System.Collections.Generic;
using System.Text;

namespace KttK.HspDecompiler.Ax3ToAs.Dictionary
{
	internal struct HspDictionaryKey : IComparable<HspDictionaryKey>, IEquatable<HspDictionaryKey>
	{
		internal HspDictionaryKey(HspDictionaryKey key)
		{
			Type = key.Type;
			Value = key.Value;
			AllValue = key.AllValue;
		}

		internal HspDictionaryKey(string theType, string theValue)
		{
			Type = DicParser.StringToInt32(theType);
			Value = DicParser.StringToInt32(theValue);
			AllValue = false;
			if (Value == -1)
				AllValue = true;
		}
		
		internal int Type;
		internal int Value;
		internal bool AllValue;

		public override string ToString()
		{
			if (Value == -1) 
				return  "Type:0x" + Type.ToString("X02") + "Value:0xFFFF";
			return "Type:0x" + Type.ToString("X02") + "Value:0x" + Value.ToString("X04");
		}

		public override bool Equals(object obj)
		{
			if (obj.GetType() != typeof(HspDictionaryKey))
				throw new Exception("サポート外");

			return Equals((HspDictionaryKey)obj);
		}

		public override int GetHashCode()
		{
			return Type.GetHashCode() ^ Value.GetHashCode();
		}

		#region IEquatable<HspDictionaryKey> メンバ

		public bool Equals(HspDictionaryKey other)
		{
			return Type.Equals(other.Type) && Value.Equals(other.Value);
		}

		#endregion

		#region IComparable<HspDictionaryKey> メンバ

		public int CompareTo(HspDictionaryKey other)
		{
			int ret = Type.CompareTo(other.Type);
			if (ret != 0)
				return ret;
			return Value.CompareTo(other.Value);
		}

		#endregion

	}
}
