using System;
using System.Collections.Generic;
using System.Text;
using KttK.HspDecompiler.Ax3ToAs.Data;
using KttK.HspDecompiler.Ax3ToAs.Data.Primitive;
namespace KttK.HspDecompiler.Ax3ToAs.Data.Token
{
	internal sealed class UnknownToken : CodeToken
	{
		private UnknownToken() { }
		internal UnknownToken(PrimitiveToken token)
		{
			this.token = token;
		}
		PrimitiveToken token;
		internal override int TokenOffset
		{
			get { return token.TokenOffset; }
		}

		public override string ToString()
		{
			return " /*" + token.ToString() + "*/";
		}
	}
}
