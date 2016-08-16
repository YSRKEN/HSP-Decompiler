using System;
using System.Collections.Generic;
using System.Text;
using KttK.HspDecompiler.Ax3ToAs.Data;
namespace KttK.HspDecompiler.Ax3ToAs.Data.Primitive
{
	internal abstract class LiteralPrimitive : OperandPrimitive
	{
		protected LiteralPrimitive() { }
		internal virtual bool IsNegativeNumber { get { return false; } }
		internal virtual bool IsMinusOne { get { return false; } }
		internal LiteralPrimitive(PrimitiveTokenDataSet dataSet)
			: base(dataSet)
		{
		}
	}

	internal sealed class LabelPrimitive : LiteralPrimitive
	{
		private LabelPrimitive() { }
		internal LabelPrimitive(PrimitiveTokenDataSet dataSet)
			: base(dataSet)
		{
			label = dataSet.Parent.Labels[Value];
		}
		readonly Label label = null;

		public override string ToString()
		{
			if (label == null)
				return DefaultName;
			return label.LabelName;
		}

		internal void LabelIsUsed()
		{
			if (label == null)
				return;
			label.Visible = true;
		}
	}

	internal sealed class IntegerPrimitive : LiteralPrimitive
	{
		private IntegerPrimitive() { }
		internal IntegerPrimitive(PrimitiveTokenDataSet dataSet)
			: base(dataSet)
		{
		}

		internal override bool IsNegativeNumber
		{
			get
			{
				return (Value < 0);
			}
		}
		internal override bool IsMinusOne
		{
			get
			{
				return Value == -1;
			}
		}
		public override string ToString()
		{
			return Value.ToString();
		}
	}

	internal sealed class DoublePrimitive : LiteralPrimitive
	{
		private DoublePrimitive() { }
		internal DoublePrimitive(PrimitiveTokenDataSet dataSet, double d)
			: base(dataSet)
		{
			this.d = d;
		}
		readonly double d = 0;

		internal override bool IsNegativeNumber
		{
			get
			{
				return (d < 0.0);
			}
		}
		public override string ToString()
		{
			//みっともない。いい方法ないかな？
			//指数表示はだめ。(3.73562892357e-12とかは認識されない。)
			//たとえ整数でも.0をつけること(そうしないとint型リテラルと認識される。一律DまたはFのプレフィックスをつけてもよい)
			//たとえ1未満でも0.をつけること(.001とかは認識してくれない。)
			//たとえとっても小さくても0.0にしてはだめ。(0.000000000000000000000000001とか1e-300とかでも0にしてくれちゃ困る)
			return d.ToString("0.0#########################################################################################################################################################################################################################################################################################################################################################");
			//         var_0 = 0.0000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000999999999999997

			//         var_0 = 0.0000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000988131291682493		
		}
	}

	internal sealed class StringPrimitive : LiteralPrimitive
	{
		private StringPrimitive() { }
		internal StringPrimitive(PrimitiveTokenDataSet dataSet, string str)
			: base(dataSet)
		{
			this.str = str;
		}
		readonly string str = null;
		public override string ToString()
		{
			StringBuilder builder = new StringBuilder();
			builder.Append('"');
			builder.Append(str);
			builder.Append('"');
			return builder.ToString();
		}
	}

	internal sealed class SymbolPrimitive : LiteralPrimitive
	{
		private SymbolPrimitive() { }
		internal SymbolPrimitive(PrimitiveTokenDataSet dataSet)
			: base(dataSet)
		{
		}
	}
}
