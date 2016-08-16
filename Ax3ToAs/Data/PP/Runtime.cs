using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace KttK.HspDecompiler.Ax3ToAs.Data
{
	class Runtime : Preprocessor
	{
		private Runtime() { }
		internal Runtime(string theName)
		{
			name = theName;
		}

		string name;

		public override string ToString()
		{
			StringBuilder strbd = new StringBuilder();
			strbd.Append("#runtime ");
			strbd.Append(@"""");
			strbd.Append(name);
			strbd.Append(@"""");
			return strbd.ToString();

		}
	}
}
