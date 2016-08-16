using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.IO;
using System.Text;
namespace KttK.HspDecompiler
{
	internal static class Program
	{
		/// <summary>
		/// アプリケーションのメイン エントリ ポイントです。
		/// </summary>
		[STAThread]
		static void Main(string[] args)
		{
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			HspConsole.Initialize();
			if (!HspDecoder.Initialize())
			{
				HspConsole.Close();
				return;
			}
			string filename = null;
			if ((args != null) && (args.Length > 0))
			{
				foreach (string file in args)
				{
					if (File.Exists(file))
					{
						filename = file;
						break;
					}
				}
			}

			try
			{
				Application.Run(new deHspDialog(filename));
			}
			catch(Exception e)
			{
				try
				{
					HspConsole.ExceptionHandlingClose(e);
				}
				catch { }
				return;
			}
			HspConsole.Close();

			
		}
		
		private readonly static string exeDir = Path.GetDirectoryName(Application.ExecutablePath) + @"\";
		private readonly static string exeName = Path.GetFileName(Application.ExecutablePath);
		private readonly static System.Diagnostics.FileVersionInfo exeVer =
			System.Diagnostics.FileVersionInfo.GetVersionInfo(Application.ExecutablePath);

		internal static string ExeName
		{
			get { return Program.exeName; }
		}

		internal static System.Diagnostics.FileVersionInfo ExeVer
		{
			get { return Program.exeVer; }
		} 

		internal static string ExeDir
		{
			get { return exeDir; }
		} 


	}
}