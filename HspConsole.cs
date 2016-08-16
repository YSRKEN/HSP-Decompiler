using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Windows.Forms;

namespace KttK.HspDecompiler
{

	internal static class HspConsole
	{
		private static int startTime = Environment.TickCount;
		private static string newLine = null;

		private static StreamWriter logStream;
		internal static void Initialize()
		{
		}


		internal static void ExceptionHandlingClose(Exception e)
		{
			WriteLog(e.GetType().ToString() +":" + e.Source + ":" + e.Message );
			WriteLog("abort");
			if (logStream != null)
			{
				logStream.Close();
				MessageBox.Show("例外を適切にcatchできませんでした。詳細をlog.datに出力し終了します。", "致命的なエラー");
			}
			logStream = null;
		}

		internal static void Close()
		{
			WriteLog("exit");
			if (logStream != null)
				logStream.Close();
			logStream = null;
		}

		internal static string NewLine
		{
			get { return HspConsole.newLine; }
		}

		internal static void DecompStart(string filePath)
		{
			startTime = Environment.TickCount;
			newLine = null;
			tabCount = 0;
			warnings.Clear();

			StringBuilder builder = new StringBuilder();
			builder.Append("経過(ms)");
			builder.Append(':');
			builder.Append("処理内容");
			newLine = builder.ToString();
			Flush();


			WriteLog("decompile " + Path.GetFileName(filePath));
		}

		private static int tabCount = 0;
		internal static void Write(string line)
		{
			int time = Environment.TickCount - startTime;
			StringBuilder builder = new StringBuilder();
			builder.Append(time.ToString("D08"));
			builder.Append(':');
			builder.Append(' ', tabCount*2);
			builder.Append(line);
			newLine = builder.ToString();
			Flush();
		}

		internal static void StartParagraph()
		{
			tabCount++;
		}

		internal static void EndParagraph()
		{
			tabCount--;
		}

		internal static void BreakParagraph()
		{
			tabCount = 0;
		}
		internal static void Warning(string errMsg, int lineNo)
		{
			string warning = string.Format("{0:D06}行:{1}", lineNo, errMsg);
			warnings.Add(warning);
			//newLine = warning;
			//Flush();
		}
		internal static void Warning(string errMsg)
		{
			warnings.Add(errMsg);
			//newLine = warning;
			//Flush();
		}


		private static List<string> warnings = new List<string>();

		internal static List<string> Warnings
		{
			get { return HspConsole.warnings; }
		}
		internal static void FatalError(string line)
		{
			int time = Environment.TickCount - startTime;
			StringBuilder builder = new StringBuilder();
			builder.Append(time.ToString("D08"));
			builder.Append(':');
			builder.Append(line);
			newLine = builder.ToString();
			Flush();
		}

		internal static void FatalError(Exception e)
		{
			
			int time = Environment.TickCount - startTime;
			StringBuilder builder = new StringBuilder();
			StringBuilder logBuilder = new StringBuilder();
			builder.Append(time.ToString("D08"));
			builder.Append(':');
			if (e is SystemException)
			{
				logBuilder.Append(e.GetType().ToString());
				logBuilder.Append(':');
				logBuilder.Append(e.Source);
				logBuilder.Append(':');
			}
			else
			{
				logBuilder.Append("HspDecoderException:");
			}
			logBuilder.Append(e.Message);
			builder.Append(logBuilder);
			newLine = builder.ToString();
			Flush();
			WriteLog(logBuilder.ToString());
		}

		internal delegate void WriteDown();
		internal static event WriteDown Flush;
		private static void HspConsole_Flush()
		{
			WriteLog(newLine);
		}
		internal static void WriteLog(string line)
		{
			if (string.IsNullOrEmpty(line))
				return;
			if (logStream == null)
				return;
			logStream.WriteLine(DateTime.Now.ToString() + ":" + line);
			logStream.Flush();
		}
	}
}
