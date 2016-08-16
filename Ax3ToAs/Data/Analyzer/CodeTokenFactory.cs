using System;
using System.Collections.Generic;
using System.Text;
using KttK.HspDecompiler.Ax3ToAs.Data.Token;
using KttK.HspDecompiler.Ax3ToAs.Data.Primitive;
using KttK.HspDecompiler.Ax3ToAs.Data;
namespace KttK.HspDecompiler.Ax3ToAs.Data.Analyzer
{
	//CodeTokenFactory��LogicalLineFactory�̈ꕔ�ł���B
	//�������H��H
partial class LogicalLineFactory
{
	private static class CodeTokenFactory
	{
		internal static ExpressionToken ReadExpression(TokenCollection stream)
		{
			if (stream.NextIsEndOfLine)
				throw new HspLogicalLineException("���F�ǂݍ��ݒ��ɃX�^�b�N����ɂȂ���");
			ExpressionTermToken elem = null;
			List<ExpressionTermToken> elements = new List<ExpressionTermToken>();
			do//�Œ��̓p�����[�^������͂��B
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
					throw new HspLogicalLineException("���F�s�K�؂ȗv�f�����o");
				elements.Add(elem);
			} while (!stream.NextIsEndOfParam);
			ExpressionToken ret = new ExpressionToken(elements);
			ret.RpnConvert();
			return ret;
		}

		private static object ReadLiteral(TokenCollection stream)
		{
			if (stream.NextIsEndOfLine)
				throw new HspLogicalLineException("���e�����F�ǂݍ��ݒ��ɃX�^�b�N����ɂȂ���");
			//���e�����v���e�B�u�ǂݍ���
			LiteralPrimitive token = stream.GetNextToken() as LiteralPrimitive;
			if (token == null)
				throw new HspLogicalLineException("���e�����F�s�K�؂ȗv�f�����o");
			return new LiteralToken(token);
		}

		internal static ArgumentToken ReadArgument(TokenCollection stream)
		{
			if (stream.NextIsEndOfLine)
				throw new HspLogicalLineException("�����F�ǂݍ��ݒ��ɃX�^�b�N����ɂȂ���");
			bool hasBracket = stream.NextIsBracketStart;
			//���ʓǂݎ̂āB
			if (hasBracket)
				stream.GetNextToken();
			List<ExpressionToken> exps = new List<ExpressionToken>();
			bool firstArgIsNull = stream.NextIsEndOfParam;
			//��x������ǂݎn�߂���s����')'�܂œǂ݂���
			while (!stream.NextIsEndOfLine)
			{
				if (hasBracket & stream.NextIsBracketEnd)
				{
					//���ʓǂݎ̂� & �����I��
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
				throw new HspLogicalLineException("�֐��F�ǂݍ��ݒ��ɃX�^�b�N����ɂȂ���");
			FunctionPrimitive token = stream.GetNextToken() as FunctionPrimitive;
			if (token == null)
				throw new HspLogicalLineException("�֐��F�֐��v���~�e�B�u�ȊO����X�^�[�g");
			//�s���Ȃ狭���I�ɏI��
			if (stream.NextIsEndOfLine)
				return new FunctionToken(token);
			//�S�[�X�g���x������
			if (token.HasGhostLabel	&& (stream.NextToken.CodeType == HspCodeType.Label))
			{
				stream.GetNextToken();
				if (stream.NextIsEndOfLine)
					return new FunctionToken(token);
			}

			//����'('�Ȃ�����ǂݍ���
			if (stream.NextIsBracketStart)
				return new FunctionToken(token, ReadArgument(stream));
			//�ǂ���ł��Ȃ��Ȃ�AhasBracket�ŕ���
			//hasBracket�@=false�Ȃ狭���I�Ɉ�������ɁB
			if (hasBracket)
				return new FunctionToken(token);
			else
				return new FunctionToken(token, ReadArgument(stream));
		}

		internal static VariableToken ReadVariable(TokenCollection stream)
		{
			if (stream.NextIsEndOfStream)
				throw new HspLogicalLineException("�ϐ��F�ǂݍ��ݒ��ɃX�^�b�N����ɂȂ���");
			//�ϐ��v���e�B�u�ǂݍ���
			VariablePrimitive token = stream.GetNextToken() as VariablePrimitive;
			if (token == null)
				throw new HspLogicalLineException("�ϐ��F�ϐ��v���~�e�B�u�ȊO����X�^�[�g");

			//�������ʂȂ�Δz��ϐ�
			if (stream.NextIsBracketStart){
				return new VariableToken(token, ReadArgument(stream));
			}
			//�����łȂ���Ε��ʂ̕ϐ�
			return new VariableToken(token);
			//if (parser.NextIsEndOfLine)
			//    return VariableToken(token);
		}

		internal static OperatorToken ReadOperator(TokenCollection stream)
		{
			if (stream.NextIsEndOfLine)
				throw new HspLogicalLineException("���Z�q�F�ǂݍ��ݒ��ɃX�^�b�N����ɂȂ���");
			//�ϐ��v���e�B�u�ǂݍ���
			OperatorPrimitive token = stream.GetNextToken() as OperatorPrimitive;
			if (token == null)
				throw new HspLogicalLineException("���Z�q�F�s�K�؂ȗv�f�����o");
			return new OperatorToken(token);

		}
	}
}

}
