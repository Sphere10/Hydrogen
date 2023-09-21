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
	
	private PackedSerializer(IItemSerializer serializer) {
		Guard.ArgumentNotNull(serializer, nameof(serializer));
		_serializer = serializer;
	}

	public bool SupportsNull => _serializer.SupportsNull;

	public bool IsConstantLength => _serializer.IsConstantLength;

	public long ConstantLength => _serializer.ConstantLength;

	public long CalculateTotalSize(IEnumerable<object> items, bool calculateIndividualItems, out long[] itemSizes) 
		=> _serializer.CalculateTotalSize(items, calculateIndividualItems, out itemSizes);

	public long CalculateSize(object item) 
		=> _serializer.CalculateSize(item);

	public void SerializeInternal(object item, EndianBinaryWriter writer) 
		=> _serializer.SerializeInternal(item, writer);

	public object DeserializeInternal(long byteSize, EndianBinaryReader reader) 
		=> _serializer.DeserializeInternal(byteSize, reader);

	public static PackedSerializer Pack(IItemSerializer serializer) {
		Guard.ArgumentNotNull(serializer, nameof(serializer));
		return new PackedSerializer(serializer);
		//var serializerType = serializer.GetType();
		//Guard.Argument(serializerType.IsSubtypeOfGenericType(typeof(IItemSerializer<>), out var concreteSerializedItemtype), nameof(serializer), $"Cannot pack serializer as is not an {typeof(IItemSerializer<>).ToStringCS()}");
		//var genericPackMethod = typeof(PackedSerializer).GetMethod(nameof(PackedSerializer.Pack)).MakeGenericMethod(concreteSerializedItemtype);
		//var packedSerializer = genericPackMethod.Invoke(null, new [] { serializer });
		//return (PackedSerializer)packedSerializer;
	}
	
	//public static PackedSerializer Pack<TItem>(IItemSerializer<TItem> serializer) {
	//	Guard.ArgumentNotNull(serializer, nameof(serializer));
	//	return new PackedSerializer(serializer, serializer.AsProjection(x => (object)x, x => (TItem)x));
	//}

	public IItemSerializer<TItem> Unpack<TItem>() {
		var unpacked = _serializer as IItemSerializer<TItem>;
		Guard.Ensure(unpacked != null, $"Cannot unpack {_serializer.GetType().Name} as is not an {nameof(IItemSerializer<TItem>)}");
		return unpacked;
	}

}
