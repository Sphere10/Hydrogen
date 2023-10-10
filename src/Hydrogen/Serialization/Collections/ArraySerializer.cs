// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

namespace Hydrogen;

public class ArraySerializer<T> : ItemSerializer<T[]> {
	private readonly IItemSerializer<T> _valueSerializer;

	public ArraySerializer(IItemSerializer<T> valueSerializer, SizeDescriptorStrategy sizeDescriptorStrategy = SizeDescriptorStrategy.UseCVarInt)
	: base(sizeDescriptorStrategy) {
		Guard.ArgumentNotNull(valueSerializer, nameof(valueSerializer));
		_valueSerializer = valueSerializer;
	}

	public override long CalculateSize(SerializationContext context, T[] item)
		=> SizeSerializer.CalculateSize(context, item.Length) + _valueSerializer.CalculateTotalSize(item, false, out _);

	public override void Serialize(T[] item, EndianBinaryWriter writer, SerializationContext context) {
		SizeSerializer.Serialize(item.Length, writer, context);
		foreach (var element in item)
			_valueSerializer.Serialize(element, writer, context);
	}

	public override T[] Deserialize(EndianBinaryReader reader, SerializationContext context) {
		var arraySize = SizeSerializer.Deserialize(reader, context);
		var array = new T[arraySize];
		for (var i = 0; i < arraySize; i++) {
			array[i] = _valueSerializer.Deserialize(reader, context);
		}
		return array;
	}
}