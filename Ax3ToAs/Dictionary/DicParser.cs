using System;
using System.Collections.Generic;
using System.Text;

namespace KttK.HspDecompiler.Ax3ToAs.Dictionary
{
	static class DicParser
	{
		internal static int StringToInt32(string str, int defaultValue)
		{
			try
			{
				str = str.Trim();
				if (str.StartsWith("0x"))
				{
					str = str.Substring(2);
					return Int32.Parse(str, System.Globalization.NumberStyles.HexNumber);
				}
				return Int32.Parse(str);
			}
			catch (Exception)
			{
				return defaultValue;
			}
		}

		internal static int StringToInt32(string str)
		{
			return StringToInt32(str, 0);
		}

		internal static double StringToDouble(string str, double defaultValue)
		{
			try
			{
				return Double.Parse(str);
			}
			catch (Exception)
			{
				return defaultValue;
			}
		}

		internal static double StringToDouble(string str)
		{
			return StringToDouble(str, 0.0);
		}

		internal static Object StringToEnum(Type enumType, string str, int defaultValue)
		{
			try
			{
				return Enum.Parse(enumType, str);
			}
			catch (Exception)
			{
				try
				{
					return Int32.Parse(str);
				}
				catch (Exception)
				{
					return defaultValue;
				}
			}
		}

		internal static object StringToEnum(Type enumType, string str)
		{
			return StringToEnum(enumType, str, 0);
		}

	}
}
