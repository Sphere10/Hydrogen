// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT NON-AI software license, see the accompanying file
// LICENSE or visit https://sphere10.com/legal/NON-AI-MIT.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System.Text;

namespace Hydrogen;

public class StringSerializer : ItemSerializerBase<string> {
	private readonly SizeDescriptorSerializer _sizeSerializer;

	public StringSerializer()
		: this(Encoding.UTF8) {
	}

	public StringSerializer(Encoding textEncoding, SizeDescriptorStrategy sizeDescriptorStrategy = SizeDescriptorStrategy.UseCVarInt) {
		Guard.ArgumentNotNull(textEncoding, nameof(textEncoding));
		TextEncoding = textEncoding;
		_sizeSerializer = new SizeDescriptorSerializer(sizeDescriptorStrategy);
	}

	public static StringSerializer UTF8 { get; } = new(Encoding.UTF8);

	public static StringSerializer ASCII { get; } = new(Encoding.ASCII);

	public static StringSerializer UTF7 { get; } = new(Encoding.UTF7);

	public static StringSerializer Unicode { get; } = new(Encoding.Unicode);

	public static StringSerializer BigEndianUnicode { get; } = new(Encoding.BigEndianUnicode);

	public static StringSerializer Default { get; } = new(Encoding.Default);

	public static StringSerializer UTF32 { get; } = new(Encoding.UTF32);


	public Encoding TextEncoding { get; }

	public override long CalculateSize(SerializationContext context, string item) {
		var textByteCount = TextEncoding.GetByteCount(item);
		var sizeCount = _sizeSerializer.CalculateSize(context, textByteCount);
		return sizeCount + textByteCount;
	}

	public override void Serialize(string item, EndianBinaryWriter writer, SerializationContext context) {
		_sizeSerializer.Serialize(TextEncoding.GetByteCount(item), writer, context);
		var bytes = TextEncoding.GetBytes(item);
		writer.Write(bytes);
	}

	public override string Deserialize(EndianBinaryReader reader, SerializationContext context) {
		var size = _sizeSerializer.Deserialize(reader, context);
		var bytes = reader.ReadBytes(size);
		return TextEncoding.GetString(bytes);
	}
}
