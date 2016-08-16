using System;

namespace KttK.HspDecompiler.Ax2ToAs.Data
{
	/// <summary>
	/// Header の概要の説明です。
	/// </summary>
internal class Header
{
	private Header()
	{
		// 
		// TODO: コンストラクタ ロジックをここに追加してください。
		//
	}

	#region var
	private int allDataByte;
	private int scriptOffset;
	private int scriptByte;
	private int textOffset;
	private int textByte;
	private int labelOffset;
	private int labelByte;
	private int dllOffset;
	private int dllByte;
	private int funcOffset;
	private int funcByte;
	private int deffuncOffset;
	private int deffuncByte;
	private int moduleOffset;
	private int moduleByte;
	#endregion

	#region propaty
	internal int AllDataByte
	{
		get
		{
			return allDataByte;
		}
	}

	internal int ScriptOffset
	{
		get
		{
			return scriptOffset;
		}
	}

	internal int ScriptByte
	{
		get
		{
			return scriptByte;
		}
	}

	internal int TextOffset
	{
		get
		{
			return textOffset;
		}
	}

	internal int TextByte
	{
		get
		{
			return textByte;
		}
	}

	internal int LabelOffset
	{
		get
		{
			return labelOffset;
		}
	}

	internal int LabelByte
	{
		get
		{
			return labelByte;
		}
	}

	internal int DllOffset
	{
		get
		{
			return dllOffset;
		}
	}

	internal int DllByte
	{
		get
		{
			return dllByte;
		}
	}

	internal int FuncOffset
	{
		get
		{
			return funcOffset;
		}
	}

	internal int FuncByte
	{
		get
		{
			return funcByte;
		}
	}

	internal int DeffuncOffset
	{
		get
		{
			return deffuncOffset;
		}
	}

	internal int DeffuncByte
	{
		get
		{
			return deffuncByte;
		}
	}

	internal int ModuleOffset
	{
		get
		{
			return moduleOffset;
		}
	}

	internal int ModuleByte
	{
		get
		{
			return moduleByte;
		}
	}




	internal int ScriptCount
	{
		get
		{
			return scriptByte / 2;
		}
	}

	internal int ScriptEndOffset
	{
		get
		{
			return scriptOffset + scriptByte;
		}
	}

	internal int LabelCount
	{
		get
		{
			return labelByte/4;
		}
	}

	internal int DllCount
	{
		get
		{
			return dllByte/24;
		}
	}

	internal int FuncCount
	{
		get
		{
			return funcByte/16;
		}
	}

	internal int DeffuncCount
	{
		get
		{
			return deffuncByte/16;
		}
	}

	internal int ModuleCount
	{
		get
		{
			return moduleByte/24;
		}
	}

	#endregion
	internal static Header FromIntArray(int[] data)
	{
		if (data == null)
			return null;
		if (data.Length < 20)
			return null;
		Header ret = new Header();
		ret.allDataByte = data[3];
		ret.scriptOffset = data[4];
		ret.scriptByte = data[5];
		ret.textOffset = data[6];
		ret.textByte = data[7];
		ret.labelOffset = data[8];
		ret.labelByte = data[9];

		ret.dllOffset = data[12];
		ret.dllByte = data[13];
		ret.funcOffset = data[14];
		ret.funcByte = data[15];
		ret.deffuncOffset = data[16];
		ret.deffuncByte = data[17];
		ret.moduleOffset = data[18];
		ret.moduleByte = data[19];
		return ret;
	}

}
}
