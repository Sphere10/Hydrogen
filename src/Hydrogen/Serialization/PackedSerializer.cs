// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Hydrogen;

public class PackedSerializer : IItemSerializer<object> {
	private readonly object _serializer;
	private readonly IItemSerializer<object> _projectedSerializer;

	
	
	private PackedSerializer(object serializer, IItemSerializer<object> projectedSerializer) {
		Guard.ArgumentNotNull(serializer, nameof(serializer));
		Guard.ArgumentNotNull(projectedSerializer, nameof(projectedSerializer));
		Guard.Argument(serializer.GetType().IsSubtypeOfGenericType(typeof(IItemSerializer<>)), nameof(projectedSerializer), $"Must be an ItemSerializer<>");	
		Guard.Argument(projectedSerializer.GetType().IsSubtypeOfGenericType(typeof(ProjectedSerializer<,>)), nameof(projectedSerializer), "Must be an ProjectedSerializer<,>");	
		_serializer = serializer;
		_projectedSerializer = projectedSerializer;
	}

	public bool IsConstantLength => _projectedSerializer.IsConstantLength;

	public long ConstantLength => _projectedSerializer.ConstantLength;

	public long CalculateTotalSize(IEnumerable<object> items, bool calculateIndividualItems, out long[] itemSizes) 
		=> _projectedSerializer.CalculateTotalSize(items, calculateIndividualItems, out itemSizes);

	public long CalculateSize(object item) 
		=> _projectedSerializer.CalculateSize(item);

	public void SerializeInternal(object item, EndianBinaryWriter writer) 
		=> _projectedSerializer.SerializeInternal(item, writer);

	public object DeserializeInternal(long byteSize, EndianBinaryReader reader) 
		=> _projectedSerializer.DeserializeInternal(byteSize, reader);

	public static PackedSerializer Pack(object serializer) {
		Guard.ArgumentNotNull(serializer, nameof(serializer));
		var serializerType = serializer.GetType();
		Guard.Argument(serializerType.IsSubtypeOfGenericType(typeof(IItemSerializer<>), out var concreteSerializedItemtype), nameof(serializer), $"Cannot pack serializer as is not an {typeof(IItemSerializer<>).ToStringCS()}");
		var genericPackMethod = typeof(PackedSerializer).GetMethod(nameof(PackedSerializer.Pack)).MakeGenericMethod(concreteSerializedItemtype);
		var packedSerializer = genericPackMethod.Invoke(null, new [] { serializer });
		return (PackedSerializer)packedSerializer;
	}
	
	public static PackedSerializer Pack<TItem>(IItemSerializer<TItem> serializer) {
		Guard.ArgumentNotNull(serializer, nameof(serializer));
		return new PackedSerializer(serializer, serializer.AsProjection(x => (object)x, x => (TItem)x));
	}

	public IItemSerializer<TItem> Unpack<TItem>() {
		var unpacked = _serializer as IItemSerializer<TItem>;
		Guard.Ensure(unpacked != null, $"Cannot unpack {_serializer.GetType().Name} as is not an {nameof(IItemSerializer<TItem>)}");
		return unpacked;
	}

}
