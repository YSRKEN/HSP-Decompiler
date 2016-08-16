using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using KttK.HspDecompiler.Ax3ToAs.Dictionary;
using KttK.HspDecompiler.Ax3ToAs.Data.Analyzer;
namespace KttK.HspDecompiler.Ax3ToAs.Data
{
	class AxData
	{
		private AxHeader header;
		TokenCollection tokens = new TokenCollection();
		List<Label> labels = new List<Label>();
		List<Usedll> dlls = new List<Usedll>();
		List<Function> functions = new List<Function>();
		List<Param> functionParams = new List<Param>();
		List<PlugIn> plugIns = new List<PlugIn>();
		Runtime runtime = null;
		List<Function> modules = new List<Function>();

		internal List<Function> Modules
		{
			get { return modules; }
		}

		internal List<PlugIn> PlugIns
		{
			get { return plugIns; }
		}

		internal Runtime Runtime
		{
			get { return runtime; }
		}

		internal AxHeader Header
		{
			get { return header; }
		}
		internal TokenCollection Tokens
		{
			get { return tokens; }
		}
		internal List<Usedll> Usedlls
		{
			get { return dlls; }
		}
		internal List<Label> Labels
		{
			get { return labels; }
		}
		internal List<Function> Functions
		{
			get { return functions; }
		}
		internal List<Param> FunctionParams
		{
			get { return functionParams; }
		}

		internal Label GetLabel(int index)
		{
			return Labels[index];
		}

		internal Function GetUserFunction(int index)
		{
			if (index < 0)
				return null;
			if (index >= functions.Count)
				return null;
			return functions[index];
			//int count = 0;
			//for (int i = 0; i < functions.Count; i++)
			//{
			//    if (functions[i].IsUserFunction)
			//    {
			//        if (count == functionIndex)
			//            return functions[i];
			//        count++;
			//    }
			//}
			//return null;
		}

		internal Function GetDllFunction(int index)
		{
			if (index < 0)
				return null;
			if (index >= functions.Count)
				return null;
			return functions[index];
			//int count = 0;
			//for (int i = 0; i < functions.Count; i++)
			//{
			//    if (functions[i].IsDllFunction)
			//    {
			//        if (count == functionIndex)
			//            return functions[i];
			//        count++;
			//    }
			//}
			//return null;
		}

		internal Usedll GetUsedll(int index)
		{
			if (index < 0)
				return null;
			if (index >= dlls.Count)
				return null;
			return dlls[index];
		}

		internal Param GetParam(int index)
		{
			if (index < 0)
				return null;
			if (index >= functionParams.Count)
				return null;
			return functionParams[index];
		}

		internal Cmd AddCmd(int pluginIndex, int methodIndex)
		{
			if (pluginIndex < 0)
				return null;
			if (pluginIndex >= plugIns.Count)
				return null;
			return plugIns[pluginIndex].AddCmd(methodIndex);
		}

		internal string ReadString(int offset, int max_count)
		{
			long seekOffset = seekOrigin + offset;
			long nowPosition = reader.BaseStream.Position;
			reader.BaseStream.Seek(seekOffset, SeekOrigin.Begin);
			List<Char> chars = new List<char>();
			char token = '\0';
			int count = 0;
			while((token = reader.ReadChar()) != '\0'){
				switch(token){
					case '\\':
						chars.Add('\\');
						chars.Add('\\');
						break;
					case '\"':
						chars.Add('\\');
						chars.Add('\"');
						break;
					case '\t':
						chars.Add('\\');
						chars.Add('t');
						break;
					case '\n':
						chars.Add('\\');
						chars.Add('n');
						break;
					case '\r':
						break;
					default:
						chars.Add(token);
						break;
				}
				count++;
				if (count >= max_count)
					break;
			}
			char[] arrayChars = new char[chars.Count];
			chars.CopyTo(arrayChars);
			reader.BaseStream.Seek(nowPosition, SeekOrigin.Begin);
			return new string(arrayChars);
		}
		internal string ReadStringLiteral(int offset)
		{
			return ReadString((int)(header.LiteralStart + offset), (int)(header.LiteralSize - offset));
		}
		internal double ReadDoubleLiteral(int offset)
		{
			double ret = 0.0;
			long seekOffset = seekOrigin + header.LiteralStart + offset;
			long nowPosition = reader.BaseStream.Position;
			reader.BaseStream.Seek(seekOffset, SeekOrigin.Begin);
			ret = reader.ReadDouble();
			reader.BaseStream.Seek(nowPosition, SeekOrigin.Begin);
			return ret;
		}
		internal string ReadIidCodeLiteral(int offset)
		{
			StringBuilder strbd = new StringBuilder();
			byte[] buf = null;
			long seekOffset = seekOrigin + header.LiteralStart + offset;
			long nowPosition = reader.BaseStream.Position;
			reader.BaseStream.Seek(seekOffset, SeekOrigin.Begin);
			buf = reader.ReadBytes(0x10);
			reader.BaseStream.Seek(nowPosition, SeekOrigin.Begin);
			strbd.Append(@"{");
			strbd.Append(buf[0x03].ToString("X02"));
			strbd.Append(buf[0x02].ToString("X02"));
			strbd.Append(buf[0x01].ToString("X02"));
			strbd.Append(buf[0x00].ToString("X02"));
			strbd.Append('-');
			strbd.Append(buf[0x05].ToString("X02"));
			strbd.Append(buf[0x04].ToString("X02"));
			strbd.Append('-');
			strbd.Append(buf[0x07].ToString("X02"));
			strbd.Append(buf[0x06].ToString("X02"));
			strbd.Append('-');
			strbd.Append(buf[0x08].ToString("X02"));
			strbd.Append(buf[0x09].ToString("X02"));
			strbd.Append('-');
			strbd.Append(buf[0x0A].ToString("X02"));
			strbd.Append(buf[0x0B].ToString("X02"));
			strbd.Append(buf[0x0C].ToString("X02"));
			strbd.Append(buf[0x0D].ToString("X02"));
			strbd.Append(buf[0x0E].ToString("X02"));
			strbd.Append(buf[0x0F].ToString("X02"));
			strbd.Append(@"}");
			return strbd.ToString();
		}

