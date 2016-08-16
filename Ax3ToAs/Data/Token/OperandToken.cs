using System;
using System.Collections.Generic;
using System.Text;
using KttK.HspDecompiler.Ax3ToAs.Data;
using KttK.HspDecompiler.Ax3ToAs.Data.Primitive;
namespace KttK.HspDecompiler.Ax3ToAs.Data.Token
{
	abstract class OperandToken : ExpressionTermToken
	{
		internal OperandToken() { }

		internal override bool IsOperand
		{
			get { return true; }
		}

		internal override bool IsOperator
		{
			get { return false; }
		}


	}
}
