using System;
using System.Collections.Generic;
using System.Text;
using KttK.HspDecompiler.Ax3ToAs.Data;
using KttK.HspDecompiler.Ax3ToAs.Data.Token;
using KttK.HspDecompiler.Ax3ToAs.Data.Primitive;

namespace KttK.HspDecompiler.Ax3ToAs.Data.Line
{
	/// <summary>
	/// on Å` goto/gosub labelç\ï∂
	/// </summary>
	internal sealed class OnStatement : LogicalLine
	{
		private OnStatement() { }
		internal OnStatement(OnFunctionPrimitive theToken, ExpressionToken exp, FunctionToken func)
		{
			this.token = theToken;
			this.exp = exp;
			this.func = func;
		}

		private readonly OnFunctionPrimitive token = null;//on
		private readonly ExpressionToken exp = null;//èåè
		private readonly FunctionToken func = null;//goto/gosub Å`
		internal override int TokenOffset
		{
			get {return token.TokenOffset; }
		}

		public override string ToString()
		{
			StringBuilder builder = new StringBuilder();
			if(token != null)
			{
				builder.Append(token.ToString());
			}
			if (exp != null)
			{
				builder.Append(' ');
				builder.Append(exp.ToString());
			}
			if (func != null)
			{
				builder.Append(' ');
				builder.Append(func.ToString());
			}
			return builder.ToString();
		}

		internal override void CheckLabel()
		{
			if (exp != null)
				exp.CheckLabel();
			if (func != null)
				func.CheckLabel();
				
		}

		internal override bool CheckRpn()
		{
			bool ret = true;
			if (exp != null)
				ret &= exp.CheckRpn();
			if (func != null)
				ret &= func.CheckRpn();
			return ret;
		}
	}
}
