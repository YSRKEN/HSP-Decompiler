using System;
using System.Collections.Generic;
using System.Text;
using KttK.HspDecompiler.Ax3ToAs.Data;
namespace KttK.HspDecompiler.Ax3ToAs.Data.Analyzer
{
	internal sealed class TokenCollection
	{
		private List<PrimitiveToken> primitives = new List<PrimitiveToken>();

		internal List<PrimitiveToken> Primitives
		{
			get { return primitives; }
		}

		private int position = 0;
		internal int Position
		{
			get
			{
				return position;
			}
			set
			{
				if (position < 0)
					throw new ArgumentOutOfRangeException();
				if (position > primitives.Count)
					throw new ArgumentOutOfRangeException();
				position = value;
			}
		}

		/// <summary>
		/// 抜け道なので出来るだけ使わないように。
		/// 変更とか絶対ダメ。
		/// </summary>
		/// <returns></returns>
		internal List<PrimitiveToken> GetPrimives()
		{
			return primitives;
		}

		internal PrimitiveToken this[int i]
		{
			get
			{
				return primitives[i];
			}
		}

		internal int Count
		{
			get
			{
				return primitives.Count;
			}
		}

		internal PrimitiveToken NextToken
		{
			get
			{
				if (this.NextIsEndOfStream)
					return null;
				return primitives[position];
			}
		}
		internal bool NextNextTokenIsGotoFunction
		{
			get
			{
				if (this.NextIsEndOfStream)
					return false;
				if ((position + 1) >= primitives.Count)
					return false;
				PrimitiveToken token = primitives[position + 1];
				return ((token.CodeExtraFlags & HspCodeExtraFlags.GotoFunction) == HspCodeExtraFlags.GotoFunction);
			}
		}


		internal TokenCollection GetLine()
		{
			if (NextIsEndOfStream)
				return null;
			List<PrimitiveToken> list = new List<PrimitiveToken>();
			list.Add(primitives[position]);
			position++;
			while (position < primitives.Count)
			{
				if (primitives[position].IsLineHead)
					break;
				list.Add(primitives[position]);
				position++;
			}
			TokenCollection ret = new TokenCollection();
			ret.primitives = list;
			return ret;

		}

		internal PrimitiveToken GetNextToken()
		{
			if (position >= primitives.Count)
				return null;
			PrimitiveToken ret = primitives[position];
			position++;
			return ret;
		}

		internal bool NextIsEndOfStream
		{
			get
			{
				return (position >= primitives.Count);
			}
		}

		internal bool NextIsEndOfLine
		{
			get
			{
				if (NextIsEndOfStream)
					return true;
				if (primitives[position].IsLineHead)
					return true;
				return false;
			}
		}

		internal bool NextIsEndOfParam
		{
			get
			{
				if (NextIsEndOfStream)
					return true;
				if (primitives[position].IsLineHead)
					return true;
				if (primitives[position].IsParamHead)
					return true;
				return false;
			}
		}

		internal bool NextIsBracketStart
		{
			get
			{
				if (NextIsEndOfStream)
					return false;
				PrimitiveToken token = primitives[position];
				if ((token.CodeExtraFlags & HspCodeExtraFlags.BracketStart) == HspCodeExtraFlags.BracketStart)
					return true;
				return false;

			}
		}

		internal bool NextIsBracketEnd
		{
			get
			{
				if (NextIsEndOfStream)
					return false;
				PrimitiveToken token = primitives[position];
				if ((token.CodeExtraFlags & HspCodeExtraFlags.BracketEnd) == HspCodeExtraFlags.BracketEnd)
					return true;
				return false;

			}
		}
		//internal bool StartOfStream
		//{
		//    get
		//    {
		//        return (position <= 0);
		//    }
		//}

		//internal void Unget()
		//{
		//    position--;
		//}

		internal void Add(PrimitiveToken token)
		{
			primitives.Add(token);
		}
	}
}
