using System;
using System.Collections.Generic;
using System.Text;
using KttK.HspDecompiler.Ax3ToAs.Data;
using KttK.HspDecompiler.Ax3ToAs.Data.Token;

namespace KttK.HspDecompiler.Ax3ToAs.Data.Line
{
	internal sealed class Command : LogicalLine
	{
		private Command() { }
		internal Command(FunctionToken function)
		{
			this.function = function;

		}


		readonly FunctionToken function = null;

		internal override bool TabIncrement
		{
			get
			{
				if ((function.Primitive.CodeExtraFlags & HspCodeExtraFlags.AddTab) == HspCodeExtraFlags.AddTab)
					return true;

				return false;
			}
		}
		internal override bool TabDecrement
		{
			get
			{
				if ((function.Primitive.CodeExtraFlags & HspCodeExtraFlags.RemoveTab) == HspCodeExtraFlags.RemoveTab)
					return true;

				return false;
			}
		}
		internal override bool HasFlagGhostGoto
		{
			get
			{
				if ((function.Primitive.CodeExtraFlags & HspCodeExtraFlags.HasGhostGoto) == HspCodeExtraFlags.HasGhostGoto)
					return true;

				return false;
			}
		}
		internal override bool HasFlagIsGhost
		{
			get
			{
				if ((function.Primitive.CodeExtraFlags & HspCodeExtraFlags.IsGhost) == HspCodeExtraFlags.IsGhost)
					return true;

				return false;
			}
		}

		internal override int TokenOffset
		{
			get { return function.TokenOffset; }
		}

		public override string ToString()
		{
			return function.ToString();
		}


		internal override void CheckLabel()
		{
			if (function != null)
				function.CheckLabel();
		}

		internal override bool CheckRpn()
		{
			if (function != null)
				return function.CheckRpn();
			return true;
		}
	}
}
