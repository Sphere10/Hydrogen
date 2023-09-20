// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;

namespace Hydrogen;

public class NullableStructSerializer<T> : ItemSerializer<T?> where T : struct {
	private readonly IItemSerializer<T> _underlyingSerializer;
	private readonly byte[] _padding;

	public NullableStructSerializer(IItemSerializer<T> valueSerializer) {
		_underlyingSerializer = valueSerializer;
		_padding = base.IsConstantLength ? new byte[base.ConstantLength] : Array.Empty<byte>();
	}

	public static NullableStructSerializer<T> Instance { get; } = new(PrimitiveSerializer<T>.Instance);

	public override bool SupportsNull => true;

	public override long ConstantLength => sizeof(bool) + base.ConstantLength;

	public override long CalculateSize(T? item) {
		long size = sizeof(bool);
		 
		if (base.IsConstantLength)
			size += base.ConstantLength;
		else if (item.HasValue)
			size += _underlyingSerializer.CalculateSize(item.Value);

		return size;
	}

	public override void SerializeInternal(T? item, EndianBinaryWriter writer) {
		if (item.HasValue) {
			writer.Write(true);
			_underlyingSerializer.Serialize(item.Value, writer);
		} else {
			writer.Write(false);
			if (base.IsConstantLength)
				writer.Write(_padding);
		}
	}

	public override T? DeserializeInternal(long byteSize, EndianBinaryReader reader) {
		var hasValue = reader.ReadBoolean();
		if (hasValue) {
			return _underlyingSerializer.Deserialize(byteSize - sizeof(bool), reader);
		} else if (base.IsConstantLength)
				reader.ReadBytes(base.ConstantLength);
		return default(T?);
	}

	public T? Deserialize(EndianBinaryReader reader) {
		if (_underlyingSerializer is AutoSizedSerializer<T> autoSerializer) {
			if (!reader.ReadBoolean())
				return default;
			return autoSerializer.Deserialize(reader);
		}

		if (_underlyingSerializer.IsConstantLength)
			return DeserializeInternal(ConstantLength, reader);

		throw new InvalidOperationException($"This method can only be used with {nameof(AutoSizedSerializer<T>)} or a statically sized serializer");

	}

}
