using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Runtime.Serialization;
using System.Diagnostics;
using Hydrogen;

namespace Hydrogen.DApp.Core {

	public static class EndianBinaryWriterExtensions {
		public static void WriteBuffer(this EndianBinaryWriter writer, byte[] buffer) {
			Guard.ArgumentNotNull(buffer, nameof(buffer));
			Guard.ArgumentInRange(buffer.Length, 0, UInt32.MaxValue, nameof(buffer.Length));
			writer.Write(buffer.Length);
			writer.Write(buffer);
		}
	} 
}
