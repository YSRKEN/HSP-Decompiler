using System;
using System.Collections.Generic;
using System.Text;

namespace KttK.HspDecompiler.Ax3ToAs.Data.Line
{
	/// <summary>
	/// if,else�X�R�[�v�̏I��������킷�@"}"�����̍s
	/// </summary>
	internal sealed class ScoopEnd : LogicalLine
	{
		internal override bool TabDecrement
		{
			get
			{
				return true;
			}
		}
		internal override int TokenOffset
		{
			get { return -1; }
		}

		public override string ToString()
		{
			return "}";
		}
	}
}
