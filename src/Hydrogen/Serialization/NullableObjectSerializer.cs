// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.ComponentModel;

namespace Hydrogen;

public class NullableObjectSerializer<T> : ItemSerializerDecorator<T> {
	private readonly byte[] _padding;

	public NullableObjectSerializer(IItemSerializer<T> valueSerializer)
		: base(valueSerializer) {
		_padding = base.IsStaticSize ? new byte[base.StaticSize] : Array.Empty<byte>();
	}

	public override long StaticSize => sizeof(bool) + base.StaticSize;

	public override long CalculateSize(T item) {
		long size = sizeof(bool);
		 
		if (base.IsStaticSize)
			size += base.StaticSize;
		else if (item is not null)
			size += base.CalculateSize(item);

		return size;
	}

	public override void SerializeInternal(T item, EndianBinaryWriter writer) {
		if (item is not null) {
			writer.Write(true);
			base.SerializeInternal(item, writer);
		} else {
			writer.Write(false);
			if (base.IsStaticSize)
				writer.Write(_padding);
		}
	}

	public override T DeserializeInternal(long byteSize, EndianBinaryReader reader) {
		var hasValue = reader.ReadBoolean();
		if (hasValue) {
			return base.DeserializeInternal(byteSize - sizeof(bool), reader);
		} else if (base.IsStaticSize)
			reader.ReadBytes(base.StaticSize);
		return default(T);
	}


	public T Deserialize(EndianBinaryReader reader) {
		if (Internal is AutoSizedSerializer<T> autoSerializer) {
			if (!reader.ReadBoolean())
				return default;
			return autoSerializer.Deserialize(reader);
		}
		
		if (Internal.IsStaticSize) {
			return DeserializeInternal(StaticSize, reader);
		}

		throw new InvalidOperationException($"This method can only be used with {nameof(AutoSizedSerializer<T>)} or a statically sized serializer");
	}

}
