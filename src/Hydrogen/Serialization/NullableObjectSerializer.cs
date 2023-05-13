// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

namespace Hydrogen;

public class NullableObjectSerializer<T> : ItemSerializerDecorator<T>  {

	public NullableObjectSerializer(IItemSerializer<T> valueSerializer) 
		: base(valueSerializer){
	}

	public override int CalculateSize(T item) 
		=> 1 + (item != null ? base.CalculateSize(item) : 0);

	public override bool TrySerialize(T item, EndianBinaryWriter writer, out int bytesWritten) {
		var isNull = item is null;
		writer.Write(!isNull);
		bytesWritten = 1;
		if (isNull) 
			return true;
		var result = base.TrySerialize(item ?? default, writer, out bytesWritten);
		bytesWritten += bytesWritten;
		return result;
	}

	public override bool TryDeserialize(int byteSize, EndianBinaryReader reader, out T item) {
		item = default;
		var isNull = reader.ReadBoolean();
		if (isNull)  
			return true;
		
		if (!base.TryDeserialize(byteSize - 1, reader, out var value)) 
			return false;
		
		item = value;
		return  true;
	}
	
}
