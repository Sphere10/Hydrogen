using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using Tools;

namespace Hydrogen;

public class StringSerializer : ItemSerializer<string> {

	public StringSerializer()
		: this(Encoding.UTF8) {
	}

	public StringSerializer(Encoding textEncoding) {
		Guard.ArgumentNotNull(textEncoding, nameof(textEncoding));
		TextEncoding = textEncoding;
	}

	public Encoding TextEncoding { get; }

	public override int CalculateSize(string item) => item != null ? TextEncoding.GetByteCount(item) : 0;

	public override bool TrySerialize(string item, EndianBinaryWriter writer, out int bytesWritten) {
		var bytes = item != null ? TextEncoding.GetBytes(item) : System.Array.Empty<byte>();
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


