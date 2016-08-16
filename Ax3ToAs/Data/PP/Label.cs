using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
namespace KttK.HspDecompiler.Ax3ToAs.Data
{
	class Label : Preprocessor, IComparable<Label>
	{
		private Label() { }
		private Label(int index):base(index) { }
		private int tokenOffset = -1;

		internal int TokenOffset
		{
			get { return tokenOffset; }
		}

		internal static Label FromBinaryReader(BinaryReader reader, AxData parent, int index)
		{
			Label ret = new Label(index);
			ret.tokenOffset = reader.ReadInt32();
			return ret;
		}





		private bool visible = false;
		internal bool Visible
		{
			get
			{
				if (function != null)
					return true;
				return visible;
			}
			set { visible = value; }
		}
		private string labelName = "*label";
		internal string LabelName
		{
			get { return labelName; }
			set { labelName = value; }
		}


		public override string ToString()
		{
			if (function != null)
				return function.ToString();
			return labelName;
		}



		#region IComparable<Label> ÉÅÉìÉo

		public int CompareTo(Label other)
		{
			int ret =  tokenOffset.CompareTo(other.tokenOffset);
			if (ret != 0)
				return ret;
			return index.CompareTo(other.index);
			
		}

		#endregion


		private Function function = null;
		internal void SetFunction(Function f)
		{
			function = f;
			visible = true;
		}
	}
}
