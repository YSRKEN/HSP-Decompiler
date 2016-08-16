using System;
using System.Collections.Generic;
using System.Text;
using KttK.HspDecompiler.Ax3ToAs.Data;

namespace KttK.HspDecompiler.Ax3ToAs.Data.Primitive
{
	internal sealed class IfStatementPrimitive : HspFunctionPrimitive
	{
		private IfStatementPrimitive() { }
		internal IfStatementPrimitive(PrimitiveTokenDataSet dataSet, int extraValue)
			: base(dataSet)
		{
			this.extraValue = extraValue;
		}

		private readonly int extraValue = -1;
		internal int JumpToOffset
		{
			get
			{
				if (extraValue == -1)
					return -1;
				int ret = extraValue + TokenOffset;
				if (this.HasLongTypeValue)
					ret += 4;
				else
					ret += 3;
				return ret;
			}
		}

		internal override string DefaultName
		{
			get
			{

				StringBuilder builder = new StringBuilder();
				builder.Append("/*");
				builder.Append(type.ToString("X02"));
				builder.Append(' ');
				builder.Append(flag.ToString("X02"));
				builder.Append(' ');
				if (HasLongTypeValue)
					builder.Append(Value.ToString("X08"));
				else
					builder.Append(Value.ToString("X04"));
				builder.Append(' ');
				builder.Append(extraValue.ToString("X04"));
				builder.Append("*/");
				return builder.ToString();
			}
		}
	}
}
