// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Diagnostics;

namespace Hydrogen;

public class PaddedSerializer<TItem> : StaticSizeItemSerializerBase<TItem> {
	private readonly IItemSerializer<TItem> _dynamicSerializer;
	private readonly SizeDescriptorSerializer _sizeDescriptorSerializer;
	public PaddedSerializer(long fixedSize, IItemSerializer<TItem> dynamicSerializer, SizeDescriptorStrategy sizeDescriptorStrategy)
		: base(fixedSize) {
		Guard.ArgumentNotNull(dynamicSerializer, nameof(dynamicSerializer));
		_dynamicSerializer = dynamicSerializer;
		_sizeDescriptorSerializer = new SizeDescriptorSerializer(sizeDescriptorStrategy);
	}

	public override bool TrySerialize(TItem item, EndianBinaryWriter writer) {
		const int StackAllocPaddingThreshold = 2048;

		var bytesWritten = 0L;
		var expectedSize = _dynamicSerializer.CalculateSize(item);
		var sizeDescriptorSize = _sizeDescriptorSerializer.CalculateSize(expectedSize);
		if (expectedSize + sizeDescriptorSize > StaticSize) {
			throw new InvalidOperationException($"Item is too large to fit in {StaticSize} bytes");
			return false;
		}

		if (!_sizeDescriptorSerializer.TrySerialize(expectedSize, writer, out var sizeDescriptorBytes))
			return false;


		bytesWritten += sizeDescriptorBytes;

		if (!_dynamicSerializer.TrySerialize(item, writer, out var itemBytes)) {
			throw new InvalidOperationException($"Failed to serialize item size descriptor");
			return false;
		}

		if (itemBytes != expectedSize) {
			throw new InvalidOperationException($"Item size mismatch. Expected to serialize {expectedSize} bytes, but serialized {itemBytes} bytes");
			return false;
		}

		bytesWritten += itemBytes;
		Debug.Assert(bytesWritten <= StaticSize);

		var remaining = StaticSize - bytesWritten;

		// TODO: should chunk this out
		Span<byte> padding = remaining <= StackAllocPaddingThreshold ? stackalloc byte[unchecked((int)remaining)] : new byte[remaining];
		if (padding.Length > 0) {
			writer.Write(padding);
			bytesWritten += remaining;
		}
		Debug.Assert(bytesWritten == StaticSize);
		return true;
	}

	public override bool TryDeserialize(EndianBinaryReader reader, out TItem item) {
		item = default;
		if (!_sizeDescriptorSerializer.TryDeserialize(reader, out var itemSize)) {
			throw new InvalidOperationException($"Failed to deserialize item size descriptor");
			return false;
		}
		var sizeDescriptorSize = _sizeDescriptorSerializer.CalculateSize(itemSize);
		if (itemSize + sizeDescriptorSize > StaticSize) {
			throw new InvalidOperationException($"Item size too large");
			return false;
		}

		if (!_dynamicSerializer.TryDeserialize(itemSize, reader, out item)) {
			return false;
		}

		var padding = StaticSize - itemSize - sizeDescriptorSize;
		if (padding > 0) {
			var _ = reader.ReadBytes(padding);
		}
		return true;
	}
}
