using System;

namespace KttK.HspDecompiler.Ax2ToAs.Data
{
	internal struct Func
	{
		private string name;
		private int hikiType;
		private int dllIndex;
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

		internal int HikiType
		{
			get
			{
				return hikiType;
			}
			set
			{
				hikiType = value;
			}
		}

		internal int DllIndex
		{
			get
			{
				return dllIndex;
			}
			set
			{
				dllIndex = value;
			}
		}

		public override string ToString()
		{
			return "#func func_" + name + " " + name + " $" + hikiType.ToString("x4");
		}
	}
}
