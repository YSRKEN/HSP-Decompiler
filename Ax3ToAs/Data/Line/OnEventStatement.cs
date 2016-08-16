using System;
using System.Collections.Generic;
using System.Text;
using KttK.HspDecompiler.Ax3ToAs.Data;
using KttK.HspDecompiler.Ax3ToAs.Data.Token;
using KttK.HspDecompiler.Ax3ToAs.Data.Primitive;

namespace KttK.HspDecompiler.Ax3ToAs.Data.Line
{
	/// <summary>
	/// if, elseç\ï∂
	/// </summary>
	internal sealed class OnEventStatement : LogicalLine
	{
		private OnEventStatement() { }
		internal OnEventStatement(OnEventFunctionPrimitive theToken, FunctionToken func)
		{
			this.token = theToken;
			this.func = func;
		}

		private readonly OnEventFunctionPrimitive token = null;//on####
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
			if (func != null)
			{
				builder.Append(' ');
				builder.Append(func.ToString());
			}
			return builder.ToString();
		}

		internal override void CheckLabel()
		{
			if (func != null)
				func.CheckLabel();
		}

		internal override bool CheckRpn()
		{
			if (func != null)
				return func.CheckRpn();
			return true;
		}
	}
}
