using System;
using System.Collections.Generic;
using System.Text;
using KttK.HspDecompiler.Ax3ToAs.Data;
namespace KttK.HspDecompiler.Ax3ToAs.Data.Primitive
{
	internal abstract class OperandPrimitive : PrimitiveToken
	{
		protected OperandPrimitive() { }
		internal OperandPrimitive(PrimitiveTokenDataSet dataSet)
			: base(dataSet)
		{
		}
	}
}
