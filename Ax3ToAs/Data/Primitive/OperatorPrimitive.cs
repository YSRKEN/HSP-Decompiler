using System;
using System.Collections.Generic;
using System.Text;
using KttK.HspDecompiler.Ax3ToAs.Data;
namespace KttK.HspDecompiler.Ax3ToAs.Data.Primitive
{
	internal sealed class OperatorPrimitive :PrimitiveToken
	{
		private OperatorPrimitive() { }
		internal OperatorPrimitive(PrimitiveTokenDataSet dataSet)
			: base(dataSet)
		{
		}
	}
}
