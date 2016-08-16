using System;
using System.Collections.Generic;
using System.Text;

namespace KttK.HspDecompiler.Ax3ToAs.Data.Token
{
	abstract class ExpressionTermToken : CodeToken
	{
		internal abstract bool IsOperand { get;}
		internal abstract bool IsOperator { get;}
		internal virtual bool IsLabel { get { return false; } }
		internal abstract int Priority { get;}
	}
}
