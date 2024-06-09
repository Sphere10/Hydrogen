// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT NON-AI software license, see the accompanying file
// LICENSE or visit https://sphere10.com/legal/NON-AI-MIT.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

namespace Hydrogen;

public class ArraySerializer<T> : ItemSerializerBase<T[]> {
	private readonly SizeDescriptorSerializer _sizeSerializer;
	private readonly IItemSerializer<T> _valueSerializer;

	public ArraySerializer(IItemSerializer<T> valueSerializer, SizeDescriptorStrategy sizeDescriptorStrategy = SizeDescriptorStrategy.UseCVarInt) {
		Guard.ArgumentNotNull(valueSerializer, nameof(valueSerializer));
		_valueSerializer = valueSerializer; // support null values
		_sizeSerializer = new SizeDescriptorSerializer(sizeDescriptorStrategy);
	}

	public override long CalculateSize(SerializationContext context, T[] item)
		=> _sizeSerializer.CalculateSize(context, item.Length) + _valueSerializer.CalculateTotalSize(context, item, false, out _);

	public override void Serialize(T[] item, EndianBinaryWriter writer, SerializationContext context) {
		_sizeSerializer.Serialize(item.Length, writer, context);
		foreach (var element in item)
			_valueSerializer.Serialize(element, writer, context);
	}

	public override T[] Deserialize(EndianBinaryReader reader, SerializationContext context) {
		var arraySize = _sizeSerializer.Deserialize(reader, context);
		var array = new T[arraySize];
		context.SetDeserializingItem(array);
		for (var i = 0; i < arraySize; i++) {
			array[i] = _valueSerializer.Deserialize(reader, context);
		}
		return array;
	}
}