		internal void LoadStart(BinaryReader theReader, Hsp3Dictionary theDictionary)
		{
			if (theReader == null)
				throw new ArgumentNullException("readerにnull値を指定できません");
			if (theDictionary == null)
				throw new ArgumentNullException("dictionaryにnull値を指定できません");
			seekOrigin = theReader.BaseStream.Position;
			reader = theReader;
			dictionary = theDictionary;
			isStarted = true;
		}
		internal void LoadEnd()
		{
			seekOrigin = -1;
			reader = null;
			dictionary = null;
			isStarted = false;
		}

		long seekOrigin;
		BinaryReader reader = null;
		Hsp3Dictionary dictionary = null;
		bool isStarted = false;

		internal bool IsStarted
		{
			get { return isStarted; }
		}
		internal BinaryReader Reader
		{
			get { return reader; }
			set { reader = value; }
		}
		internal long StartOfCode
		{
			get{
				return header.CodeStart + seekOrigin;
			}
		}
		internal Hsp3Dictionary Dictionary
		{
			get { return dictionary; }
			set { dictionary = value; }
		}

		internal void ReadHeader()
		{
			if (!isStarted)
				throw new InvalidOperationException("LoadStartが呼び出されていません");
			long streamSize = reader.BaseStream.Length - seekOrigin;
			if (streamSize < 0x60)
				throw new HspDecoderException("AxData", "ファイルヘッダーが見つかりません");
			try
			{
				header = AxHeader.FromBinaryReader(reader);
			}
			catch (SystemException e)
			{
				throw new HspDecoderException("AxHeader", "ヘッダー解析中に想定外のエラー", e);
			}
			return;
		}
		internal void ReadPreprocessor(Hsp3Dictionary dictionary)
		{
			if (!isStarted)
				throw new InvalidOperationException("LoadStartが呼び出されていません");
			if (header == null)
				throw new InvalidOperationException("ヘッダーが読み込まれていません");
			if (header.RuntimeStart != 0)
			{
				string runtimeName = ReadString((int)header.RuntimeStart, (int)(header.CodeStart - header.RuntimeStart));
				if(runtimeName != null)
					runtime = new Runtime(runtimeName);
			}
			uint count = header.LabelCount;
			for (int i = 0; i < count; i++)
			{
				long offset = seekOrigin + header.LabelStart + ((int)HeaderDataSize.Label * i);
				reader.BaseStream.Seek(offset, SeekOrigin.Begin);
				labels.Add(Label.FromBinaryReader(reader, this, i));
			}

			count = header.DllCount;
			for (int i = 0; i < count; i++)
			{
				long offset = seekOrigin + header.DllStart + ((int)HeaderDataSize.Dll * i);
				reader.BaseStream.Seek(offset, SeekOrigin.Begin);
				dlls.Add(Usedll.FromBinaryReader(reader, this, i));
			}

			count = header.ParameterCount;
			for (int i = 0; i < count; i++)
			{
				long offset = seekOrigin + header.ParameterStart + ((int)HeaderDataSize.Parameter * i);
				reader.BaseStream.Seek(offset, SeekOrigin.Begin);
				functionParams.Add(Param.FromBinaryReader(reader, this,i));
			}

			count = header.FunctionCount;
			for (int i = 0; i < count; i++)
			{
				long offset = seekOrigin + header.FunctionStart + ((int)HeaderDataSize.Function * i);
				reader.BaseStream.Seek(offset, SeekOrigin.Begin);
				functions.Add(Function.FromBinaryReader(reader, this,i));
			}

			count = header.PluginCount;
			for (int i = 0; i < count; i++)
			{
				long offset = seekOrigin + header.PluginStart + ((int)HeaderDataSize.Plugin * i);
				reader.BaseStream.Seek(offset, SeekOrigin.Begin);
				plugIns.Add(PlugIn.FromBinaryReader(reader, this, i));
			}
			if ((count != 0) && (header.PluginParameterCount != 0))
			{
				plugIns[0].ExtendedTypeCount = (int)header.PluginParameterCount;
			}

			foreach (Param param in functionParams)
			{
				param.SetFunction(this);
			}
			RenameFunctions(dictionary);

		}


