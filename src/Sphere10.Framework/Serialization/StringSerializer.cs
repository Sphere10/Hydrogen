using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using Sphere10.Framework.Values;
using Tools;

namespace Sphere10.Framework {

	public class StringSerializer : ItemSerializer<string> {
	
		public StringSerializer(Encoding textEncoding) {
			Guard.ArgumentNotNull(textEncoding, nameof(textEncoding));
			TextEncoding = textEncoding;
		}

		public Encoding TextEncoding { get; }

		public override int CalculateSize(string item) => TextEncoding.GetByteCount(item);

		public override bool TrySerialize(string item, EndianBinaryWriter writer, out int bytesWritten) {
			var bytes = TextEncoding.GetBytes(item);
			Debug.Assert(bytes.Length == CalculateSize(item));
			writer.Write(bytes);
			bytesWritten = bytes.Length;
			return true;
		}

		public override bool TryDeserialize(int byteSize, EndianBinaryReader reader, out string item) {
			var bytes = reader.ReadBytes(byteSize);
			item = TextEncoding.GetString(bytes);
			return true;
		}
	}
}
