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
	private readonly Func<EndianBinaryReader, T> _deserializer;

	public ActionItemSerializer(Func<T, long> sizer, Action<T, EndianBinaryWriter> serializer, Func<EndianBinaryReader, T> deserializer, bool supportsNull = false)
		: base(sizer, supportsNull) {
		Guard.ArgumentNotNull(serializer, nameof(serializer));
		Guard.ArgumentNotNull(deserializer, nameof(deserializer));
		_serializer = serializer;
		_deserializer = deserializer;
	}

	public void Serialize(T item, EndianBinaryWriter writer, SerializationContext context) 
		=> _serializer(item, writer);

	public T Deserialize(EndianBinaryReader reader, SerializationContext context) 
		=> _deserializer(reader);

}
