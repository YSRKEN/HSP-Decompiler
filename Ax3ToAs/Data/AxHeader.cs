using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
namespace KttK.HspDecompiler.Ax3ToAs.Data
{
	internal enum HeaderDataSize
	{
		Code = 2,
		Label = 4,
		Dll = 16,
		Function = 28,
		Parameter = 8,
		Plugin = 16,
	}

	internal sealed class AxHeader
	{
		static internal AxHeader FromBinaryReader(BinaryReader reader)
		{
			long seekOrigin = reader.BaseStream.Position;
			AxHeader header = new AxHeader();
			try
			{
				header.fileType = new string(reader.ReadChars(4));
				for (int i = 1; i < 21; i++)
				{
					header.data[i] = reader.ReadUInt32();
				}
				header.data[21] = reader.ReadUInt16();
				header.data[22] = reader.ReadUInt16();
				header.data[23] = reader.ReadUInt32();
				header.data[24] = reader.ReadUInt32();
			}
			catch (Exception e)
			{
				throw new HspDecoderException("AxHeader","�t�@�C���`�����Ⴂ�܂�", e);
			}
			header.checkHeader(reader.BaseStream.Length - seekOrigin);
			return header;
		}

		private void checkHeader(long fileSize)
		{
			if (fileType.Equals("HSP2", StringComparison.Ordinal))
				throw HSPDecoderException("HSP2�ȑO�ŃR���p�C�����ꂽ�t�@�C���ł�");
			if (!fileType.Equals("HSP3", StringComparison.Ordinal))
				throw HSPDecoderException("�t�@�C���`�����Ⴂ�܂�");
			if (AllDataSize > fileSize)
				throw HSPDecoderException("�w�b�_�̏�񂪈ُ�ł�(�w�b�_�ɋL�ڂ���Ă���t�@�C���T�C�Y�����ۂ̃t�@�C�����𒴂��Ă��܂�)");


			if (CodeStart > AllDataSize)
				throw HSPDecoderException("�w�b�_�̏�񂪈ُ�ł�(�R�[�h�̈�̊J�n�ʒu���t�@�C�����𒴂��Ă��܂�)");
			if (LiteralStart > AllDataSize)
				throw HSPDecoderException("�w�b�_�̏�񂪈ُ�ł�(���e�����̈�̊J�n�ʒu���t�@�C�����𒴂��Ă��܂�)");
			if (LabelStart > AllDataSize)
				throw HSPDecoderException("�w�b�_�̏�񂪈ُ�ł�(���x����`�̈�̊J�n�ʒu���t�@�C�����𒴂��Ă��܂�)");
			if (DllStart > AllDataSize)
				throw HSPDecoderException("�w�b�_�̏�񂪈ُ�ł�(DLL��`�̈�J�n�̈ʒu���t�@�C�����𒴂��Ă��܂�)");
			if (FunctionStart > AllDataSize)
				throw HSPDecoderException("�w�b�_�̏�񂪈ُ�ł�(���[�U��`�֐��̈�̊J�n�ʒu���t�@�C�����𒴂��Ă��܂�)");
			if (PluginStart > AllDataSize)
				throw HSPDecoderException("�w�b�_�̏�񂪈ُ�ł�(�v���O�C����`�̈�̊J�n�ʒu���t�@�C�����𒴂��Ă��܂�)");
			if (ParameterStart > AllDataSize)
				throw HSPDecoderException("�w�b�_�̏�񂪈ُ�ł�(�֐��p�����[�^��`�̈�̊J�n�ʒu���t�@�C�����𒴂��Ă��܂�)");

			if (CodeEnd > AllDataSize)
				throw HSPDecoderException("�w�b�_�̏�񂪈ُ�ł�(�R�[�h�̈�̏I�[���t�@�C�����𒴂��Ă��܂�)");
			if (LiteralEnd > AllDataSize)
				throw HSPDecoderException("�w�b�_�̏�񂪈ُ�ł�(���e�����̈�̏I�[���t�@�C�����𒴂��Ă��܂�)");
			if (LabelEnd > AllDataSize)
				throw HSPDecoderException("�w�b�_�̏�񂪈ُ�ł�(���x����`�̈�̏I�[���t�@�C�����𒴂��Ă��܂�)");
			if (DllEnd > AllDataSize)
				throw HSPDecoderException("�w�b�_�̏�񂪈ُ�ł�(DLL��`�̈�̏I�[���t�@�C�����𒴂��Ă��܂�)");
			if (FunctionEnd > AllDataSize)
				throw HSPDecoderException("�w�b�_�̏�񂪈ُ�ł�(���[�U��`�֐��̈�̏I�[���t�@�C�����𒴂��Ă��܂�)");
			if (PluginEnd > AllDataSize)
				throw HSPDecoderException("�w�b�_�̏�񂪈ُ�ł�(�v���O�C����`�̈�̏I�[���t�@�C�����𒴂��Ă��܂�)");
			if (ParameterEnd > AllDataSize)
				throw HSPDecoderException("�w�b�_�̏�񂪈ُ�ł�(�֐��p�����[�^��`�̈�̏I�[���t�@�C�����𒴂��Ă��܂�)");
		}

