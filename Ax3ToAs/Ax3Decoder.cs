using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using KttK.HspDecompiler.Ax3ToAs.Data;
using KttK.HspDecompiler.Ax3ToAs.Dictionary;

#if AllowDecryption
using KttK.HspDecompiler.DpmToAx.HspCrypto;
#endif
using KttK.HspDecompiler.Ax3ToAs.Data.Analyzer;
namespace KttK.HspDecompiler.Ax3ToAs
{
	class Ax3Decoder : AbstractAxDecoder
	{
		internal Ax3Decoder() { }


		private Hsp3Dictionary dictionary;

		internal Hsp3Dictionary Dictionary
		{
			get { return dictionary; }
			set { dictionary = value; }
		}
		

//		internal List<string> Decode(string axPath)

#if AllowDecryption
//		internal List<string> DecodeAndDecrypt(BinaryReader reader,int fileSize)

#endif

		public override List<string> Decode(BinaryReader reader)
		{
			AxData data = new AxData();
			LexicalAnalyzer lex = null;
			TokenCollection stream = null;
			SyntacticAnalyzer synt = null;
			List<LogicalLine> lines = null;
			List<string> stringLines = new List<string>();
			try
			{
				global::KttK.HspDecompiler.HspConsole.Write("ヘッダー解析中...");
				data.LoadStart(reader, dictionary);
				data.ReadHeader();
				global::KttK.HspDecompiler.HspConsole.Write("プリプロセッサ解析中...");
				data.ReadPreprocessor(dictionary);
				global::KttK.HspDecompiler.HspConsole.Write("字句解析中...");
				lex = new LexicalAnalyzer(dictionary);
				stream = lex.Analyze(data);
				data.LoadEnd();
				global::KttK.HspDecompiler.HspConsole.Write("構文解析中...");
				synt = new SyntacticAnalyzer();
				lines = synt.Analyze(stream, data);
				global::KttK.HspDecompiler.HspConsole.Write("出力ファイル作成中...");
				foreach (LogicalLine line in lines)
				{
					if (line.Visible)
					{
						string str = new string('\t',line.TabCount);
						stringLines.Add(str + line.ToString());
					}
				}
			}
			catch (SystemException e)
			{
			    throw new HspDecoderException("AxData", "想定外のエラー", e);
			}
			return stringLines;
		}

		/*private AxData preprocessorAnalyze()
		{
		}*/


	}
}
