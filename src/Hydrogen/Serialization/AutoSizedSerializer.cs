// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;

namespace Hydrogen;

public class AutoSizedSerializer<TItem> : ItemSerializerDecorator<TItem> {
	private readonly SizeDescriptorSerializer _sizeDescriptorSerializer;

	public AutoSizedSerializer(IItemSerializer<TItem> internalSerializer, SizeDescriptorStrategy sizeDescriptorStrategy)
		: base(internalSerializer) {
		_sizeDescriptorSerializer = new SizeDescriptorSerializer(sizeDescriptorStrategy);
	}

	public override bool IsStaticSize => _sizeDescriptorSerializer.IsStaticSize && base.IsStaticSize;

	public override long CalculateSize(TItem item) {
		var itemSize = base.CalculateSize(item);
		return _sizeDescriptorSerializer.CalculateSize(itemSize) + itemSize;
	}

	public bool TrySerialize(TItem item, EndianBinaryWriter writer)
		=> TrySerialize(item, writer, out _);

	public override bool TrySerialize(TItem item, EndianBinaryWriter writer, out long bytesWritten) {
		bytesWritten = 0;
		var itemSize = item != null ? base.CalculateSize(item) : 0;
		if (!_sizeDescriptorSerializer.TrySerialize(itemSize, writer, out var sizeDescriptorBytesWritten))
			return false;

		if (!base.TrySerialize(item, writer, out var itemBytesWritten))
			return false;

		bytesWritten = sizeDescriptorBytesWritten + itemBytesWritten;
		return true;
	}

	public override bool TryDeserialize(long byteSize, EndianBinaryReader reader, out TItem item) {
		// Caller trying to read item passing explicit size, so check length matches
		item = default;
		if (!_sizeDescriptorSerializer.TryDeserialize(reader, out var itemSize)) {
			return false;
		}

		var sizeDescriptorSize = _sizeDescriptorSerializer.CalculateSize(itemSize);

		Guard.ArgumentEquals(byteSize, sizeDescriptorSize + itemSize, nameof(byteSize), "Read overflow");

		return base.TryDeserialize(itemSize, reader, out item);
	}

	public bool TryDeserialize(EndianBinaryReader reader, out TItem item) {
		// Caller trying to read item passing explicit size, so check length matches
		item = default;
		if (!_sizeDescriptorSerializer.TryDeserialize(reader, out var itemSize)) {
			return false;
		}

		return base.TryDeserialize(itemSize, reader, out item);
	}

	public TItem Deserialize(EndianBinaryReader reader) {
		if (!TryDeserialize(reader, out var item))
			throw new InvalidOperationException($"Unable to deserialize object");
		return item;
	}

}
