using System;
using System.Collections.Generic;
using System.Text;
using KttK.HspDecompiler.Ax3ToAs.Data.Line;
using KttK.HspDecompiler.Ax3ToAs.Data.Token;
using KttK.HspDecompiler.Ax3ToAs.Data.Primitive;
using KttK.HspDecompiler.Ax3ToAs.Data;
namespace KttK.HspDecompiler.Ax3ToAs.Data.Analyzer
{

	partial class LogicalLineFactory
	{
		//外から見えるのはここだけ
		/// <summary>
		/// 一行分のTokenCollectionからLogicalLineを作成する
		/// </summary>
		/// <param defaultName="parser"></param>
		/// <returns></returns>
		internal static LogicalLine GetCodeToken(TokenCollection stream)
		{
			if (stream == null)
				return null;
			if (stream.Count == 0)
				return null;
			if (stream.NextIsEndOfStream)
				return null;
			LogicalLine line = null;
			try
			{
				if (stream.NextToken is IfStatementPrimitive) //if, else行
					return (LogicalLine)readIf(stream);
				if (stream.NextToken is McallFunctionPrimitive) //on行
					return (LogicalLine)readMcall(stream);
				if (stream.NextToken is OnEventFunctionPrimitive)
				{ //on###行
					if(stream.NextNextTokenIsGotoFunction)//goto/gosubがないなら
						return (LogicalLine)readOnEvent(stream);
					else
						return (LogicalLine)readCommand(stream);
				}
				if (stream.NextToken is OnFunctionPrimitive) //on行
					return (LogicalLine)readOn(stream);
				if (stream.NextToken is FunctionPrimitive)//その他の関数
					return (LogicalLine)readCommand(stream);
				if (stream.NextToken is VariablePrimitive)//代入行
					return (LogicalLine)readAssignment(stream);
			}
			//ここでHspLogicalLineExceptionをcatchする。他のところでは行ってはならない
			catch (HspLogicalLineException e)
			{

				line = new UnknownLine(stream.Primitives);
				line.AddError(e.Message);
				return line;
			}
			line = new UnknownLine(stream.Primitives);
			line.AddError("？行：先頭の単語が解釈できない");
			return line;
		}

		private static LogicalLine readMcall(TokenCollection stream)
		{
			int start = stream.Position;
			McallFunctionPrimitive mcall = stream.GetNextToken() as McallFunctionPrimitive;
			if (mcall == null)
				throw new HspLogicalLineException("mcall：mcallプリミティブ以外からスタート");
			if (stream.NextIsEndOfLine)
			{
				stream.Position = start;
				return (LogicalLine)readCommand(stream);
			}
			ExpressionToken exp = CodeTokenFactory.ReadExpression(stream);
			if (exp.CanRpnConvert)//RPN変換ができるなら普通の関数として扱う。
			{
				stream.Position = start;
				return (LogicalLine)readCommand(stream);
			}

			stream.Position = start;
			stream.GetNextToken();
			VariablePrimitive var = stream.GetNextToken() as VariablePrimitive;
			if (var == null)
				throw new HspLogicalLineException("mcall行：変換不能な形式");
			if (stream.NextIsBracketStart)//mcall の記法は配列変数を認めない
				throw new HspLogicalLineException("mcall行：変換不能な形式");
			if (stream.NextIsEndOfLine)
				throw new HspLogicalLineException("mcall行：変換不能な形式");
			exp = CodeTokenFactory.ReadExpression(stream);
			if (stream.NextIsEndOfLine)
				return new McallStatement(mcall, var, exp, null);
			ArgumentToken arg = CodeTokenFactory.ReadArgument(stream);
			if (stream.NextIsEndOfLine)
				return new McallStatement(mcall, var, exp, arg);
			throw new HspLogicalLineException("mcall行：余分なトークンがある");
		}

