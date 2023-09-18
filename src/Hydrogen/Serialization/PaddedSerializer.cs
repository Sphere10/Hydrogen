// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;

namespace Hydrogen;

public class PaddedSerializer<TItem> : ConstantLengthItemSerializerBase<TItem> {
	private readonly IItemSerializer<TItem> _dynamicSerializer;
	private readonly SizeDescriptorSerializer _sizeDescriptorSerializer;
	public PaddedSerializer(long fixedSize, IItemSerializer<TItem> dynamicSerializer, SizeDescriptorStrategy sizeDescriptorStrategy)
		: base(fixedSize) {
		Guard.ArgumentNotNull(dynamicSerializer, nameof(dynamicSerializer));
		_dynamicSerializer = dynamicSerializer;
		_sizeDescriptorSerializer = new SizeDescriptorSerializer(sizeDescriptorStrategy);
	}

	public override void SerializeInternal(TItem item, EndianBinaryWriter writer) {
		const int StackAllocPaddingThreshold = 2048;

		// Write size descriptor
		var expectedSize = _dynamicSerializer.CalculateSize(item);
		var sizeDescriptorBytes = _sizeDescriptorSerializer.Serialize(expectedSize, writer);
		Guard.Ensure(expectedSize + sizeDescriptorBytes <= ConstantLength, $"Item is too large to fit in {ConstantLength} bytes");

		// Write item
		var itemBytes = _dynamicSerializer.Serialize(item, writer);
		Guard.Ensure(itemBytes == expectedSize, $"Overflow detected. Expected to serialize {expectedSize} bytes, but serialized {itemBytes} bytes");

		// Write padding
		// TODO: should chunk this out
		var paddingLength = ConstantLength - (itemBytes + sizeDescriptorBytes);
	
		Span<byte> padding = paddingLength <= StackAllocPaddingThreshold ? stackalloc byte[unchecked((int)paddingLength)] : new byte[paddingLength];
		if (padding.Length > 0) {
			writer.Write(padding);
		}
	}

	public override TItem Deserialize(EndianBinaryReader reader) {
		// read size descriptor
		var itemSize = _sizeDescriptorSerializer.Deserialize(reader);
		var sizeDescriptorSize = _sizeDescriptorSerializer.CalculateSize(itemSize);
		Guard.Ensure(itemSize + sizeDescriptorSize <= ConstantLength, $"Item is too large to fit in {ConstantLength} bytes");

		// read item
		var item = _dynamicSerializer.Deserialize(itemSize, reader);

		// read padding
		var padding = ConstantLength - itemSize - sizeDescriptorSize;
		if (padding > 0) {
			var _ = reader.ReadBytes(padding);
		}
		return item;
	}
}
