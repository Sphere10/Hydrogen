// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System.Text;

namespace Hydrogen;

public class StringSerializer : ItemSerializer<string> {

	public StringSerializer()
		: this(Encoding.UTF8) {
	}

	public StringSerializer(Encoding textEncoding, SizeDescriptorStrategy sizeDescriptorStrategy = SizeDescriptorStrategy.UseCVarInt) 
		: base(sizeDescriptorStrategy) {
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

	public override long CalculateSize(string item) {
		var textByteCount = TextEncoding.GetByteCount(item);
		var sizeCount = SizeSerializer.CalculateSize(textByteCount);
		return sizeCount + textByteCount;
	}

	public override void Serialize(string item, EndianBinaryWriter writer) {
		SizeSerializer.Serialize(TextEncoding.GetByteCount(item), writer);
		var bytes = TextEncoding.GetBytes(item);
		writer.Write(bytes);
	}

	public override string Deserialize(EndianBinaryReader reader) {
		var size = SizeSerializer.Deserialize(reader);
		var bytes = reader.ReadBytes(size);
		return TextEncoding.GetString(bytes);
	}
}
