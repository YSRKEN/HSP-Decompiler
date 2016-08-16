using System;
using System.IO;
using System.Text;
using System.Diagnostics;
using System.Collections.Generic;
namespace KttK.HspDecompiler.Ax2ToAs.Data
{
	/// <summary>
	/// AxData の概要の説明です。
	/// </summary>
internal class AxData
{
	internal AxData()
	{
		// 
		// TODO: コンストラクタ ロジックをここに追加してください。
		//
	}

	private Header header;
	private Label[] labels;
	private Dll[] dlls;
	private Func[] funcs;
	private Deffunc[] deffuncs;
	private Module[] modules;

	private byte[] labelData;
	private byte[] dllData;
	private byte[] funcData;
	private byte[] deffuncData;
	private byte[] moduleData;
	private byte[] tokenData;
	internal byte[] TokenData
	{
		get
		{
			return  tokenData;
		}
	}

	private byte[] stringData;
	private void readData(Stream stream)
	{
		long startPosition = stream.Position;
		byte[] headerBuffer = new byte[80];
		if (stream.Read(headerBuffer, 0, 80) < 80)
			throw new HspDecoderException("AxData", "ファイルヘッダーが見つかりません");
		int[] buffer = new int[20];
		for (int i = 0; i< 20; i++)
		{
			buffer[i] = BitConverter.ToInt32(headerBuffer, i*4);
		}
		try
		{
			this.header = Header.FromIntArray(buffer);
		}
		catch(Exception e)
		{
			throw new HspDecoderException("AxHeader", "ヘッダー解析中に想定外のエラー",e);
		}
		if (this.header == null)
			throw new HspDecoderException("AxHeader", "ヘッダー解析に失敗");
		try
		{
			Header head = this.header;
			this.tokenData = new byte[head.ScriptByte];
			stream.Seek(startPosition + head.ScriptOffset, SeekOrigin.Begin);
			stream.Read(this.tokenData, 0, head.ScriptByte);

			this.dllData = new byte[head.DllByte];
			stream.Seek(startPosition + head.DllOffset, SeekOrigin.Begin);
			stream.Read(this.dllData, 0, head.DllByte);

			this.funcData = new byte[head.FuncByte];
			stream.Seek(startPosition + head.FuncOffset, SeekOrigin.Begin);
			stream.Read(this.funcData, 0, head.FuncByte);

			this.deffuncData = new byte[head.DeffuncByte];
			stream.Seek(startPosition + head.DeffuncOffset, SeekOrigin.Begin);
			stream.Read(this.deffuncData, 0, head.DeffuncByte);

			this.moduleData = new byte[head.ModuleByte];
			stream.Seek(startPosition + head.ModuleOffset, SeekOrigin.Begin);
			stream.Read(this.moduleData, 0, head.ModuleByte);

			this.labelData = new byte[head.LabelByte];
			stream.Seek(startPosition + head.LabelOffset, SeekOrigin.Begin);
			stream.Read(this.labelData, 0, head.LabelByte);

			this.stringData = new byte[head.TextByte];
			stream.Seek(startPosition + head.TextOffset, SeekOrigin.Begin);
			stream.Read(this.stringData, 0, head.TextByte);
		}
		catch(Exception e)
		{
			throw new HspDecoderException("AxHeader", "ストリームの読み取り中に想定外のエラー",e);
		}
		stream.Seek(startPosition, SeekOrigin.Begin);
	}

	#region create
	internal static AxData FromStream(Stream stream)
	{
		AxData data = new AxData();

		data.readData(stream);

		return data;
	}
	#endregion
	#region read
	internal string GetString(int offset)
	{
		return ReadString(offset, stringData);
	}

	private string ReadString(int offset)
	{
		return ReadString(offset, stringData);
	}

	private string ReadString(int offset, byte[] dumpData)
	{
		Encoding encode = Encoding.GetEncoding("SHIFT-JIS");
		List<byte> buffer = new List<byte>();
		byte token;
		while(offset < dumpData.Length)
		{
			token = dumpData[offset];
			offset++;
			if (token == 0)
				break;
			buffer.Add(token);
		}
		if (buffer.Count == 0)
			return "";
		byte[] bytes = new byte[buffer.Count];
		buffer.CopyTo(bytes);
		return encode.GetString(bytes);
	}

	private void ReadLabels()
	{
		labels = new Label[header.LabelCount];
		for (int i=0; i<header.LabelCount; i++)
		{
			int offset = i * 4;
			labels[i] = new Label(i, BitConverter.ToInt32(labelData, offset));
		}
	}

	private void ReadDlls()
	{
		dlls = new Dll[header.DllCount];

		for (int i = 0; i< header.DllCount; i++)
		{
			int offset = 4 + (i * 24);
			dlls[i].Name = ReadString(offset, dllData);
		}
	}

	private void ReadFuncs()
	{
		funcs = new Func[header.FuncCount];
		for (int i = 0; i< header.FuncCount; i++)
		{
			int offset = i * 16;
			funcs[i].DllIndex = BitConverter.ToInt16(funcData, offset);
			offset += 4;
			funcs[i].HikiType = BitConverter.ToInt16(funcData, offset);
			offset += 4;
			int funcnameOffset = BitConverter.ToInt32(funcData, offset);
			funcs[i].Name = ReadString(funcnameOffset);
		}
	}

	private void ReadModules()
	{
		if (header.ModuleCount==0)
			return;
		modules = new Module[header.ModuleCount];

		for (int i = 0; i< header.ModuleCount; i++)
		{
			int offset = 4 + (i*24);
			modules[i].Name = ReadString(offset, dllData);
		}
	}

