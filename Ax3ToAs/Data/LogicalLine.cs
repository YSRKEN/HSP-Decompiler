using System;
using System.Collections.Generic;
using System.Text;

namespace KttK.HspDecompiler.Ax3ToAs.Data
{
	//internal enum LineType
	//{
	//    NONE = 0,
	//    Function = 1,
	//    Assignment = 2,
	//    IfStatement = 3,
	//    StatementEnd = 4,
	//    Preprocessor = 5,
	//    Label = 6,
	//    Unknown = 7,
	//}

	abstract class  LogicalLine
	{

		internal abstract int TokenOffset
		{
			get;
		}

		protected int tabCount = 0;

		internal virtual int TabCount
		{
			get { return tabCount; }
			set { tabCount = value; }
		}

		protected List<string> errorMes = new List<string>();
		internal List<string> GetErrorMes() { return errorMes; }
		internal void AddError(string error) { errorMes.Add(error); }
		public override abstract string ToString();

		private bool visible = true;
		internal bool Visible
		{
			get
			{
				return visible;
			}
			set
			{
				visible = value;
			}
		}

		internal virtual bool TabIncrement { get { return false; } }
		internal virtual bool TabDecrement { get { return false; } }
		internal virtual bool HasFlagGhostGoto { get { return false; } }
		internal virtual bool HasFlagIsGhost { get { return false; } }

		internal virtual void CheckLabel() { }
		internal virtual bool CheckRpn() { return true; }
	}
}
