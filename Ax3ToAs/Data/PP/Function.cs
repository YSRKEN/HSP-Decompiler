using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace KttK.HspDecompiler.Ax3ToAs.Data
{

	internal enum FunctionType
	{
		NULL = 0x00,
		func = 0x01,
		cfunc = 0x02,
		deffunc = 0x03,
		defcfunc = 0x04,
		comfunc = 0x05,
		module = 0x06,
	}

	internal enum FunctionFlags
	{
		NULL = 0,
		onexit = 0x01,
	}

	class Function : Preprocessor
	{
		private Function() { }
		private Function(int index) : base(index) { }
		private int dllIndex;
		private int functionIndex;

		List<Param> functionParams = new List<Param>();
		int strIndex;
		private int paramSizeSum;
		private int labelIndex;
		private Int16 int_0;
		private int flags;
		internal static Function FromBinaryReader(BinaryReader reader, AxData parent, int index)
		{
			Function ret = new Function(index);
			ret.dllIndex = reader.ReadInt16();
			ret.functionIndex = reader.ReadInt16();

			int paramStart = reader.ReadInt32();
			int paramCount = reader.ReadInt32();
			if (paramCount != 0)
				ret.functionParams = parent.FunctionParams.GetRange(paramStart, paramCount);
			ret.strIndex = reader.ReadInt32();
			if (ret.strIndex >= 0)
				ret.defaultName = parent.ReadStringLiteral(ret.strIndex);
			ret.paramSizeSum = reader.ReadInt32();
			ret.labelIndex = reader.ReadInt32();

			ret.int_0 = reader.ReadInt16();
			ret.flags = reader.ReadInt16();
			switch (ret.Type)
			{
				case FunctionType.defcfunc:
				case FunctionType.deffunc:
					Label label = parent.GetLabel(ret.labelIndex);
					if (label != null)
						label.SetFunction(ret);
					ret.label = label;
					break;

				case FunctionType.func:
				case FunctionType.cfunc:
				case FunctionType.comfunc:
					Usedll dll = parent.GetUsedll(ret.dllIndex);
					if (dll != null)
						dll.AddFunction(ret);
					ret.dll = dll;
					break;
				case FunctionType.module:
					parent.Modules.Add(ret);
					break;
			}
			return ret;
		}

		internal bool IsModuleFunction
		{
			get
			{
				return this.Type == FunctionType.module;
			}
		}
		internal bool IsComFunction
		{
			get
			{
				return this.Type == FunctionType.comfunc;
			}
		}
		internal bool IsUserFunction
		{
			get{
				switch (this.Type)
				{
					case FunctionType.deffunc:
					case FunctionType.defcfunc:
						return true;
				}
				return false;
			}
		}

		internal bool IsDllFunction
		{
			get
			{
				switch (this.Type)
				{
					case FunctionType.func:
					case FunctionType.cfunc:
						return true;
				}
				return false;
			}
		}

		private string defaultName = null;

		internal string DefaultName
		{
			get { return defaultName; }
		}

		internal Function ParentModule
		{
			get
			{
				if (functionParams.Count == 0)
					return null;
				if (!functionParams[0].IsModuleType)
					return null;
				return functionParams[0].Module;

			}
		}
		private string name = null;
		private Label label = null;
		private Usedll dll = null;
		internal FunctionType Type
		{
			get
			{
				if (dllIndex == -1)
					return FunctionType.deffunc;
				if (dllIndex == -2)
					return FunctionType.defcfunc;
				if (dllIndex == -3)
					return FunctionType.module;
				if (dllIndex >= 0)
				{
					//if (strIndex == -1)
					if (functionIndex == -7)
						return FunctionType.comfunc;
					if (labelIndex == 2)
						return FunctionType.func;
					if (labelIndex == 3)///func onexit
						return FunctionType.func;
					if (labelIndex == 4)
						return FunctionType.cfunc;
				}
				return FunctionType.NULL;
			}
		}
		internal FunctionFlags Flags
		{
			get
			{
				if ((flags == 1)&&(dllIndex == -1))
					return FunctionFlags.onexit;
				if ((dllIndex >= 0)&&(labelIndex == 3))
					return FunctionFlags.onexit;
				return FunctionFlags.NULL;
			}
		}
		internal void SetName(string name)
		{
			this.name = name;
		}

		internal string FunctionName
		{
			get
			{
				if (name != null)
					return name;
				if (defaultName == null)
				{
					if (this.Type == FunctionType.comfunc)
						return "comfunc_" + index.ToString();
					return null;
				}
				switch (this.Type)
				{
					case FunctionType.defcfunc:
					case FunctionType.deffunc:
					case FunctionType.module:
						return defaultName;
					case FunctionType.func:
					case FunctionType.cfunc:
						if(name != null)
							return name;
						return defaultName;
					case FunctionType.comfunc:
						return "comfunc_" + index.ToString();
					default:
						break;
				}
				return null;
			}
		}

		private string modFunctionToString()
		{
			StringBuilder strBld = new StringBuilder();
			switch (this.defaultName)
			{
				case "__init":
					strBld.Append("#modinit");
					break;
				case "__term":
					strBld.Append("#modterm");
					break;
				default:
					strBld.Append("#modfunc");
					strBld.Append(' ');
					strBld.Append(this.FunctionName);
					break;
			}
			if (functionParams.Count > 1)
			{
				for (int i = 1; i < functionParams.Count; i++)
				{
					if (i != 1)
						strBld.Append(',');
					strBld.Append(' ');
					strBld.Append(functionParams[i].ToString());
				}
			}
			return strBld.ToString();
		}

		private string moduleToString(bool useModuleStyle)
		{
			StringBuilder strBld = new StringBuilder();
			if (useModuleStyle)
			{
				strBld.Append("#module ");
				strBld.Append(this.FunctionName);
			}
			else
			{
				strBld.Append("#struct ");
				strBld.Append(this.FunctionName);
			}
			if (functionParams.Count > 1)
			{
				for (int i = 1; i < functionParams.Count; i++)
				{
					if (i != 1)
						strBld.Append(',');
					strBld.Append(' ');
					if (useModuleStyle)
					{
						strBld.Append(functionParams[i].ToString(true, true, true));
					}
					else
					{
						strBld.Append(functionParams[i].ToString(true, false, true));
					}
				}
			}
			return strBld.ToString();
		}

		internal string ToString(bool useModuleStyle)
		{
			StringBuilder strBld = new StringBuilder();

			int paramStart = 0;
			switch (this.Type)
			{
				case FunctionType.defcfunc:
					strBld.Append("#defcfunc ");
					strBld.Append(this.FunctionName);
					break;
				case FunctionType.module:
					return moduleToString(useModuleStyle);
				case FunctionType.deffunc:
					if(useModuleStyle)
						if ((functionParams.Count != 0) && (functionParams[0].IsModuleType))
							return modFunctionToString();
					strBld.Append("#deffunc ");
					strBld.Append(this.FunctionName);
					if ((Flags & FunctionFlags.onexit) == FunctionFlags.onexit)
						strBld.Append(" onexit");
					break;
				case FunctionType.func:
					strBld.Append("#func ");
					strBld.Append(this.FunctionName);
					strBld.Append(' ');
					if ((Flags & FunctionFlags.onexit) == FunctionFlags.onexit)
						strBld.Append("onexit ");
					strBld.Append('"');
					strBld.Append(defaultName);
					strBld.Append('"');
					break;
				case FunctionType.cfunc:
					strBld.Append("#cfunc ");
					strBld.Append(this.FunctionName);
					strBld.Append(@" """);
					strBld.Append(defaultName);
					strBld.Append('"');
					break;
				case FunctionType.comfunc:
					strBld.Append("#comfunc ");
					strBld.Append(this.FunctionName);
					strBld.Append(' ');
					strBld.Append(labelIndex.ToString());
					paramStart = 1;
					break;
				default:
					return "/*#deffunc?*/";

			}
			if (functionParams.Count > paramStart)
			{

				for (int i = paramStart; i < functionParams.Count; i++)
				{
					if (i != paramStart)
						strBld.Append(',');
					strBld.Append(' ');
					strBld.Append(functionParams[i].ToString());
				}
			}
			return strBld.ToString();
		}

		public override string ToString()
		{
			return ToString(false);
		}
	}
}
