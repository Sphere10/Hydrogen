// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
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

	public static StringSerializer UTF8 { get; } = new(Encoding.UTF8);

	public static StringSerializer ASCII { get; } = new(Encoding.ASCII);

	public static StringSerializer UTF7 { get; } = new(Encoding.UTF7);

	public static StringSerializer BigEndianUnicode { get; } = new(Encoding.BigEndianUnicode);

	public static StringSerializer Default { get; } = new(Encoding.Default);

	public static StringSerializer UTF32 { get; } = new(Encoding.UTF32);


	public Encoding TextEncoding { get; }

	public override long CalculateSize(string item) => !string.IsNullOrEmpty(item) ? TextEncoding.GetByteCount(item) : 0;

	public override void SerializeInternal(string item, EndianBinaryWriter writer) {
		var bytes = !string.IsNullOrEmpty(item) ? TextEncoding.GetBytes(item) : System.Array.Empty<byte>();
		Debug.Assert(bytes.Length == CalculateSize(item));
		ByteArraySerializer.Instance.SerializeInternal(bytes, writer);
	}

	public override string DeserializeInternal(long byteSize, EndianBinaryReader reader) {
		if (byteSize == 0)
			return string.Empty;

		var bytes = ByteArraySerializer.Instance.DeserializeInternal(byteSize, reader);
		return TextEncoding.GetString(bytes);
	}
}
