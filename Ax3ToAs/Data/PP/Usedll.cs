using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace KttK.HspDecompiler.Ax3ToAs.Data
{
	internal enum UsedllType
	{
		None = 0x00,
		uselib = 0x01,
		usecom = 0x02
	}

	internal sealed class Usedll : Preprocessor
	{
		private Usedll() { }
		private Usedll(int index):base(index) { }
		private string name = null;
		private string clsName = null;
		private int type;
		private int int_2;
		internal static Usedll FromBinaryReader(BinaryReader reader, AxData parent,int index)
		{
			Usedll ret = new Usedll(index);
			ret.type = reader.ReadInt32();
			int nameOffset = reader.ReadInt32();
			ret.int_2 = reader.ReadInt32();
			int clsNameOffset = reader.ReadInt32();
			switch (ret.Type)
			{
				case UsedllType.usecom:
					ret.name = parent.ReadIidCodeLiteral(nameOffset);
					ret.clsName = parent.ReadStringLiteral(clsNameOffset);
					break;
				case UsedllType.uselib:
					ret.name = parent.ReadStringLiteral(nameOffset);
					break;
			}

			return ret;
		}
		List<Function> functions = new List<Function>();

		internal UsedllType Type
		{
			get
			{
				switch (type)
				{
					case 1:
						return UsedllType.uselib;
					case 4:
						return UsedllType.usecom;
				}
				return UsedllType.None;
			}
		}

		public override string ToString()
		{
			if (name == null)
				return @"//#uselib? //dllñºïsñæ";
			StringBuilder strBld = new StringBuilder();
			switch (this.Type)
			{
				case UsedllType.uselib:
					strBld.Append(@"#uselib """);
					strBld.Append(name);
					strBld.Append(@"""");
					break;
				case UsedllType.usecom:
					strBld.Append(@"#usecom");
					if (functions.Count != 0)
					{
						strBld.Append(' ');
						strBld.Append(functions[0].FunctionName);
					}
					else
					{
						strBld.Append(' ');
						strBld.Append("/*ä÷êîÇ»Çµ*/");
					}

					strBld.Append(' ');
					strBld.Append('"');
					strBld.Append(name);
					strBld.Append('"');
					strBld.Append(' ');
					strBld.Append('"');
					strBld.Append(clsName);
					strBld.Append('"');
					break;
				default:
					return @"//#uselib? //ñ¢ëŒâûÇÃå`éÆ";
			}
			return strBld.ToString();
		}

		internal void AddFunction(Function ret)
		{
			functions.Add(ret);
		}
		internal List<Function> GetFunctions()
		{
			if ((this.Type == UsedllType.usecom) &&(functions.Count != 0))
			{
				List<Function> ret = new List<Function>(functions);
				ret.RemoveAt(0);
				return ret;
			}
			return functions;
		}
	}
}
