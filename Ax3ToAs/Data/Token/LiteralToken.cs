using System;
using System.Collections.Generic;
using System.Text;
using KttK.HspDecompiler.Ax3ToAs.Data;
using KttK.HspDecompiler.Ax3ToAs.Data.Primitive;

namespace KttK.HspDecompiler.Ax3ToAs.Data.Token
{
	internal sealed class LiteralToken : OperandToken
	{
		private LiteralToken() { }
		internal LiteralToken(LiteralPrimitive token)
		{
			this.token = token;
		}

		readonly private LiteralPrimitive token = null;
		internal bool IsNegativeNumber
		{
			get
			{
				if (token == null)
					return false;
				return token.IsNegativeNumber;
			}
		}

		internal bool IsMinusOne
		{
			get { return token.IsMinusOne; }
		}

		internal override int TokenOffset
		{
			get
			{
				if (token == null)
					return -1;
				return token.TokenOffset;
			}
		}

		public override string ToString()
		{
			if((token.CodeType == HspCodeType.Symbol) && (token.ToString() == "?"))
				return "";
			return token.ToString();
		}

		internal override int Priority
		{
			get
			{
				if (this.IsNegativeNumber)
					return -1;
				return 100;
			}
		}

		internal override void CheckLabel()
		{
			LabelPrimitive label = token as LabelPrimitive;
			if (label != null)
			{
				label.LabelIsUsed();
			}
		}
	}
}
