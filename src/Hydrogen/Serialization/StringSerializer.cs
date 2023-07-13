// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System.Diagnostics;
using System.Text;

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

	public override long CalculateSize(string item) => item != null ? TextEncoding.GetByteCount(item) : 0;

	public override bool TrySerialize(string item, EndianBinaryWriter writer, out long bytesWritten) {
		var bytes = item != null ? TextEncoding.GetBytes(item) : System.Array.Empty<byte>();
		Debug.Assert(bytes.Length == CalculateSize(item));
		writer.Write(bytes);
		bytesWritten = bytes.Length;
		return true;
	}

	public override bool TryDeserialize(long byteSize, EndianBinaryReader reader, out string item) {
		var bytes = reader.ReadBytes(byteSize);
		item = TextEncoding.GetString(bytes);
		return true;
	}
}
