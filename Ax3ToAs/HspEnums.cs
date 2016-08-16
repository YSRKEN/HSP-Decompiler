using System;
using System.Collections.Generic;
using System.Text;

namespace KttK.HspDecompiler.Ax3ToAs
{
	public enum HspCodeType
	{
		NONE = 0x00,
		Operator = 0xFF,
		Symbol = 0xFE,
		Variable = 0x01,
		String = 0x02,
		Double = 0x03,
		Integer = 0x04,
		Param = 0x05,
		Label = 0x07,
		HspFunction = 0x08,
		IfStatement = 0x0B,
		UserFunction = 0x0C,
		DllFunction = 0x10,
		ComFunction = 0x11,
		PlugInFunction = 0x12,


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
