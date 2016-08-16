using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
namespace KttK.HspDecompiler.Ax3ToAs.Data
{
	class PlugIn : Preprocessor
	{
		private PlugIn() { }
		private PlugIn(int index):base(index) { }
		int int_0;
		string dllName = null;
		string exportName = null;
		int int_3;
		internal static PlugIn FromBinaryReader(BinaryReader reader, AxData parent, int index)
		{
			PlugIn ret = new PlugIn(index);
			ret.int_0 = reader.ReadInt32();
			int dllNameOffset = reader.ReadInt32();
			int exportNameOffset = reader.ReadInt32();
			ret.int_3 = reader.ReadInt32();
			ret.dllName = parent.ReadStringLiteral(dllNameOffset);
			ret.exportName = parent.ReadStringLiteral(exportNameOffset);
			return ret;
		}

		Dictionary<int, Cmd> cmds = new Dictionary<int, Cmd>();
		private int extendedTypeCount = 0;

		internal int ExtendedTypeCount
		{
			get { return extendedTypeCount; }
			set { extendedTypeCount = value; }
		}
		internal Cmd AddCmd(int methodIndex)
		{
			Cmd cmd = null;
			if(cmds.TryGetValue(methodIndex, out cmd))
				return cmd;
			cmd = new Cmd(index, methodIndex);
			cmds.Add(methodIndex, cmd);
			return cmd;
		}
		internal Dictionary<int, Cmd> GetCmds()
		{
			return cmds;
		}

		public override string ToString()
		{
			StringBuilder strbd = new StringBuilder();
			strbd.Append("#regcmd");
			strbd.Append(' ');
			strbd.Append('"');
			strbd.Append(exportName);
			strbd.Append('"');
			strbd.Append(',');
			strbd.Append(' ');
			strbd.Append('"');
			strbd.Append(dllName);
			strbd.Append('"');
			if (extendedTypeCount != 0)
			{
				strbd.Append(',');
				strbd.Append(' ');
				strbd.Append(extendedTypeCount.ToString());
			}
				

			return strbd.ToString();
		}

		
	}
}