	private void ReadDeffuncs()
	{
		deffuncs = new Deffunc[header.DeffuncCount];

		for (int i = 0; i< header.DeffuncCount; i++)
		{
			int offset = i* 16;
			int labelIndex = BitConverter.ToInt32(deffuncData, offset) - 0x1000;
			labels[labelIndex].Deffunc = i;

			offset += 4;
			deffuncs[i].HikiType = BitConverter.ToInt16(deffuncData, offset);
			offset += 2;
			deffuncs[i].HikiCount = BitConverter.ToInt16(deffuncData, offset);
			offset += 2;
			int deffuncnameOffset = BitConverter.ToInt32(deffuncData, offset);
			deffuncs[i].Name = ReadString(deffuncnameOffset);
			labels[labelIndex].Name = deffuncs[i].ToString();
		}
	}

	#endregion

	List<string> lines = new List<string>();
	internal void Decompile()
	{
		int startTime = System.Environment.TickCount;
		Token.CurrentData = this;
		lines.Clear();
		
		ReadLabels();
		ReadDlls();
		//ReadScript();
		ReadFuncs();
		ReadModules();
		ReadDeffuncs();

		if(dlls != null)
		{
			for (int i = 0; i<dlls.Length;i++)
			{
				lines.Add(dlls[i].ToString());
				if(funcs != null)
				{
					for (int j = 0; j<funcs.Length;j++)
					{
						if (funcs[j].DllIndex == i)
							lines.Add(funcs[j].ToString());
					}
				}
			}
		}

		Token.SetZero();
		Token token;
		//ラベルが呼び出される回数を調べる
		try
		{
			while((token = Token.GetNext())!=null)
			{
				if (token.LabelIndex!= -1)
					labels[token.LabelIndex].LoadCount += 1;
			}
		}
		catch(Exception e)
		{
			throw new HspDecoderException("AxHeader", "ラベルの読み取り中に復帰できないエラー", e);
		}

		enabledCount = 0;
		for(int i = 0; i< labels.Length; i++)
		{
			if (labels[i].LoadCount > 0)
				labels[i].Enabled = true;
			else
				labels[i].Enabled = false;
			if (labels[i].Enabled)
				enabledCount++;
		}

		string line;
		Token.SetZero();
		while((line = GetLine())!=null)
		{
			lines.Add(line);
		}
		int scoopCount = tabNo - 1;
		//if (scoopCount != 0)
		//    MainProc.Process.WriteLog("※警告※ " + scoopCount.ToString() + "個の未解決スコープが残りました");
		
		return;
	}

	private void AddLabel()
	{
		for(int i = 0; i< labels.Length; i++)
		{
			if( !labels[i].Enabled )
				continue;
			if( Token.Index>=labels[i].TokenIndex )
			{
				lines.Add(labels[i].ToString());
				labels[i].Enabled = false;
			}
		}
	}

	private string GetTab(int tab)
	{
		string ret = "";
		Debug.Assert( tab >= 0);
		for (int i = 0; i< tab; i++)
		{
			ret += "\t";
		}
		return ret;
	}

	private int tabNo = 1;
	private List<int> ifEnd = new List<int>();
	private int unknownCount = 0;
	private int usedCount = 0;
	private int enabledCount = 0;
	private string GetLine()
	{
		string line = "";
		Token token= Token.GetNext();
		if ( token == null)
			return null;
		for( int i = 0 ; i < ifEnd.Count; i++)
		{
			if ((token.Id == (int)ifEnd[i]) || (token.IfJumpId == (int)ifEnd[i]))
			{
				tabNo--;
				lines.Add(GetTab(tabNo) + "}");
				ifEnd.RemoveAt(i);
				i--;
			}
		}
		for( int i = 0; i<labels.Length; i++)
		{
			if (!labels[i].Enabled)
				continue;
			if (token.Id == labels[i].TokenIndex)
			{
				lines.Add(labels[i].Name);
				labels[i].Enabled = false;
				usedCount++;
			}
		}
		bool tabPlus = token.TabPlus;
		int ifJumpTo = token.IfJumpTo;
		if (token.TabMinus)
		{
			tabNo--;
		}
		line = GetTab(tabNo);
		line += token.GetString();

		if (!token.isKnown)
		{
			//MainProc.Process.WriteLog("解釈できないコード: " + (lines.Count + 1).ToString() + "行目 :" + token.GetString());
			unknownCount++;
		}
		if ( !token.isLineend )
		{ 
			while((token = Token.GetNext())!=null)
			{
				string add = token.GetString();
				if ( token.isArg )
					line += ", ";
				else
					line += " ";
				line += add;
				if (!token.isKnown)
				{
					//MainProc.Process.WriteLog("解釈できないコード: " + (lines.Count + 1).ToString() + "行目 :" + token.GetString());
					unknownCount++;
				}
				if ( token.isLineend )
					break;
			}
		}
		if(tabPlus)
			tabNo++;
		if ( ifJumpTo >= 0 )
		{
			line += " {";
			ifEnd.Add(ifJumpTo);
		}
		return line;
	}

	internal string GetDeffuncName(int index)
	{
		if ((index >= deffuncs.Length) || (index < 0))
			return null;
		return deffuncs[index].Name;
	}

	internal string GetFuncName(int index)
	{
		if ((index >= funcs.Length) || (index < 0))
			return null;
		return funcs[index].Name;
	}

	internal List<string> GetLines()
	{
		return lines;
	}


}
}
