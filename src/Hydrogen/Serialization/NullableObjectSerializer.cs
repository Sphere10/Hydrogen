// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;

namespace Hydrogen;

public class NullableObjectSerializer<T> : ItemSerializerDecorator<T> {

	public NullableObjectSerializer(IItemSerializer<T> valueSerializer)
		: base(valueSerializer) {
	}

	public override long CalculateSize(T item)
		=> sizeof(bool) + (item is null ? 0 : CalculateSize(item));

	public override void SerializeInternal(T item, EndianBinaryWriter writer) {
		if (item is null) {
			writer.Write(false);
		} else {
			writer.Write(true);
			base.SerializeInternal(item, writer);
		}
	}

	public override T DeserializeInternal(long byteSize, EndianBinaryReader reader) {
		if (!reader.ReadBoolean())
			return default;
		return base.DeserializeInternal(byteSize - 1, reader);
	}

	public T Deserialize(EndianBinaryReader reader) {
		if (Internal is not AutoSizedSerializer<T> autoSerializer)
			throw new InvalidOperationException("This method can only be used with AutoSizedSerializer");

		if (!reader.ReadBoolean())
			return default;

		return autoSerializer.Deserialize(reader);
	}

}
