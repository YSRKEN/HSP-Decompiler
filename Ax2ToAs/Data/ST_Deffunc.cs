using System;

namespace KttK.HspDecompiler.Ax2ToAs.Data
{


	internal struct Deffunc
	{
		private string name;
		private int hikiType;
		private int hikiCount;
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

		internal int HikiCount
		{
			get
			{
				return hikiCount;
			}
			set
			{
				hikiCount = value;
			}
		}

		public override string ToString()
		{
			string hiki = "";
			if (hikiCount >= 1)
			{
				if ((hikiType & 1) != 0)
					hiki = "val";
				else if ((hikiType & 2) != 0)
					hiki = "str";
				else
					hiki = "int";
			}
			if (hikiCount >= 2)
			{
				if ((hikiType & 0x10) != 0)
					hiki += ", val";
				else if ((hikiType & 0x20) != 0)
					hiki += ", str";
				else
					hiki += ", int";
			}
			for (int i = 0; i<(hikiCount -2); i++)
			{
				hiki += ", int";
			}

			return "#deffunc " + name  + " " + hiki;
		}
	}
}