		internal void DeleteInvisibleLables()
		{
			labels = labels.FindAll(LabelIsVisible);
		}

		private bool LabelIsVisible(Label label)
		{
			return label.Visible;
		}


		private void RenameFunctions(Hsp3Dictionary dictionary)
		{
			//#funcなどの名前かぶり対策
			List<string> functionNames = new List<string>();
			List<Function> initializer = new List<Function>();
			List<Function> comfuncs = new List<Function>();
			List<Function> dllfuncs = new List<Function>();
			//ver1.20　標準命令を避けるように
			functionNames.AddRange(dictionary.GetAllFuncName());
			foreach (Function func in functions)
			{
				switch (func.Type)
				{
					case FunctionType.cfunc:
					case FunctionType.func:
						dllfuncs.Add(func);
						break;
					case FunctionType.comfunc:
						comfuncs.Add(func);
						break;

					case FunctionType.defcfunc:
					case FunctionType.deffunc:
					case FunctionType.module:
						if (func.ParentModule != null)//(func.DefaultName.Equals("__init", StringComparison.OrdinalIgnoreCase) || func.DefaultName.Equals("__term", StringComparison.OrdinalIgnoreCase))
							initializer.Add(func);
						else
						{
							func.SetName(func.DefaultName);
							functionNames.Add(func.DefaultName.ToLower());
						}
						break;
				}
			}
			foreach (Function func in initializer)
			{
				string defName = func.DefaultName;
				if (!functionNames.Contains(defName.ToLower()))
				{
					func.SetName(defName);
					functionNames.Add(defName.ToLower());
					continue;
				}
				string newName = defName;
				int index = 1;
				do
				{
					newName = string.Format("{0}_{1}", defName, index);
					index++;
				} while (functionNames.Contains(newName));
				func.SetName(newName);
				functionNames.Add(newName.ToLower());
			}
			foreach (Function func in dllfuncs)
			{
				string defName = func.DefaultName;
				string newName = defName;
				if (newName.StartsWith("_", StringComparison.Ordinal) && (newName.Length > 1))
					newName = newName.Substring(1);
				int atIndex = newName.IndexOf("@", StringComparison.Ordinal);
				if(atIndex > 0)
					newName = newName.Substring(0, atIndex);
				if (!functionNames.Contains(newName.ToLower()))
				{
					func.SetName(newName);
					functionNames.Add(newName.ToLower());
					continue;
				}
				int index = 1;
				do
				{
					newName = string.Format("func_{0}", index);
					index++;
				} while (functionNames.Contains(newName));
				func.SetName(newName);
				functionNames.Add(newName.ToLower());
			}
			foreach (Function func in comfuncs)
			{
				string newName = "";
				int index = 1;
				do
				{
					newName = string.Format("comfunc_{0}", index);
					index++;
				} while (functionNames.Contains(newName));//.ToLower()
				func.SetName(newName);
				functionNames.Add(newName.ToLower());
			}
			
		}

		internal void RenameLables()
		{
			if(labels.Count <= 0)
				return;
			labels.Sort();
			int keta = ((int)System.Math.Log10(labels.Count)) + 1;
			string formatBase = "*label_{0:D0" + keta.ToString() + "}" ;
			for (int i = 0; i < labels.Count; i++)
			{
				labels[i].LabelName = string.Format(formatBase, i);
			}
			return;
		}
	}
}
