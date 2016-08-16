using System;
using System.Collections.Generic;
using System.Text;
using KttK.HspDecompiler.Ax3ToAs.Data;
using KttK.HspDecompiler.Ax3ToAs.Data.Primitive;
namespace KttK.HspDecompiler.Ax3ToAs.Data.Token
{
	/// <summary>
	/// 演算子トークン
	/// </summary>
	internal sealed class OperatorToken : ExpressionTermToken
	{
		private OperatorToken() { }
		internal OperatorToken(OperatorPrimitive source)
		{
			primitive = source;
		}

		readonly OperatorPrimitive primitive = null;
		internal override int TokenOffset
		{
			get { return primitive.TokenOffset; }
		}

		public override string ToString()
		{
			return primitive.ToString();
		}

		internal string ToString(bool isAssignment, bool hasExpression)
		{
			string ret = primitive.ToString();
			if (primitive.CodeType != HspCodeType.Operator)
				return primitive.ToString();
			if (isAssignment)
			{
				if ((!hasExpression) && (ret == "+"))
					return "++";
				else if ((!hasExpression) && (ret == "-"))
					return "--";
				switch (ret)
				{
					case "=":
					case ">":
					case "<":// a <= bは a = (a<=b)と解釈される
						return ret;
					default:
						return ret + "=";
				}
			}
			else
			{
				switch (ret)
				{
					case "=":
					case "!":
						return ret + "=";
				}
			}
			return ret;
		}


		internal override bool IsOperand
		{
			get { return false; }
		}

		internal override bool IsOperator
		{
			get { return true; }
		}

		internal override int Priority
		{
			get { return primitive.OperatorPriority; }
		}
	}
}
