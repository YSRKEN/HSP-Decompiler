using System;
using System.Collections.Generic;
using System.Text;

namespace KttK.HspDecompiler.Ax3ToAs.Data
{
	internal sealed class Cmd : Preprocessor
	{
		private Cmd() { }
		internal Cmd(int pluginIndex, int methodIndex)
		{
			this.pluginIndex = pluginIndex;
			this.methodIndex = methodIndex;
		}
		private int pluginIndex;
		private int methodIndex;

		internal string FunctionName
		{
			get
			{
				StringBuilder strbd = new StringBuilder();
				strbd.Append("cmd_");
				strbd.Append(pluginIndex.ToString());
				strbd.Append('_');
				strbd.Append(methodIndex.ToString());
				return strbd.ToString();
			}
		}

		public override string ToString()
		{
			StringBuilder strbd = new StringBuilder();
			strbd.Append("#cmd ");
			strbd.Append(FunctionName);
			strbd.Append(' ');
			strbd.Append(methodIndex.ToString());
			return strbd.ToString();
		}
	}
}
