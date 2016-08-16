using System;
using System.Text.RegularExpressions;
using System.Diagnostics;
using System.Windows.Forms;
using System.Text;


namespace KttK.HspDecompiler.Ax2ToAs.Data
{
	/// <summary>
	/// Token の概要の説明です。
	/// </summary>
internal class Token
{
	private Token()
	{
		// 
		// TODO: コンストラクタ ロジックをここに追加してください。
		//
	}
	private Token(int offset)
	{
	}
	
	private static AxData data = null;
	internal static AxData CurrentData
	{
		set
		{
			data = value;
		}
	}
	
	private int fType;
	private int fValue;
	private int id;
	private int size;
	internal bool isLineend;
	internal int IfJumpTo = -1;
	#region static
	private static int nextOffset;
	internal static int Index;
	private static Token nextToken;
	internal static void SetZero()
	{
		Index = 0;
		nextOffset = 0;
		nextToken = GetToken(0);
		nextOffset += nextToken.size;
	}

	internal static Token GetNext()
	{

		Token ret = nextToken;

		nextToken = GetToken(nextOffset);
		if (ret == null)
		{
			Index = nextOffset /2;
			return null;
		}
		Index = ret.id;
		if ((nextToken == null) || (nextToken.isLinehead))
			ret.isLineend = true;
		if (nextToken != null)
		{
			nextOffset += nextToken.size;
			if (ret.NextIsUnenableLabel)
			{
				ret.isLineend = false;
				Token UnenableToken = GetNext();//無効ラベル
				if ((nextToken == null) || (nextToken.isLinehead))
					ret.isLineend = true;
				return ret;
			}
		}
		return ret;
	}

	private static Token GetToken(int offset)
	{
		if ((offset<0)||((offset+1) >= data.TokenData.Length))
			return null;
		Token ret = new Token();
		ret.id = offset / 2;
		ret.fValue = data.TokenData[offset];offset++;
		ret.fType = data.TokenData[offset];offset++;
		ret.size = 2;
		if ((ret.fType & 0x01) != 0)
		{
			ret.fType ^= 0x01;
			ret.fValue += 256;
		}
		if ( (ret.Type == 0x58) && ((ret.Value == 0) || (ret.Value == 1)) )
		{
			ret.size += 2;
			ret.IfJumpTo = BitConverter.ToInt16(data.TokenData, offset); offset += 2;
			ret.IfJumpTo += ret.id + 2;
		}
		if ( ((ret.fType&0x80) != 0) ) //&&(ret.fValue==0) )
		{
			ret.fType ^= 0x80;
			ret.size += 4;
			ret.fValue = BitConverter.ToInt32(data.TokenData, offset); offset+= 4;
		}
		return ret;
	}

	internal static int Percent
	{
		get{
			double ret = ((double)nextOffset * 100.0) / data.TokenData.Length;
			return (int)ret;
		}
	}

	#endregion
	#region propaty
	private int Type
	{
		get
		{
			return fType&0x78;
		}
	}

	private int Value
	{
		get
		{
			return fValue;
		}
	}

	private bool NextIsUnenableLabel
	{
		get
		{
			if ((Type == 0x40) && ((fValue == 0x03) || (fValue == 0x11) || (fValue == 0x2b)) )
				return true;
			return false;
		}
	}

	internal bool isLinehead//このトークンは行頭にくるべきであり、直前に"\n"を入れる必要がある。
	{
		get
		{
			return (fType&2)!=0;
		}
	}


	internal bool isArg//このトークンは引数のひとつであり、直前に','を入れる必要がある。
	{
		get
		{
			return (fType&4)!=0;
		}
	}

	internal bool TabPlus
	{
		get
		{
			if (Type == 0x40)
			{
				if (Value == 0x11)//repeat
					return true;
			}
			if (Type == 0x58)
			{
				if ((Value == 0) || (Value == 1))//if else
					return true;
			}
			return false;
		}
	}

	internal bool TabMinus
	{
		get
		{
			if (Type == 0x40)
			{
				if (Value == 0x12)//loop
					return true;
			}
			return false;
		}
	}


	internal int Id
	{
		get
		{
			return id;
		}
	}

	internal int IfJumpId
	{
		get
		{
			if ( (Type == 0x58) &&(Value == 1) )
			{
				return id + 2;
			}
			return id;
		}
	}

	internal bool isEmpty	
	{
		get
		{
			return (fType==0)&&(fValue==0);
		}
	}

