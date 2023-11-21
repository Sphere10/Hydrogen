// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.


using System;

namespace Hydrogen;

public class NullableSerializer<T> : ItemSerializerBase<T?> where T : struct  {
	private readonly byte[] _padding;
	private readonly IItemSerializer<T> _valueSerializer;

	public NullableSerializer(IItemSerializer<T> valueSerializer, bool preserveConstantSize = false) {
		Guard.ArgumentNotNull(valueSerializer, nameof(valueSerializer));
		Guard.Argument(!valueSerializer.SupportsNull, nameof(valueSerializer), "Serializer already supports null.");
		_valueSerializer = valueSerializer;
		var isConstantSize = _valueSerializer.IsConstantSize && preserveConstantSize;
		IsConstantSize =  isConstantSize;
		ConstantSize = isConstantSize ? sizeof(bool) + _valueSerializer.ConstantSize : -1;
		_padding = isConstantSize ? new byte[_valueSerializer.ConstantSize] : Array.Empty<byte>();
	}

	public static NullableSerializer<T> Instance { get; } = new(PrimitiveSerializer<T>.Instance);

	public override bool SupportsNull => true; 

	public override bool IsConstantSize { get; }

	public override long ConstantSize { get; } 

	public override long CalculateSize(SerializationContext context, T? item) {
		if (IsConstantSize)
			return ConstantSize;
		
		long size = sizeof(bool);
		if (item.HasValue)
			size += _valueSerializer.CalculateSize(context, item);

		return size;
	}

	public override void Serialize(T? item, EndianBinaryWriter writer, SerializationContext context) {
		if (item.HasValue) {
			writer.Write(true);
			_valueSerializer.Serialize(item, writer, context);
		} else {
			writer.Write(false);
			if (IsConstantSize)
				writer.Write(_padding);
		}
	}

	public override T? Deserialize(EndianBinaryReader reader, SerializationContext context) {
		var hasValue = reader.ReadBoolean();
		var result = hasValue ? new T?(_valueSerializer.Deserialize(reader, context)) : null;
		if (IsConstantSize)
			reader.ReadBytes(_valueSerializer.ConstantSize);
		return result;
	}
}
