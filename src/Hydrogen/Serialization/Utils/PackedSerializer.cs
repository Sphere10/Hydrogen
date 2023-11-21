// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System.Collections.Generic;

namespace Hydrogen;

internal class PackedSerializer : IItemSerializer<object> {
	private readonly IItemSerializer _serializer;
	
	public PackedSerializer(IItemSerializer serializer) {
		Guard.ArgumentNotNull(serializer, nameof(serializer));
		_serializer = serializer;
	}

	public bool SupportsNull => _serializer.SupportsNull;

	public bool IsConstantSize => _serializer.IsConstantSize;

	public long ConstantSize => _serializer.ConstantSize;

	public long CalculateTotalSize(SerializationContext context, IEnumerable<object> items, bool calculateIndividualItems, out long[] itemSizes) 
		=> _serializer.CalculateTotalSize(context, items, calculateIndividualItems, out itemSizes);

	public long CalculateSize(SerializationContext context, object item) 
		=> _serializer.CalculateSize(context, item);

	public void Serialize(object item, EndianBinaryWriter writer, SerializationContext context) 
		=> _serializer.Serialize(item, writer, context);

	public object Deserialize(EndianBinaryReader reader, SerializationContext context) 
		=> _serializer.Deserialize(reader, context);

	public static PackedSerializer Pack(IItemSerializer serializer) {
		Guard.ArgumentNotNull(serializer, nameof(serializer));
		return new PackedSerializer(serializer);
	}

	public IItemSerializer<TItem> Unpack<TItem>() {
		var unpacked = _serializer as IItemSerializer<TItem>;
		Guard.Ensure(unpacked != null, $"Cannot unpack {_serializer.GetType().Name} as is not an {nameof(IItemSerializer<TItem>)}");
		return unpacked;
	}

}
