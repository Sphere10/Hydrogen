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
