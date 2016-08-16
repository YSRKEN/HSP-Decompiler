using System;
using System.Collections.Generic;
using System.Text;
using KttK.HspDecompiler.Ax3ToAs.Data;
using KttK.HspDecompiler.Ax3ToAs.Data.Primitive;
namespace KttK.HspDecompiler.Ax3ToAs.Data.Token
{
	/// <summary>
	/// Ž®
	/// </summary>
	internal sealed class ExpressionToken : CodeToken
	{
		private ExpressionToken() { }
		internal ExpressionToken(List<ExpressionTermToken> elements)
		{
			tokens = elements;
		}

		private readonly List<ExpressionTermToken> tokens = null;
		ExpressionTermToken convertedToken = null;
		private bool tryConvert = false;
		internal bool CanRpnConvert
		{
			get
			{
				if (convertedToken != null)
					return true;
				if (!tryConvert)
					return RpnConvert();
				return false;
			}
		}

		internal bool RpnConvert()
		{
			if (convertedToken != null)
				return true;
			if (tokens.Count == 0)
				return false;
			tryConvert = true;
			if (tokens.Count == 1)
			{
				convertedToken = tokens[0];
				return true;
			}
			List<ExpressionTermToken> stack = new List<ExpressionTermToken>();
			List<ExpressionTermToken> source = new List<ExpressionTermToken>();
			try
			{
				source.AddRange(tokens);
				while (source.Count != 0)
				{
					ExpressionTermToken token = source[0];
					source.RemoveAt(0);
					if (token.IsOperator)
					{
						OperandToken right = (OperandToken)stack[stack.Count - 1];
						stack.RemoveAt(stack.Count - 1);
						OperandToken left = (OperandToken)stack[stack.Count - 1];
						stack.RemoveAt(stack.Count - 1);
						stack.Add((ExpressionTermToken)(new SubExpressionToken(left, right, (OperatorToken)token)));
					}
					else
					{
						stack.Add(token);
					}
				}
			}
			catch (Exception)
			{
				return false;
			}
			if (stack.Count != 1)
				return false;
			convertedToken = stack[0];
			return true;
		}

		internal override int TokenOffset
		{
			get
			{
				if ((tokens == null) || (tokens.Count == 0))
					return -1;
				CodeToken token = tokens[0] as CodeToken;
				if (token == null)
					return -1;
				return token.TokenOffset;
			}
		}

		internal string ToString(bool getRpnConverted)
		{
			if (getRpnConverted && (convertedToken != null))
				return convertedToken.ToString();
			StringBuilder builder = new StringBuilder();
			int i = 0;
			foreach (ExpressionTermToken token in tokens)
			{
				if (i != 0)
					builder.Append(' ');
				builder.Append(token.ToString());
				i++;
			}
			return builder.ToString();
		}

		public override string ToString()
		{
			return this.ToString(true);
		}


		internal override void CheckLabel()
		{
			foreach (CodeToken token in tokens)
				token.CheckLabel();
		}

		internal override bool CheckRpn()
		{
			return this.CanRpnConvert;
		}
	}
}