	internal int LabelIndex
	{
		get
		{
			if (isLinehead)
				return -1;
			if ( Type == 0x18 ) 
				return Value;
			return -1;
		}
	}
	#endregion
	#region propaty for decompile
	internal string GetString()
	{
		int type = Type;
		string ret = null;
		switch(Type)
		{
			case 0x00://token_value=文字コード
			switch(Value)
			{
				case 0x61:
					return "<=";
				case 0x62:
					return ">=";
				case 0x63:
					return "<<";
				case 0x64:
					return ">>";
				default:
					Encoding encode = Encoding.GetEncoding("SHIFT-JIS");
					byte[] bytes = new byte[1];
					bytes[0] = (byte)Value;
					return encode.GetString(bytes);
			}
			case 0x08://token_value=整数値
				return Value.ToString();
			case 0x10://token_value=文字列を格納しているアドレス
				ret = data.GetString(Value);
				return "\"" + Escape(ret) + "\"";
			case 0x18://token_value=ラベルナンバー
				return "label_"+Value.ToString();
			case 0x20://token_value=変数ナンバー
				return "var_"+Value.ToString();
			case 0x38://hsp標準命令
				ret = GetStdFunc1Name(Value);
				if (ret!= null)
					return ret;
				break;
			case 0x40://hsp標準命令２
				ret = GetStdFunc2Name(Value);
				if (ret!= null)
					return ret;
				break;
			case 0x48://hsp標準命令３
				ret = GetStdFunc3Name(Value);
				if (ret!= null)
					return ret;
				break;
			case 0x50://func関数
				ret = data.GetFuncName(Value - 0x10);
				if (ret != null)
					return "func_" + ret;
				break;
			case 0x58://if系命令
				if (Value == 0)
					return "if";
				if (Value == 1)
					return "else";
				break;
			case 0x60://deffunc関数
				ret = data.GetDeffuncName(Value - 0x10);
				if (ret != null)
					return ret;
				break;
			case 0x78://end
				if (Value == 0)
					return "end";
				break;
		}
		return Type.ToString("x2") + Value.ToString("x2");
	}

	internal bool isKnown
	{
		get
		{
			int type = Type;
			string ret = null;
			switch(Type)
			{
				case 0x00://token_value=文字コード
					return true;
				case 0x08://token_value=整数値
					return true;
				case 0x10://token_value=文字列を格納しているアドレス
					return true;
				case 0x18://token_value=ラベルナンバー
					return true;
				case 0x20://token_value=変数ナンバー
					return true;
				case 0x38://hsp標準命令
					ret = GetStdFunc1Name(Value);
					if (ret!= null)
						return true;
					break;
				case 0x40://hsp標準命令２
					ret = GetStdFunc2Name(Value);
					if (ret!= null)
						return true;
					break;
				case 0x48://hsp標準命令３
					ret = GetStdFunc3Name(Value);
					if (ret!= null)
						return true;
					break;
				case 0x50://func関数
					ret = data.GetFuncName(Value - 0x10);
					if (ret != null)
						return true;
					break;
				case 0x58://if系命令
					if (Value == 0)
						return true;
					if (Value == 1)
						return true;
					break;
				case 0x60://deffunc関数
					ret = data.GetDeffuncName(Value - 0x10);
					if (ret != null)
						return true;
					break;
				case 0x78://end
					if (Value == 0)
						return true;
					break;
			}
			return false;
		}
	}

	private readonly char[] escapeWord = new char[] {'\n', '\r', '\t', '\"', '\\'};
	private string Escape(string str)
	{

		if (str == null)
			return null;
		if (str.Length == 0)
			return str;
		int i;
		if ((i=str.IndexOfAny(escapeWord)) >= 0)
		{
			char spliter = str[i];
			string mid;
			switch(spliter)
			{
				case '\n':
					mid = @"\n";
					break;
				case '\r':
					mid = "";//@"\r";\nが\r\nに翻訳される不具合の修正。HSPでは"\n"⇒"\r\n"にするようだ
					break;
				case '\t':
					mid = @"\t";
					break;
				case '\"':
					mid = @"\""";
					break;
				case '\\':
					mid = @"\\";
					break;
				default:
					Debug.Assert(false);
					mid = "";
					break;
			}
			string[] ret = str.Split(escapeWord, 2);
			return ret[0] + mid + Escape(ret[1]);
		}
		return str;
		
	}
	#region HSPfuncname
	private string GetStdFunc1Name(int v)//0x38系命令
	{
		switch(v)
		{
			case 0x00: return "system";
			case 0x01: return "hspstat";
			case 0x02: return "hspver";
			case 0x03: return "cnt";
			case 0x04: return "err";
			case 0x05: return "strsize";
			case 0x06: return "looplev";
			case 0x07: return "sublev";
			case 0x40: return "mousex";
			case 0x41: return "mousey";
			case 0x42: return "csrx";
			case 0x43: return "csry";
			case 0x44: return "paluse";
			case 0x45: return "dispx";
			case 0x46: return "dispy";
			case 0x47: return "rval";
			case 0x48: return "gval";
			case 0x49: return "bval";
			case 0x4a: return "stat";
			case 0x4b: return "winx";
			case 0x4c: return "winy";
			case 0x4d: return "prmx";
			case 0x4e: return "prmy";
			case 0x4f: return "iparam";
			case 0x50: return "wparam";
			case 0x51: return "lparam";
			case 0x60: return "cmdline";
			case 0x61: return "windir";
			case 0x62: return "curdir";
			case 0x63: return "refstr";
			case 0x64: return "exedir";





		}
		return null;
	}

