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
	private readonly IItemSerializer<T> _valueSerializer;

	public BoxedNullableSerializer(IItemSerializer<T> valueSerializer, bool preserveConstantSize = false) : base(SizeDescriptorStrategy.UseCVarInt) {
		Guard.ArgumentNotNull(valueSerializer, nameof(valueSerializer));
		Guard.Argument(!valueSerializer.SupportsNull, nameof(valueSerializer), "Serializer already supports null. Boxing it here is inefficient.");
		_valueSerializer = valueSerializer;
		IsConstantSize =  _valueSerializer.IsConstantSize && preserveConstantSize;
		ConstantSize = IsConstantSize ? sizeof(bool) + _valueSerializer.ConstantSize : -1;
		_padding = IsConstantSize ? new byte[_valueSerializer.ConstantSize] : Array.Empty<byte>();
	}

	public override bool SupportsNull => false; // this doesn't support null values, only BoxedNullable<T>() with a null Value

	public override bool IsConstantSize { get; }

	public override long ConstantSize { get; } 

	public override long CalculateSize(SerializationContext context, BoxedNullable<T> item) {
		if (IsConstantSize)
			return ConstantSize;
		
		long size = sizeof(bool);
		if (item.HasValue)
			size += _valueSerializer.CalculateSize(context, item);

		return size;
	}

	public override void Serialize(BoxedNullable<T> item, EndianBinaryWriter writer, SerializationContext context) {
		if (item.HasValue) {
			writer.Write(true);
			_valueSerializer.Serialize(item, writer, context);
		} else {
			writer.Write(false);
			if (IsConstantSize)
				writer.Write(_padding);
		}
	}

	public override BoxedNullable<T> Deserialize(EndianBinaryReader reader, SerializationContext context) {
		var hasValue = reader.ReadBoolean();
		var result = hasValue ? new BoxedNullable<T>(_valueSerializer.Deserialize(reader, context)) : new BoxedNullable<T>();
		if (IsConstantSize)
			reader.ReadBytes(_valueSerializer.ConstantSize);
		return result;
	}

}
