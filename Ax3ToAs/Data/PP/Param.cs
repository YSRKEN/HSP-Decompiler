using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using KttK.HspDecompiler.Ax3ToAs.Dictionary;
namespace KttK.HspDecompiler.Ax3ToAs.Data
{

	class Param : Preprocessor
	{
		private Param() { }
		private Param(int paramIndex):base(paramIndex){}
		string paramTypeName = "NULL";
		/// <summary>
		/// structのindex。
		/// </summary>
		short deffuncIndex;
		/// <summary>
		/// パラメーターの開始サイズまたはmodinit関数のindex
		/// </summary>
		int paramStartByte;
		internal static Param FromBinaryReader(BinaryReader reader, AxData parent, int index)
		{
			Param ret = new Param(index);
			ret.paramType = reader.ReadUInt16();
			if (!parent.Dictionary.ParamLookUp(ret.paramType, out ret.paramTypeName))
				ret.paramTypeName = "NULL";
			ret.deffuncIndex = reader.ReadInt16();
			ret.paramStartByte = reader.ReadInt32();

			return ret;
		}

		private bool paramNameIsUsed = false;
		private UInt16 paramType = 0;
		private Function module = null;

		internal Function Module
		{
			get { return module; }
		}
		private bool isStructParameter = false;

		internal void SetFunction(AxData parent)
		{
			if (deffuncIndex < 0)
				return;
			module = parent.GetUserFunction(deffuncIndex);
			if (module == null)
				return;
			if (module.IsModuleFunction)
				if (this.IsModuleType)
					this.nameFormatter = module.FunctionName;
				else
					isStructParameter = true;

		}
		
		internal bool ParamNameIsUsed
		{
			get { return paramNameIsUsed; }
			set { paramNameIsUsed = value; }
		}

		private string nameFormatter = "prm_{0}";
		internal string ParamName
		{
			//if (module != null)
			//	return module.FunctionName;
			get
			{
				if (this.isStructParameter)
				{

					StringBuilder strbd = new StringBuilder();
					strbd.Append(module.FunctionName);
					strbd.Append('_');
					strbd.Append(string.Format(nameFormatter, index));
					return strbd.ToString();
				}
				return string.Format(nameFormatter, index);
			}
		}

		internal string ToString(bool force_Named, bool remove_type, bool localToVar)
		{
			StringBuilder strbd = new StringBuilder();
			if (!remove_type)
			{
				if (paramTypeName == "NULL")
				{
					strbd.Append("/*不明な型 ");
					strbd.Append(paramType.ToString("X04"));
					strbd.Append("*/");
				}
				else if ((localToVar) && (paramTypeName.Equals("local", StringComparison.Ordinal)))
					strbd.Append("var");
				else
					strbd.Append(paramTypeName);
			}
			if ((force_Named)||(paramNameIsUsed)||(this.IsModuleType))
			{
				if (strbd.Length > 0)
					strbd.Append(' ');
				strbd.Append(string.Format(nameFormatter, index));
			}
			return strbd.ToString();
		}

		public override string ToString()
		{
			return ToString(false, false, false);
		}

		internal bool IsModuleType
		{
			get
			{
				switch (paramTypeName)
				{
					case "modvar":
					case "modinit":
					case "modterm":
					case "struct":
						return true;
				}
				return false;
			}
		}

	}
}
