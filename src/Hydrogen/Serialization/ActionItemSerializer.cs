// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;

namespace Hydrogen;

public class ActionItemSerializer<T> : ActionItemSizer<T>, IItemSerializer<T> {
	private readonly Func<T, EndianBinaryWriter, long> _serializer;
	private readonly Func<long, EndianBinaryReader, T> _deserializer;

	public ActionItemSerializer(Func<T, long> sizer, Func<T, EndianBinaryWriter, long> serializer, Func<long, EndianBinaryReader, T> deserializer)
		: base(sizer) {
		Guard.ArgumentNotNull(serializer, nameof(serializer));
		Guard.ArgumentNotNull(deserializer, nameof(deserializer));
		_serializer = serializer;
		_deserializer = deserializer;
	}

	public bool TrySerialize(T item, EndianBinaryWriter writer, out long bytesWritten) {
		bytesWritten = _serializer(item, writer);
		return true;
	}

	public bool TryDeserialize(long byteSize, EndianBinaryReader reader, out T item) {
		item = _deserializer(byteSize, reader);
		return true;
	}
}
