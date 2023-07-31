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
	private readonly Action<T, EndianBinaryWriter> _serializer;
	private readonly Func<long, EndianBinaryReader, T> _deserializer;

	public ActionItemSerializer(Func<T, long> sizer, Action<T, EndianBinaryWriter> serializer, Func<long, EndianBinaryReader, T> deserializer)
		: base(sizer) {
		Guard.ArgumentNotNull(serializer, nameof(serializer));
		Guard.ArgumentNotNull(deserializer, nameof(deserializer));
		_serializer = serializer;
		_deserializer = deserializer;
	}

	public void SerializeInternal(T item, EndianBinaryWriter writer) 
		=> _serializer(item, writer);

	public T DeserializeInternal(long byteSize, EndianBinaryReader reader) 
		=> _deserializer(byteSize, reader);

}
