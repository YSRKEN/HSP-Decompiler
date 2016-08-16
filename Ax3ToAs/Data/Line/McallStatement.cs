using System;
using System.Collections.Generic;
using System.Text;
using KttK.HspDecompiler.Ax3ToAs.Data;
using KttK.HspDecompiler.Ax3ToAs.Data.Token;
using KttK.HspDecompiler.Ax3ToAs.Data.Primitive;

namespace KttK.HspDecompiler.Ax3ToAs.Data.Line
{
	class McallStatement : LogicalLine
	{
		private McallStatement() { }
		internal McallStatement(McallFunctionPrimitive theToken, VariablePrimitive var, ExpressionToken exp, ArgumentToken arg)
		{
			this.token = theToken;
			this.var = var;
			this.exp = exp;
			this.arg = arg;
		}
		private readonly McallFunctionPrimitive token = null;
		private readonly VariablePrimitive var = null;//配列変数も受け付けない。
		private readonly ExpressionToken exp = null;
		private readonly ArgumentToken arg = null;

		internal override int TokenOffset
		{
			get { return token.TokenOffset; }
		}

		private string ToStringFunctionStyle()
		{
			if (arg == null)
				return token.ToString();
			StringBuilder builder = new StringBuilder();
			builder.Append(token.ToString());
			if (var != null)
			{
				builder.Append(' ');
				builder.Append(var.ToString());
				if (exp != null)
				{
					builder.Append(' ');
					builder.Append(',');
					builder.Append(var.ToString());
				}
			}
			if (arg != null)
			{
				builder.Append(arg.ToString());
			}
			return builder.ToString();
		}

		internal string ToString(bool convertMcall)
		{
			if (!convertMcall)
				return ToStringFunctionStyle();
			if (var == null)
				return ToStringFunctionStyle();
			if (exp == null)
				return ToStringFunctionStyle();
			if (arg == null)
				return ToStringFunctionStyle();
			StringBuilder builder = new StringBuilder();
			builder.Append(var.ToString());
			builder.Append("->");
			builder.Append(exp.ToString());
			//deHSP1.20のバグ修正。expとargの間にカンマを入れないように修正。
			if(arg != null)
				builder.Append(arg.ToString(true));

			return builder.ToString();
		}

		public override string ToString()
		{
			return ToString(true);
		}

		internal override void CheckLabel()
		{
			if (exp != null)
				exp.CheckLabel();
			if (arg != null)
				arg.CheckLabel();
		}

		internal override bool CheckRpn()
		{
			bool ret = true;
			if (exp != null)
				ret &= exp.CheckRpn();
			if (arg != null)
				ret &= arg.CheckRpn();
			return ret;
		}
	}
}
