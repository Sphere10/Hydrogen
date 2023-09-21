// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

namespace Hydrogen;

internal class SizeSavingSerializer<TItem> : ItemSerializerDecorator<TItem>, IAutoSizedSerializer<TItem> {
	private readonly SizeDescriptorSerializer _sizeDescriptorSerializer;

	public SizeSavingSerializer(IItemSerializer<TItem> internalSerializer, SizeDescriptorStrategy sizeDescriptorStrategy)
		: base(internalSerializer) {
		_sizeDescriptorSerializer = new SizeDescriptorSerializer(sizeDescriptorStrategy);
	}

	public override bool IsConstantLength => _sizeDescriptorSerializer.IsConstantLength && base.IsConstantLength;

	public override long CalculateSize(TItem item) {
		var itemSize = base.CalculateSize(item);
		return _sizeDescriptorSerializer.CalculateSize(itemSize) + itemSize;
	}

	public override void SerializeInternal(TItem item, EndianBinaryWriter writer) {
		var itemSize = base.CalculateSize(item);
		_sizeDescriptorSerializer.SerializeInternal(itemSize, writer);
		base.SerializeInternal(item, writer);
	}

	public override TItem DeserializeInternal(long byteSize, EndianBinaryReader reader) {
		// Caller trying to read item passing explicit size, so check length matches
		var itemSize = _sizeDescriptorSerializer.Deserialize(reader);
		var sizeDescriptorSize = _sizeDescriptorSerializer.CalculateSize(itemSize);
		Guard.Ensure(byteSize == sizeDescriptorSize + itemSize, "Read overflow");
		return base.DeserializeInternal(itemSize, reader);
	}

	public TItem Deserialize(EndianBinaryReader reader) {
		var itemSize = _sizeDescriptorSerializer.Deserialize(reader);
		return base.DeserializeInternal(itemSize, reader);
	}
}
