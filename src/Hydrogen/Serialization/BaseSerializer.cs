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
using Hydrogen.FastReflection;

namespace Hydrogen;

/// <summary>
/// A Serializer that works for base-level objects that delegates actual serialization to registered concrete-level serializers. Serialization is wrapped with the
/// type-code which permits selection of correct concrete type.
/// </summary>
/// <typeparam name="TBase">The type of object which is serialized/deserialized</typeparam>
public class BaseSerializer<TBase> : IItemSerializer<TBase> {
	private readonly SerializerFactory _factory;
	private readonly SerializerSerializer _serializerSerializer;

	public BaseSerializer(bool supportsNull = false) {
		_factory = new SerializerFactory();
		_serializerSerializer = new SerializerSerializer(_factory);
		SupportsNull = supportsNull;
	}

	public SerializerFactory Factory => _factory;

	public bool SupportsNull { get; }

	public bool IsConstantSize => false;

	public long ConstantSize => -1;

	public long CalculateTotalSize(IEnumerable<TBase> items, bool calculateIndividualItems, out long[] itemSizes) {
		var itemSizesL = new List<long>();
		var totalSize = items.Aggregate(
			0L,
			(i, o) => {
				var itemSize = _factory.GetSerializer<TBase>(o.GetType()).CalculateSize(o);
				if (calculateIndividualItems)
					itemSizesL.Add(itemSize);
				return itemSize;
			});
		itemSizes = itemSizesL.ToArray();
		return totalSize;
	}

	public long CalculateSize(TBase item)  {
		var serializer = _factory.GetSerializer<TBase>(item.GetType());
		return _serializerSerializer.CalculateSize(serializer) + serializer.CalculateSize(item);
	}

	public void Serialize(TBase item, EndianBinaryWriter writer) {
		var serializer = _factory.GetSerializer<TBase>(item.GetType());
		_serializerSerializer.Serialize(serializer, writer);
		serializer.Serialize(item, writer);
	}

	public TBase Deserialize(EndianBinaryReader reader) {
		var serializerObj = _serializerSerializer.Deserialize(reader);
		var serializer = GetTypedSerializer<TBase>(serializerObj);
		return serializer.Deserialize(reader);
	}

	public IItemSerializer<TSerializerDataType> GetTypedSerializer<TSerializerDataType>(IItemSerializer serializerObj) {
		if (serializerObj is IItemSerializer<TSerializerDataType> serializer)
			return serializer;

		var actualDataType = serializerObj.ItemType;

		Guard.Ensure(actualDataType.FastIsSubTypeOf(typeof(TSerializerDataType)), $"Serializer object is not an {typeof(IItemSerializer<>).ToStringCS()}<{typeof(TSerializerDataType).ToStringCS()}>");

		var genericMethod = DecoratorExtensions.SerializerCastMethod.MakeGenericMethod(new [] { actualDataType,  typeof(TSerializerDataType) });
		serializer = genericMethod.FastInvoke(null, new [] { serializerObj }) as IItemSerializer<TSerializerDataType>;;
		return serializer;

	}
}
