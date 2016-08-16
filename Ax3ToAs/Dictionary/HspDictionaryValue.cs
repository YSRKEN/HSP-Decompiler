using System;
using System.Collections.Generic;
using System.Text;

namespace KttK.HspDecompiler.Ax3ToAs.Dictionary
{
	internal struct HspDictionaryValue
	{
		internal HspDictionaryValue(string theName, string theType, string[] theExtras)
		{
			Name = theName;
			Type = (HspCodeType)Enum.Parse(typeof(HspCodeType), theType);
			Extra = HspCodeExtraFlags.NONE;
			OparatorPriority = -1;
			foreach (string theExtra in theExtras)
			{
				string testString = theExtra.Trim();
				if (testString.Length == 0)
					continue;
				if (testString.StartsWith("Priority_"))
				{
					OparatorPriority = int.Parse(testString.Substring(9));
					continue;
				}
				Extra |= (HspCodeExtraFlags)Enum.Parse(typeof(HspCodeExtraFlags), testString);
			}
		}

		internal string Name;
		internal HspCodeType Type;
		internal HspCodeExtraFlags Extra;
		internal int OparatorPriority;

		public override string ToString()
		{
			if (Name.Length == 0)
				return Type.ToString();
			return Type.ToString() + "  \"" + Name + "\"";
		}

		
	}
}
