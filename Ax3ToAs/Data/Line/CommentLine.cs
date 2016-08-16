using System;
using System.Collections.Generic;
using System.Text;

namespace KttK.HspDecompiler.Ax3ToAs.Data.Line
{
	class CommentLine :LogicalLine
	{
		internal CommentLine() { }
		internal CommentLine(string str)
		{
			comment = str;

		}
		private readonly string comment = null;
		internal override int TokenOffset
		{
			get { return -1; }
		}

		internal override int TabCount
		{
			get
			{
				return 0;
			}
		}

		public override string ToString()
		{
			if (comment == null)
				return string.Empty;
			StringBuilder strbd = new StringBuilder();
			strbd.Append("//");
			strbd.Append(comment);
			return strbd.ToString();
		}
	}
}
