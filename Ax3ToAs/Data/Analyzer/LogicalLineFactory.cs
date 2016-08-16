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
		//�O���猩����̂͂�������
		/// <summary>
		/// ��s����TokenCollection����LogicalLine���쐬����
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
				if (stream.NextToken is IfStatementPrimitive) //if, else�s
					return (LogicalLine)readIf(stream);
				if (stream.NextToken is McallFunctionPrimitive) //on�s
					return (LogicalLine)readMcall(stream);
				if (stream.NextToken is OnEventFunctionPrimitive)
				{ //on###�s
					if(stream.NextNextTokenIsGotoFunction)//goto/gosub���Ȃ��Ȃ�
						return (LogicalLine)readOnEvent(stream);
					else
						return (LogicalLine)readCommand(stream);
				}
				if (stream.NextToken is OnFunctionPrimitive) //on�s
					return (LogicalLine)readOn(stream);
				if (stream.NextToken is FunctionPrimitive)//���̑��̊֐�
					return (LogicalLine)readCommand(stream);
				if (stream.NextToken is VariablePrimitive)//����s
					return (LogicalLine)readAssignment(stream);
			}
			//������HspLogicalLineException��catch����B���̂Ƃ���ł͍s���Ă͂Ȃ�Ȃ�
			catch (HspLogicalLineException e)
			{

				line = new UnknownLine(stream.Primitives);
				line.AddError(e.Message);
				return line;
			}
			line = new UnknownLine(stream.Primitives);
			line.AddError("�H�s�F�擪�̒P�ꂪ���߂ł��Ȃ�");
			return line;
		}

		private static LogicalLine readMcall(TokenCollection stream)
		{
			int start = stream.Position;
			McallFunctionPrimitive mcall = stream.GetNextToken() as McallFunctionPrimitive;
			if (mcall == null)
				throw new HspLogicalLineException("mcall�Fmcall�v���~�e�B�u�ȊO����X�^�[�g");
			if (stream.NextIsEndOfLine)
			{
				stream.Position = start;
				return (LogicalLine)readCommand(stream);
			}
			ExpressionToken exp = CodeTokenFactory.ReadExpression(stream);
			if (exp.CanRpnConvert)//RPN�ϊ����ł���Ȃ畁�ʂ̊֐��Ƃ��Ĉ����B
			{
				stream.Position = start;
				return (LogicalLine)readCommand(stream);
			}

			stream.Position = start;
			stream.GetNextToken();
			VariablePrimitive var = stream.GetNextToken() as VariablePrimitive;
			if (var == null)
				throw new HspLogicalLineException("mcall�s�F�ϊ��s�\�Ȍ`��");
			if (stream.NextIsBracketStart)//mcall �̋L�@�͔z��ϐ���F�߂Ȃ�
				throw new HspLogicalLineException("mcall�s�F�ϊ��s�\�Ȍ`��");
			if (stream.NextIsEndOfLine)
				throw new HspLogicalLineException("mcall�s�F�ϊ��s�\�Ȍ`��");
			exp = CodeTokenFactory.ReadExpression(stream);
			if (stream.NextIsEndOfLine)
				return new McallStatement(mcall, var, exp, null);
			ArgumentToken arg = CodeTokenFactory.ReadArgument(stream);
			if (stream.NextIsEndOfLine)
				return new McallStatement(mcall, var, exp, arg);
			throw new HspLogicalLineException("mcall�s�F�]���ȃg�[�N��������");
		}

		private static OnStatement readOn(TokenCollection stream)
		{
			OnFunctionPrimitive token = stream.GetNextToken() as OnFunctionPrimitive;
			if (token == null)
				throw new HspLogicalLineException("on��������s�F��������v���~�e�B�u�ȊO����X�^�[�g");
			//�����Ȃ����Ƃ����邩������Ȃ�(���s���G���[)
			if (stream.NextIsEndOfLine)
				return new OnStatement(token,null, null);
			//����ǂށBgoto/gosub���Ȃ����Ƃ����邩������Ȃ�(���s���G���[)
			ExpressionToken exp = CodeTokenFactory.ReadExpression(stream);
			if (stream.NextIsEndOfLine)
				return new OnStatement(token, exp,null);
			//goto/gosub�֐���ǂށBgoto/gosub�ȊO�ł��R���p�C���͒ʂ�(���s���G���[)
			//���̊֐��ɂ�()�����Ȃ��B
			FunctionToken func = CodeTokenFactory.ReadFunction(stream, false);
			if (stream.NextIsEndOfLine)
				return new OnStatement(token, exp, func);
			//�܂����܂��Ă���G���[�ˁB
			throw new HspLogicalLineException("on��������s�F�]���ȃg�[�N��������");

		}

		private static OnEventStatement readOnEvent(TokenCollection stream)
		{
			OnEventFunctionPrimitive token = stream.GetNextToken() as OnEventFunctionPrimitive;
			if (token == null)
				throw new HspLogicalLineException("on��������s�F��������v���~�e�B�u�ȊO����X�^�[�g");
			//goto/gosub���Ȃ����Ƃ����邩������Ȃ�(���s���G���[)
			if (stream.NextIsEndOfLine)
				return new OnEventStatement(token, null);
			//goto/gosub�֐���ǂށBgoto/gosub�ȊO�ł��R���p�C���͒ʂ�(���s���G���[)
			//���̊֐��ɂ�()�����Ȃ��B
			FunctionToken func = CodeTokenFactory.ReadFunction(stream, false);
			if (stream.NextIsEndOfLine)
				return new OnEventStatement(token, func);
			//�܂����܂��Ă���G���[�ˁB
			throw new HspLogicalLineException("on��������s�F�]���ȃg�[�N��������");
		}

		/// <summary>
		/// ����s
		/// </summary>
		/// <param defaultName="primitives"></param>
		/// <returns></returns>
		private static Assignment readAssignment(TokenCollection stream)
		{
			//�ϐ���ǂށB���s���������s����Ȃ��B
			VariableToken token = CodeTokenFactory.ReadVariable(stream);
			//���Z�q��ǂށB���s����Ȃ����s����Ȃ��B
			if (stream.NextIsEndOfLine)
				throw new HspLogicalLineException("����s�F���Z�q�Ȃ�");
			OperatorToken op = CodeTokenFactory.ReadOperator(stream);
			//���������Ȃ����Ƃ�����B"x++"�Ƃ�
			if (stream.NextIsEndOfLine)
				return new Assignment(token, op);
			else
			{
				//������ǂށB���܂肪�o�Ȃ����OK
				ArgumentToken arg = CodeTokenFactory.ReadArgument(stream);
				if (stream.NextIsEndOfLine)
					return new Assignment(token, op, arg);
			}
			throw new HspLogicalLineException("����s�F�]���ȃg�[�N��������");

		}

		/// <summary>
		/// If,else�X�e�[�g�����g�̊J�n
		/// </summary>
		/// <param defaultName="primitives"></param>
		/// <returns></returns>
		private static IfStatement readIf(TokenCollection stream)
		{
			IfStatementPrimitive token = stream.GetNextToken() as IfStatementPrimitive;
			if (token == null)
				throw new HspLogicalLineException("��������s�F��������v���~�e�B�u�ȊO����X�^�[�g");

			//else�ɂ͎����Ȃ��B
			if (stream.NextIsEndOfLine)
				return new IfStatement(token);
			else
			{
				//����ǂށB���܂肪�o�Ȃ����OK
				ArgumentToken arg = CodeTokenFactory.ReadArgument(stream);
				if (stream.NextIsEndOfLine)
					return new IfStatement(token, arg);
			}
			throw new HspLogicalLineException("��������s�F�]���ȃg�[�N��������");
		}

		/// <summary>
		/// ���ߍs
		/// </summary>
		/// <param defaultName="primitives"></param>
		/// <returns></returns>
		private static Command readCommand(TokenCollection stream)
		{
			//���Ԃ�CodeTokenFactory�ɔC����
			//���߂ɂ́i�֐��ƈ���āj���ʂ͂���Ȃ�
			FunctionToken func = CodeTokenFactory.ReadFunction(stream, false);
			if (stream.NextIsEndOfLine)
				return new Command(func);
			throw new HspLogicalLineException("���ߍs�F�]���ȃg�[�N��������");
		}

	}
}
