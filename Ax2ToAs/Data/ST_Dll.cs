using System;

namespace KttK.HspDecompiler.Ax2ToAs.Data
{
	internal struct Dll
	{
		private string name;
		internal string Name
		{
			get
			{
				return name;
			}
			set
			{
				name = value;
			}
		}
		public override string ToString()
		{
			return "#uselib " + "\"" + name + "\"";
		}
	}
}
