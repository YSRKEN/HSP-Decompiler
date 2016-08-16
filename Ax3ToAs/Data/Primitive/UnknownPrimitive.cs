using System;
using System.Collections.Generic;
using System.Text;
using KttK.HspDecompiler.Ax3ToAs.Data;
namespace KttK.HspDecompiler.Ax3ToAs.Data.Primitive
{
	internal sealed class UnknownPrimitive : PrimitiveToken
	{
		private UnknownPrimitive() { }
		internal UnknownPrimitive(PrimitiveTokenDataSet dataSet)
			: base(dataSet)
		{
		}

		public override string ToString()
		{
			return DefaultName;
		}
	}
}
