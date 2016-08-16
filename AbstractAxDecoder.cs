using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace KttK.HspDecompiler
{
	internal abstract class AbstractAxDecoder
	{
		protected AbstractAxDecoder() { }
		public abstract List<string> Decode(BinaryReader reader);
	}
}
