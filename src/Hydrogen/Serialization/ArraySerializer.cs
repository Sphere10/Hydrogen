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

	public override long CalculateSize(T[] item)
		=> SizeSerializer.CalculateSize(item.Length) + _valueSerializer.CalculateTotalSize(item, false, out _);

	public override void SerializeInternal(T[] item, EndianBinaryWriter writer) {
		SizeSerializer.SerializeInternal(item.Length, writer);
		foreach (var element in item)
			_valueSerializer.SerializeInternal(element, writer);
	}

	public override T[] DeserializeInternal(EndianBinaryReader reader) {
		var arraySize = SizeSerializer.Deserialize(reader);
		var array = new T[arraySize];
		for (var i = 0; i < arraySize; i++) {
			array[i] = _valueSerializer.Deserialize(reader);
		}
		return array;
	}
}