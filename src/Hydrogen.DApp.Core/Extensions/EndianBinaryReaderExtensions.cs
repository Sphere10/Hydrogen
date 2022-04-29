using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Runtime.Serialization;
using System.Diagnostics;
using Sphere10.Framework;

namespace Sphere10.Hydrogen.Core {

	public static class EndianBinaryReaderExtensions {
		public static byte[] ReadBuffer(this EndianBinaryReader reader) {
			var len = reader.ReadUInt32();
			if (len > Int32.MaxValue)
				throw new NotSupportedException();
			return reader.ReadBytes((int)len);
		}
	} 
}
