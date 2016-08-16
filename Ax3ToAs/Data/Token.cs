using System;
using System.Collections.Generic;
using System.Text;

namespace KttK.HspDecompiler.Ax3ToAs.Data
{
	abstract class CodeToken
	{
		internal abstract int TokenOffset
		{
			get;
		}

		public abstract override string ToString();
		internal virtual void CheckLabel() { }
		internal virtual bool CheckRpn(){return true;}
	}
}
