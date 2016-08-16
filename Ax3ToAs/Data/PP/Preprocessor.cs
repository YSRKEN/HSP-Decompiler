using System;
using System.Collections.Generic;
using System.Text;

namespace KttK.HspDecompiler.Ax3ToAs.Data
{
	internal abstract class Preprocessor
	{
		protected Preprocessor() { }
		protected Preprocessor(int index)
		{
			this.index = index;
		}
		protected readonly int index;
		public abstract override string ToString();
	}
}
