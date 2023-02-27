﻿using System;

namespace Hydrogen;

public class NullableStructSerializer<T> : ItemSerializer<T?> where T : struct {
	private readonly IItemSerializer<T> _underlyingSerializer;

	public NullableStructSerializer(IItemSerializer<T> valueSerializer) {
		_underlyingSerializer = valueSerializer;
	}

	public override int CalculateSize(T? item) 
		=> 1 + (item != null ? _underlyingSerializer.CalculateSize(item.Value) : 0);

	public override bool TrySerialize(T? item, EndianBinaryWriter writer, out int bytesWritten) {
		var isNull = item is null;
		writer.Write(!isNull);
		bytesWritten = 1;
		if (isNull) 
			return true;
		var result = _underlyingSerializer.TrySerialize((T)item, writer, out bytesWritten);
		bytesWritten += bytesWritten;
		return result;
	}

	public override bool TryDeserialize(int byteSize, EndianBinaryReader reader, out T? item) {
		item = default;
		var isNull = reader.ReadBoolean();
		if (isNull)  
			return true;
		
		if (!_underlyingSerializer.TryDeserialize(byteSize - 1, reader, out var value)) 
			return false;
		
		item = value;
		return  true;
	}

}