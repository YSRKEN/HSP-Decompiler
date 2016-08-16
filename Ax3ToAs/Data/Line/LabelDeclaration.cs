using System;
using System.Collections.Generic;
using System.Text;

namespace KttK.HspDecompiler.Ax3ToAs.Data.Line
{
	/// <summary>
	/// プリプロセッサ、ラベル宣言行
	/// </summary>
	internal sealed class PreprocessorDeclaration : LogicalLine
	{
		private PreprocessorDeclaration() { }
		internal PreprocessorDeclaration(Preprocessor pp)
		{
			this.pp = pp;
		}

		private readonly Preprocessor pp = null;


		internal override int TabCount
		{
			get
			{
				return 0;
			}
		}

		internal override int TokenOffset
		{
			get { return -1; }
		}

		public override string ToString()
		{
			return pp.ToString();
		}
	}
}
