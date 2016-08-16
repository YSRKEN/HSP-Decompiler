using System;
using System.Collections.Generic;
using System.Text;

namespace KttK.HspDecompiler.Ax3ToAs
{
	public enum HspCodeType
	{
		NONE = 0x00,//    #define TYPE_MARK 0
		Operator = 0xFF,//    #define TYPE_MARK 0
		Symbol = 0xFE,//    #define TYPE_MARK 0
		Variable = 0x01,//#define TYPE_VAR 1
		String = 0x02,//#define TYPE_STRING 2
		Double = 0x03,//#define TYPE_DNUM 3
		Integer = 0x04,//#define TYPE_INUM 4
		Param = 0x05,//#define TYPE_STRUCT 5
		//#define TYPE_XLABEL 6
		Label = 0x07,//#define TYPE_LABEL 7
		HspFunction = 0x08,//#define TYPE_INTCMD 8
		//#define TYPE_EXTCMD 9
		//#define TYPE_EXTSYSVAR 10

		IfStatement = 0x0B,//#define TYPE_CMPCMD 11
		UserFunction = 0x0C,//#define TYPE_MODCMD 12
		//#define TYPE_INTFUNC 13
		//#define TYPE_SYSVAR 14
		//#define TYPE_PROGCMD 15
		DllFunction = 0x10,//#define TYPE_DLLFUNC 16
		ComFunction = 0x11,//#define TYPE_DLLCTRL 17
		PlugInFunction = 0x12,//#define TYPE_USERDEF 18


		OnEventStatement = 0x20,
		OnStatement = 0x21,
		ElseStatement = 0x22,
		McallStatement = 0x23,
	}

	[Flags]
	public enum HspCodeExtraFlags
	{
		NONE = 0x00,
		HasExtraInt16 = 0x0001,
		HasGhostLabel = 0x0002,
		HasGhostGoto  = 0x0004,
		AddTab        = 0x0008,
		RemoveTab     = 0x0010,
		IsGhost       = 0x0020,
		BracketStart  = 0x0040,
		BracketEnd	  = 0x0080,
		GotoFunction  = 0x0100,

	}


}
