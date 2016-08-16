using System;
using System.Collections.Generic;
using System.Text;
using KttK.HspDecompiler.Ax3ToAs.Data;
using KttK.HspDecompiler.Ax3ToAs.Data.Token;

namespace KttK.HspDecompiler.Ax3ToAs.Data.Line
{
	/// <summary>
	/// �����
	/// </summary>
	internal sealed class Assignment : LogicalLine
	{
		private Assignment() { }
		internal Assignment(VariableToken theVar, OperatorToken theOp)
		{
			var = theVar;
			op = theOp;
		}
		internal Assignment(VariableToken theVar, OperatorToken theOp, ArgumentToken theArg)
		{
			var = theVar;
			op = theOp;
			arg = theArg;
		}

		readonly VariableToken var = null;
		readonly OperatorToken op = null;

		//���ʂ͂ЂƂ̎������A�z��ϐ��ɂ͂������������邱�Ƃ�����B
		readonly ArgumentToken arg = null;

		internal override int TokenOffset
		{
			get
			{
				if (var == null)
					return -1;
				return var.TokenOffset;
			}
		}

		public override string ToString()
		{
			StringBuilder builder = new StringBuilder(var.ToString());
			if (arg != null)
			{
				builder.Append(' ');
				builder.Append(op.ToString(true, arg != null));
				builder.Append(arg.ToString());
			}
			else
			{
				builder.Append(op.ToString(true, arg != null));//a++�Ƃ��BHSP�ł͂��̕ӂ̏����͂��������݂����B
			}
			return builder.ToString();
		}



		internal override void CheckLabel()
		{
			if(var != null)
				var.CheckLabel();
			if (op != null)
				op.CheckLabel();
			if (arg != null)
				arg.CheckLabel();
		}

		internal override bool CheckRpn()
		{
			bool ret = true;
			if (var != null)
				ret &= var.CheckRpn();
			if (arg != null)
				ret &= arg.CheckRpn();
			return true;
		}
	}
}