	private string GetStdFunc2Name(int v)//0x40系命令
	{
		switch(v)
		{
			case 0x00: return "goto";
			case 0x01: return "gosub";
			case 0x02: return "return";
			case 0x03: return "break";
			case 0x04: return "onexit";
			case 0x05: return "onkey";
			case 0x06: return "onclick";
			case 0x08: return "onerror";
			case 0x09: return "on";
			case 0x0a: return "exgoto";
			case 0x10: return "wait";
			case 0x11: return "repeat";
			case 0x12: return "loop";
			case 0x13: return "mes";
			case 0x14: return "dim";
			case 0x15: return "sdim";
			case 0x16: return "alloc";
			case 0x17: return "bload";
			case 0x18: return "bsave";
			case 0x19: return "bcopy";
			case 0x1a: return "stop";
			case 0x1b: return "run";
			case 0x1c: return "rnd";
			case 0x1d: return "str";
			case 0x1e: return "int";
			case 0x1f: return "skiperr";
			case 0x20: return "dup";
			case 0x21: return "await";
			case 0x22: return "poke";
			case 0x23: return "peek";
			case 0x24: return "wpoke";
			case 0x25: return "wpeek";
			case 0x26: return "strlen";
			case 0x27: return "getstr";
			case 0x28: return "exist";
			case 0x29: return "strmid";
			case 0x2a: return "instr";
			case 0x2b: return "continue";
			case 0x2c: return "mref";
			case 0x2d: return "logmode";
			case 0x2e: return "logmes";
			case 0x2f: return "memcpy";
			case 0x30: return "memset";
			case 0x31: return "notesel";
			case 0x32: return "noteadd";
			case 0x33: return "noteget";
			case 0x34: return "notemax";
			case 0x35: return "notedel";
			case 0x36: return "noteload";
			case 0x37: return "notesave";
			case 0x38: return "memfile";


		}
		return null;
	}

	private string GetStdFunc3Name(int v)//0x48系命令
	{
		switch(v)
		{
			case 0x00: return "button";
			case 0x10: return "title";
			case 0x11: return "pos";
			case 0x13: return "cls";
			case 0x14: return "font";
			case 0x15: return "sysfont";
			case 0x16: return "objsize";
			case 0x17: return "picload";
			case 0x18: return "color";
			case 0x19: return "palcolor";
			case 0x1a: return "palette";
			case 0x1b: return "redraw";
			case 0x1c: return "width";
			case 0x1d: return "gsel";
			case 0x1e: return "gcopy";
			case 0x1f: return "gzoom";
			case 0x20: return "gmode";
			case 0x21: return "bmpsave";
			case 0x22: return "text";
			case 0x23: return "getkey";
			case 0x24: return "sndload";
			case 0x25: return "snd";
			case 0x26: return "mci";
			case 0x27: return "input";
			case 0x28: return "mesbox";
			case 0x29: return "buffer";
			case 0x2a: return "screen";
			case 0x2b: return "bgscr";
			case 0x2c: return "dialog";
			case 0x2d: return "chgdisp";
			case 0x2e: return "exec";
			case 0x2f: return "mkdir";
			case 0x30: return "sndoff";
			case 0x31: return "boxf";
			case 0x32: return "pget";
			case 0x33: return "pset";
			case 0x34: return "palfade";
			case 0x35: return "getpal";
			case 0x36: return "gettime";
			case 0x37: return "palcopy";
			case 0x38: return "randomize";
			case 0x39: return "clrobj";
			case 0x3a: return "chkbox";
			case 0x3b: return "line";
			case 0x3c: return "stick";
			case 0x3d: return "ginfo";
			case 0x3e: return "combox";
			case 0x3f: return "chdir";
			case 0x40: return "objprm";
			case 0x41: return "objsend";
			case 0x42: return "objmode";
			case 0x43: return "sysinfo";
			case 0x44: return "getpath";
			case 0x48: return "mouse";
			case 0x49: return "dirlist";
			case 0x4a: return "delete";
			case 0x4b: return "listbox";
			case 0x4c: return "objsel";
			case 0x4d: return "ll_ret";
			case 0x4e: return "ll_retset";
			case 0x4f: return "ll_getptr";
			case 0x50: return "ll_peek";
			case 0x51: return "ll_peek1";
			case 0x52: return "ll_peek2";
			case 0x53: return "ll_peek4";
			case 0x54: return "ll_poke";
			case 0x55: return "ll_callfunc";
			case 0x56: return "ll_n";
			case 0x57: return "ll_poke1";
			case 0x58: return "ll_poke2";
			case 0x59: return "ll_poke4";
			case 0x5a: return "ll_libfree";
			case 0x5b: return "ll_callfnv";
			case 0x5c: return "ll_call";
			case 0x5d: return "ll_free";
			case 0x5e: return "ll_s";
			case 0x5f: return "ll_p";
			case 0x60: return "ll_str";
			case 0x61: return "ll_dll";
			case 0x62: return "ll_func";
			case 0x63: return "ll_type";
			case 0x64: return "ll_z";
			case 0x65: return "ll_libload";
			case 0x66: return "ll_getproc";
			case 0x67: return "ll_bin";
		}
		return null;
	}
	#endregion
	#endregion
}

}
