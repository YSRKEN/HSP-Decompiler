using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using KttK.HspDecompiler.Ax3ToAs.Dictionary;

namespace KttK.HspDecompiler.Ax3ToAs.Data
{
	internal class PrimitiveTokenDataSet
	{
		internal AxData Parent;
		internal int TokenOffset;
		internal int Type;
		internal int Flag;
		internal int Value;
		internal string Name;
		internal HspDictionaryValue DicValue;
	}

	internal abstract class PrimitiveToken
	{
		protected PrimitiveToken() { }
		internal PrimitiveToken(PrimitiveTokenDataSet dataSet)
		{
			parent = dataSet.Parent;
			codeType = dataSet.DicValue.Type;
			codeExtraFlags = dataSet.DicValue.Extra;
			dicValueName = dataSet.DicValue.Name;
			oparatorPriority = dataSet.DicValue.OparatorPriority;
			tokenOffset = dataSet.TokenOffset;
			type = dataSet.Type;
			flag = dataSet.Flag;
			value = dataSet.Value;
			name = dataSet.Name;
		}

		protected string dicValueName = "null";
		protected readonly int type;
		protected readonly HspCodeType codeType;

		protected readonly int flag;
		protected readonly int value;

		internal int Value
		{
			get { return value; }
		} 



		private readonly HspCodeExtraFlags codeExtraFlags;
		private readonly AxData parent = null;
		private int oparatorPriority;
		private string name;
		int tokenOffset;

		internal bool HasGhostLabel
		{
			get
			{
				if (!this.IsLineHead)
					return false;
				if ((codeExtraFlags & HspCodeExtraFlags.HasGhostLabel) == HspCodeExtraFlags.HasGhostLabel)
					return true;
				return false;
			}
		}
		internal HspCodeType CodeType
		{
			get { return codeType; }
		}
		internal HspCodeExtraFlags CodeExtraFlags
		{
			get { return codeExtraFlags; }
		}
		internal int OperatorPriority
		{
			get
			{
				if (codeType != HspCodeType.Operator)
					throw new InvalidOperationException("演算子でないプリミティブに優先度が要求されました");
				return oparatorPriority;
			}
		}
		internal bool HasLongTypeValue
		{
			get
			{
				return ((flag & 0x80) == 0x80);
			}
		}
		internal bool IsParamHead
		{
			get
			{
				return ((flag & 0x40) == 0x40);
			}
		}
		internal bool IsLineHead
		{
			get
			{
				return ((flag & 0x20) == 0x20);
			}
		}

		internal string Name
		{
			get{ return name; }
		}

		internal int TokenOffset
		{
			get { return tokenOffset; }
		}

		internal void SetName()
		{
			switch (this.codeType)
			{
				case HspCodeType.Label:
					
					name = dicValueName + value.ToString();
					return;

				case HspCodeType.Integer:
				case HspCodeType.Param:
				case HspCodeType.Variable:
					name = dicValueName + value.ToString();
					return;
				case HspCodeType.UserFunction:
				case HspCodeType.DllFunction:
					//defaultName = "not supported";
					//break;
				case HspCodeType.NONE:
				default:
					break;
			}
		}

		public override string ToString()
		{
			return name;
		}

		internal virtual string DefaultName
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
					builder.Append(value.ToString("X08"));
				else
					builder.Append(value.ToString("X04"));
				builder.Append("*/");
				return builder.ToString();
			}
		}


	}
}
