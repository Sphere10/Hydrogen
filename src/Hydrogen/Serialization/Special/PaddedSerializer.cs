// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT NON-AI software license, see the accompanying file
// LICENSE or visit https://sphere10.com/legal/NON-AI-MIT.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;

namespace Hydrogen;

internal class PaddedSerializer<TItem> : ConstantSizeItemSerializerBase<TItem> {
	private readonly IItemSerializer<TItem> _dynamicSerializer;
	public PaddedSerializer(long fixedSize, IItemSerializer<TItem> dynamicSerializer)
		: base(fixedSize, dynamicSerializer.SupportsNull) {
		Guard.ArgumentNotNull(dynamicSerializer, nameof(dynamicSerializer));
		_dynamicSerializer = dynamicSerializer;
	}

	public override void Serialize(TItem item, EndianBinaryWriter writer, SerializationContext context) {
		const int StackAllocPaddingThreshold = 2048;

		// Check
		var expectedSize = _dynamicSerializer.CalculateSize(context, item);
		Guard.Ensure(expectedSize <= ConstantSize, $"Item is too large to fit in {ConstantSize} bytes");

		// Write item
		var itemBytes = _dynamicSerializer.SerializeReturnSize(item, writer);
		Guard.Ensure(itemBytes == expectedSize, $"Overflow detected. Expected to serialize {expectedSize} bytes, but serialized {itemBytes} bytes");

		// Write padding
		var paddingLength = ConstantSize - itemBytes;
		if (paddingLength > 0) {
			Span<byte> padding = paddingLength <= StackAllocPaddingThreshold ? stackalloc byte[unchecked((int)paddingLength)] : new byte[paddingLength];
			writer.Write(padding);
		}
	}

	public override TItem Deserialize(EndianBinaryReader reader, SerializationContext context) {
		// read size descriptor
		var pos = reader.BaseStream.Position;

		// read item
		var item = _dynamicSerializer.Deserialize(reader, context);

		var bytesRead = reader.BaseStream.Position - pos;
		Guard.Ensure(bytesRead <= ConstantSize, $"Item was larger than {ConstantSize} bytes");

		// read padding
		var padding = ConstantSize - bytesRead;
		if (padding > 0) {
			var _ = reader.ReadBytes(padding);
		}
		return item;
	}
}