		private static OnStatement readOn(TokenCollection stream)
		{
			OnFunctionPrimitive token = stream.GetNextToken() as OnFunctionPrimitive;
			if (token == null)
				throw new HspLogicalLineException("on条件分岐行：条件分岐プリミティブ以外からスタート");
			//式がないこともあるかもしれない(実行時エラー)
			if (stream.NextIsEndOfLine)
				return new OnStatement(token,null, null);
			//式を読む。goto/gosubがないこともあるかもしれない(実行時エラー)
			ExpressionToken exp = CodeTokenFactory.ReadExpression(stream);
			if (stream.NextIsEndOfLine)
				return new OnStatement(token, exp,null);
			//goto/gosub関数を読む。goto/gosub以外でもコンパイルは通る(実行時エラー)
			//この関数には()がつかない。
			FunctionToken func = CodeTokenFactory.ReadFunction(stream, false);
			if (stream.NextIsEndOfLine)
				return new OnStatement(token, exp, func);
			//まだあまってたらエラーね。
			throw new HspLogicalLineException("on条件分岐行：余分なトークンがある");

		}

		private static OnEventStatement readOnEvent(TokenCollection stream)
		{
			OnEventFunctionPrimitive token = stream.GetNextToken() as OnEventFunctionPrimitive;
			if (token == null)
				throw new HspLogicalLineException("on条件分岐行：条件分岐プリミティブ以外からスタート");
			//goto/gosubがないこともあるかもしれない(実行時エラー)
			if (stream.NextIsEndOfLine)
				return new OnEventStatement(token, null);
			//goto/gosub関数を読む。goto/gosub以外でもコンパイルは通る(実行時エラー)
			//この関数には()がつかない。
			FunctionToken func = CodeTokenFactory.ReadFunction(stream, false);
			if (stream.NextIsEndOfLine)
				return new OnEventStatement(token, func);
			//まだあまってたらエラーね。
			throw new HspLogicalLineException("on条件分岐行：余分なトークンがある");
		}

		/// <summary>
		/// 代入行
		/// </summary>
		/// <param defaultName="primitives"></param>
		/// <returns></returns>
		private static Assignment readAssignment(TokenCollection stream)
		{
			//変数を読む。失敗したら代入行じゃない。
			VariableToken token = CodeTokenFactory.ReadVariable(stream);
			//演算子を読む。失敗するなら代入行じゃない。
			if (stream.NextIsEndOfLine)
				throw new HspLogicalLineException("代入行：演算子なし");
			OperatorToken op = CodeTokenFactory.ReadOperator(stream);
			//式が続かないこともある。"x++"とか
			if (stream.NextIsEndOfLine)
				return new Assignment(token, op);
			else
			{
				//引数を読む。あまりが出なければOK
				ArgumentToken arg = CodeTokenFactory.ReadArgument(stream);
				if (stream.NextIsEndOfLine)
					return new Assignment(token, op, arg);
			}
			throw new HspLogicalLineException("代入行：余分なトークンがある");

		}

		/// <summary>
		/// If,elseステートメントの開始
		/// </summary>
		/// <param defaultName="primitives"></param>
		/// <returns></returns>
		private static IfStatement readIf(TokenCollection stream)
		{
			IfStatementPrimitive token = stream.GetNextToken() as IfStatementPrimitive;
			if (token == null)
				throw new HspLogicalLineException("条件分岐行：条件分岐プリミティブ以外からスタート");

			//elseには式がない。
			if (stream.NextIsEndOfLine)
				return new IfStatement(token);
			else
			{
				//式を読む。あまりが出なければOK
				ArgumentToken arg = CodeTokenFactory.ReadArgument(stream);
				if (stream.NextIsEndOfLine)
					return new IfStatement(token, arg);
			}
			throw new HspLogicalLineException("条件分岐行：余分なトークンがある");
		}

		/// <summary>
		/// 命令行
		/// </summary>
		/// <param defaultName="primitives"></param>
		/// <returns></returns>
		private static Command readCommand(TokenCollection stream)
		{
			//実態はCodeTokenFactoryに任せる
			//命令には（関数と違って）括弧はいらない
			FunctionToken func = CodeTokenFactory.ReadFunction(stream, false);
			if (stream.NextIsEndOfLine)
				return new Command(func);
			throw new HspLogicalLineException("命令行：余分なトークンがある");
		}

	}
}
