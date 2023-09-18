// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

namespace Hydrogen;

public class ArraySerializer<T> : ItemSerializer<T[]> {
	private readonly IAutoSizedSerializer<T> _valueSerializer;
	private readonly SizeDescriptorSerializer _sizeDescriptorSerializer;
	
	public ArraySerializer(IItemSerializer<T> valueSerializer, SizeDescriptorStrategy sizeDescriptorStrategy) {
		Guard.ArgumentNotNull(valueSerializer, nameof(valueSerializer));
		
		if (valueSerializer is not IAutoSizedSerializer<T> autoSizedSerializer) {
			if (valueSerializer.IsConstantLength)
				autoSizedSerializer = new ConstantLengthAutoSizedSerializer<T>(valueSerializer);
			else
				autoSizedSerializer = new AutoSizedSerializer<T>(valueSerializer, sizeDescriptorStrategy);
		}
		_valueSerializer = autoSizedSerializer;	
		_sizeDescriptorSerializer = new SizeDescriptorSerializer(sizeDescriptorStrategy);
	}

	public override long CalculateSize(T[] item) 
		=> _sizeDescriptorSerializer.CalculateSize(item.Length) + _valueSerializer.CalculateTotalSize(item, false, out _);

	public override void SerializeInternal(T[] item, EndianBinaryWriter writer) {
		_sizeDescriptorSerializer.SerializeInternal(item.Length, writer);
		foreach(var element in item)
			_valueSerializer.SerializeInternal(element, writer);
	}

	public override T[] DeserializeInternal(long byteSize, EndianBinaryReader reader) {
		var arraySize = _sizeDescriptorSerializer.Deserialize(reader);
		var array = new T[arraySize];
		for (var i = 0; i < arraySize; i++) {
			array[i] = _valueSerializer.Deserialize(reader);
		}
		return array;
	}
}