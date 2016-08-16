using System;
using System.Collections.Generic;
using System.Text;

namespace KttK.HspDecompiler.Ax3ToAs.Data.Analyzer
{
	/// <summary>
	/// LogicalLine��͒��̗�O
	/// �����Ă悢�̂͂Q��Factory�̒�����
	/// �󂯎~�߂���̂�LogicalLine��internal���\�b�h����
	/// </summary>
	class HspLogicalLineException : ApplicationException
	{
		internal HspLogicalLineException()
			: base()
		{
		}

		internal HspLogicalLineException(string message)
			: base(message)
		{
		}

		internal HspLogicalLineException(string source, string message)
			: base(message)
		{
			this.Source = source;
		}

		internal HspLogicalLineException(string message, Exception e)
			: base(message, e)
		{
		}

		internal HspLogicalLineException(string source, string message, Exception e)
			: base(message, e)
		{
			this.Source = source;
		}
	}
}
