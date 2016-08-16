using System;
using System.Collections.Generic;
using System.Text;
using KttK.HspDecompiler.Ax3ToAs.Data;
namespace KttK.HspDecompiler.Ax3ToAs.Data.Primitive
{
	internal abstract class FunctionPrimitive : PrimitiveToken
	{
		protected FunctionPrimitive() { }
		internal FunctionPrimitive(PrimitiveTokenDataSet dataSet)
			: base(dataSet)
		{
		}
		
	}

	internal sealed class UserFunctionPrimitive : FunctionPrimitive
	{
		private UserFunctionPrimitive() { }
		internal UserFunctionPrimitive(PrimitiveTokenDataSet dataSet)
			: base(dataSet)
		{
			func = dataSet.Parent.GetUserFunction(Value);
		}
		private readonly Function func = null;
		public override string ToString()
		{
			if (func == null)
				return DefaultName;
			return func.FunctionName;
		}
	}

	internal sealed class DllFunctionPrimitive : FunctionPrimitive
	{
		private DllFunctionPrimitive() { }
		internal DllFunctionPrimitive(PrimitiveTokenDataSet dataSet)
			: base(dataSet)
		{
			func = dataSet.Parent.GetDllFunction(Value);
		}
		private readonly Function func = null;
		public override string ToString()
		{
			if (func == null)
				return DefaultName;
			return func.FunctionName;
		}
	}

	internal sealed class PlugInFunctionPrimitive : FunctionPrimitive
	{
		private PlugInFunctionPrimitive() { }
		internal PlugInFunctionPrimitive(PrimitiveTokenDataSet dataSet)
			: base(dataSet)
		{
			int pluginIndex = dataSet.DicValue.OparatorPriority;
			cmd = dataSet.Parent.AddCmd(pluginIndex, Value);
		}
		private readonly Cmd cmd = null;
		public override string ToString()
		{
			if (cmd == null)
				return DefaultName;
			return cmd.FunctionName;
		}
	}

	internal sealed class ComFunctionPrimitive : FunctionPrimitive
	{
		private ComFunctionPrimitive() { }
		internal ComFunctionPrimitive(PrimitiveTokenDataSet dataSet)
			: base(dataSet)
		{
			func = dataSet.Parent.GetDllFunction(Value - 0x1000);
		}
		private readonly Function func = null;
		public override string ToString()
		{
			if (func == null)
				return DefaultName;
			return func.FunctionName;
		}
	}
}

