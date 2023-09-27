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
/// A serializer defined for objects of type <see cref="TBase"/> that uses a <see cref="SerializerFactory"/> to select serializers for concrete types.
/// In addition to serializing the value, this serializer also serializes the serializer used to serialize the value. This permits it to ensure
/// the same serializer is used for deserialization.
/// </summary>
/// <remarks>Ensure that the identical factory is used for both Serialization and Deserializatin for consistent results.</remarks>
/// <typeparam name="TBase">The base-types of objects being serialized/deserialized</typeparam>
public class FactorySerializer<TBase> : IItemSerializer<TBase> {
	private readonly SerializerFactory _factory;
	private readonly SerializerSerializer _serializerSerializer;

	public FactorySerializer() 
		: this(new SerializerFactory()) {
	}

	public FactorySerializer(SerializerFactory factory) {
		_factory = factory;
		_serializerSerializer = new SerializerSerializer(_factory);
	}

	public SerializerFactory Factory => _factory;

	public bool SupportsNull => false;

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

		return new CastedSerializer<TSerializerDataType>(serializerObj);

	}
}
