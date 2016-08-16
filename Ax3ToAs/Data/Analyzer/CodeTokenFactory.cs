using System;
using System.Collections.Generic;
using System.Text;
using KttK.HspDecompiler.Ax3ToAs.Data.Token;
using KttK.HspDecompiler.Ax3ToAs.Data.Primitive;
using KttK.HspDecompiler.Ax3ToAs.Data;
namespace KttK.HspDecompiler.Ax3ToAs.Data.Analyzer
{
	//CodeTokenFactoryはLogicalLineFactoryの一部である。
	//下請け工場？
partial class LogicalLineFactory
{
	private static class CodeTokenFactory
	{
		internal static ExpressionToken ReadExpression(TokenCollection stream)
		{
			if (stream.NextIsEndOfLine)
				throw new HspLogicalLineException("式：読み込み中にスタックが空になった");
			ExpressionTermToken elem = null;
			List<ExpressionTermToken> elements = new List<ExpressionTermToken>();
			do//最低一つはパラメータがあるはず。
			{
				if (stream.NextIsBracketEnd)
					break;
				if (stream.NextToken is OperatorPrimitive)
					elem = (ExpressionTermToken)(ReadOperator(stream));
				else if (stream.NextToken is LiteralPrimitive)
					elem = (ExpressionTermToken)(ReadLiteral(stream));
				else if (stream.NextToken is FunctionPrimitive)
					elem = (ExpressionTermToken)(ReadFunction(stream, true));
				else if (stream.NextToken is VariablePrimitive)
					elem = (ExpressionTermToken)(ReadVariable(stream));
				else
					throw new HspLogicalLineException("式：不適切な要素を検出");
				elements.Add(elem);
			} while (!stream.NextIsEndOfParam);
			ExpressionToken ret = new ExpressionToken(elements);
			ret.RpnConvert();
			return ret;
		}

		private static object ReadLiteral(TokenCollection stream)
		{
			if (stream.NextIsEndOfLine)
				throw new HspLogicalLineException("リテラル：読み込み中にスタックが空になった");
			//リテラルプリティブ読み込み
			LiteralPrimitive token = stream.GetNextToken() as LiteralPrimitive;
			if (token == null)
				throw new HspLogicalLineException("リテラル：不適切な要素を検出");
			return new LiteralToken(token);
		}

		internal static ArgumentToken ReadArgument(TokenCollection stream)
		{
			if (stream.NextIsEndOfLine)
				throw new HspLogicalLineException("引数：読み込み中にスタックが空になった");
			bool hasBracket = stream.NextIsBracketStart;
			//括弧読み捨て。
			if (hasBracket)
				stream.GetNextToken();
			List<ExpressionToken> exps = new List<ExpressionToken>();
			bool firstArgIsNull = stream.NextIsEndOfParam;
			//一度引数を読み始めたら行末か')'まで読みきる
			while (!stream.NextIsEndOfLine)
			{
				if (hasBracket & stream.NextIsBracketEnd)
				{
					//括弧読み捨て & 引数終了
					stream.GetNextToken();
					break;
				}
				exps.Add(ReadExpression(stream));
			}
			return new ArgumentToken(exps, hasBracket, firstArgIsNull);
		}

		internal static FunctionToken ReadFunction(TokenCollection stream, bool hasBracket)
		{
			if (stream.NextIsEndOfStream)
				throw new HspLogicalLineException("関数：読み込み中にスタックが空になった");
			FunctionPrimitive token = stream.GetNextToken() as FunctionPrimitive;
			if (token == null)
				throw new HspLogicalLineException("関数：関数プリミティブ以外からスタート");
			//行末なら強制的に終了
			if (stream.NextIsEndOfLine)
				return new FunctionToken(token);
			//ゴーストラベル処理
			if (token.HasGhostLabel	&& (stream.NextToken.CodeType == HspCodeType.Label))
			{
				stream.GetNextToken();
				if (stream.NextIsEndOfLine)
					return new FunctionToken(token);
			}

			//次が'('なら引数読み込み
			if (stream.NextIsBracketStart)
				return new FunctionToken(token, ReadArgument(stream));
			//どちらでもないなら、hasBracketで分岐
			//hasBracket　=falseなら強制的に引数ありに。
			if (hasBracket)
				return new FunctionToken(token);
			else
				return new FunctionToken(token, ReadArgument(stream));
		}

		internal static VariableToken ReadVariable(TokenCollection stream)
		{
			if (stream.NextIsEndOfStream)
				throw new HspLogicalLineException("変数：読み込み中にスタックが空になった");
			//変数プリティブ読み込み
			VariablePrimitive token = stream.GetNextToken() as VariablePrimitive;
			if (token == null)
				throw new HspLogicalLineException("変数：変数プリミティブ以外からスタート");

			//次が括弧ならば配列変数
			if (stream.NextIsBracketStart){
				return new VariableToken(token, ReadArgument(stream));
			}
			//そうでなければ普通の変数
			return new VariableToken(token);
			//if (parser.NextIsEndOfLine)
			//    return VariableToken(token);
		}

		internal static OperatorToken ReadOperator(TokenCollection stream)
		{
			if (stream.NextIsEndOfLine)
				throw new HspLogicalLineException("演算子：読み込み中にスタックが空になった");
			//変数プリティブ読み込み
			OperatorPrimitive token = stream.GetNextToken() as OperatorPrimitive;
			if (token == null)
				throw new HspLogicalLineException("演算子：不適切な要素を検出");
			return new OperatorToken(token);

		}
	}
}

}
