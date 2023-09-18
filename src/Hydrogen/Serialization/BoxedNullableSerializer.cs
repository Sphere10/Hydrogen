// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;

namespace Hydrogen;

internal sealed class BoxedNullableSerializer<T> : ItemSerializer<BoxedNullable<T>> {
	private readonly byte[] _padding;
	private readonly bool _preserveConstantLength;
	private readonly IItemSerializer<T> _valueSerializer;
	private readonly bool _isConstantLength;

	public BoxedNullableSerializer(IItemSerializer<T> valueSerializer, bool preserveConstantLength = false) {
		Guard.ArgumentNotNull(valueSerializer, nameof(valueSerializer));
		_valueSerializer = valueSerializer;
		_preserveConstantLength = preserveConstantLength;
		_padding = IsConstantLength ? new byte[_valueSerializer.ConstantLength] : Array.Empty<byte>();
		_isConstantLength =  _valueSerializer.IsConstantLength && _preserveConstantLength;
	}

	public override bool IsConstantLength => _isConstantLength;

	public override long ConstantLength => _isConstantLength ? sizeof(bool) + _valueSerializer.ConstantLength : -1;

	public override long CalculateSize(BoxedNullable<T> item) {
		long size = sizeof(bool);
		 
		if (_isConstantLength)
			size += _valueSerializer.ConstantLength;
		else if (item.HasValue)
			size += _valueSerializer.CalculateSize(item);

		return size;
	}

	public override void SerializeInternal(BoxedNullable<T> item, EndianBinaryWriter writer) {
		if (item.HasValue) {
			writer.Write(true);
			_valueSerializer.SerializeInternal(item, writer);
		} else {
			writer.Write(false);
			if (_isConstantLength)
				writer.Write(_padding);
		}
	}

	public override BoxedNullable<T> DeserializeInternal(long byteSize, EndianBinaryReader reader) {
		var hasValue = reader.ReadBoolean();
		if (hasValue) {
			return _valueSerializer.DeserializeInternal(byteSize - sizeof(bool), reader);
		}

		if (_isConstantLength)
			reader.ReadBytes(_valueSerializer.ConstantLength);

		return default(T);
	}


	public BoxedNullable<T> Deserialize(EndianBinaryReader reader) {
		var hasValue = reader.ReadBoolean();

		if (_valueSerializer is AutoSizedSerializer<T> autoSerializer) {
			if (hasValue) {
				return autoSerializer.Deserialize(reader);
			}
			
			if (_isConstantLength) {
				reader.ReadBytes(_valueSerializer.ConstantLength);
			} 
			return default(T);
		}
		
		if (_valueSerializer.IsConstantLength) {
			if (hasValue) {
				return DeserializeInternal(_valueSerializer.ConstantLength, reader);
			}
			
			if (_preserveConstantLength) {
				reader.ReadBytes(_valueSerializer.ConstantLength);
			}

			return default(T);
		}

		throw new InvalidOperationException($"This method can only be used with {nameof(AutoSizedSerializer<T>)} or a statically sized serializer");
	}

}