		private static HspDecoderException HSPDecoderException(string str)
		{
			return new HspDecoderException("AxHeader", str);
		}


		string fileType;
		uint[] data = new uint[25];
		internal const uint HeaderSize = 0x60;
		internal string FileType
		{
			get { return fileType; }
		}

		internal uint AllDataSize	{ get { return data[3]; } }
		internal uint CodeStart	{ get { return data[4]; } }
		internal uint CodeSize	{ get { return data[5] / (int)HeaderDataSize.Code; } }
		internal uint CodeEnd		{ get { return CodeStart + CodeSize; } }
		internal uint LiteralStart{ get { return data[6]; } }
		internal uint LiteralSize { get { return data[7]; } }
		internal uint LiteralEnd { get { return LiteralStart + LiteralSize; } }
		internal uint LabelStart	{ get { return data[8]; } }
		internal uint LabelSize	{ get { return data[9]; } }
		internal uint LabelCount	{ get { return data[9] / (int)HeaderDataSize.Label; } }
		internal uint LabelEnd	{ get { return LabelStart + LabelSize; } }
		internal uint DebugStart	{ get { return data[10]; } }
		internal uint DebugSize { get { return data[11]; } }
		internal uint DebugEnd { get { return DebugStart + DebugSize; } }
		internal uint DllStart	{ get { return data[12]; } }
		internal uint DllSize		{ get { return data[13]; } }
		internal uint DllCount	{ get { return data[13] / (int)HeaderDataSize.Dll; } }
		internal uint DllEnd		{ get { return DllStart + DllSize; } }
		internal uint FunctionStart { get { return data[14]; } }
		internal uint FunctionSize { get { return data[15]; } }
		internal uint FunctionCount { get { return data[15] / (int)HeaderDataSize.Function; } }
		internal uint FunctionEnd { get { return FunctionStart + FunctionSize; } }
		internal uint ParameterStart { get { return data[16]; } }
		internal uint ParameterSize { get { return data[17]; } }
		internal uint ParameterCount { get { return data[17] / (int)HeaderDataSize.Parameter; } }
		internal uint ParameterEnd { get { return ParameterStart + ParameterSize; } }
//		internal uint Start { get { return data[18]; } }
//		internal uint Size { get { return data[19]; } }
//		internal uint Count { get { return data[19] / (int)HeaderDataSize.; } }
//		internal uint End { get { return Start + Size; } }
		internal uint PluginStart { get { return data[20]; } }
		internal uint PluginSize { get { return data[21]; } }
		internal uint PluginCount { get { return data[21] / (int)HeaderDataSize.Plugin; } }
		internal uint PluginParameterCount { get { return data[22]; } }
		internal uint PluginEnd { get { return PluginStart + PluginSize; } }
		internal uint RuntimeStart { get { return data[24]; } }
	}
}
