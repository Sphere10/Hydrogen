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
/// <remarks>Ensure that the identical factory is used for both Serialization and Deserialization for consistent results.</remarks>
/// <typeparam name="TBase">The base-types of objects being serialized/deserialized</typeparam>
public class PolymorphicSerializer<TBase> : IItemSerializer<TBase> {
	private readonly SerializerFactory _factory;
	private readonly SerializerSerializer _serializerSerializer;

	public PolymorphicSerializer() 
		: this(new SerializerFactory()) {
	}

	public PolymorphicSerializer(SerializerFactory factory) {
		_factory = factory;
		_serializerSerializer = new SerializerSerializer(_factory);
	}

	public SerializerFactory Factory => _factory;

	public bool SupportsNull => false;

	public bool IsConstantSize => false;

	public long ConstantSize => -1;

	public long CalculateTotalSize(SerializationContext context, IEnumerable<TBase> items, bool calculateIndividualItems, out long[] itemSizes) {
		var itemSizesL = new List<long>();
		var totalSize = items.Aggregate(
			0L,
			(i, o) => {
				var itemSize = GetItemSerializer(o).CalculateSize(o);
				if (calculateIndividualItems)
					itemSizesL.Add(itemSize);
				return itemSize;
			});
		itemSizes = itemSizesL.ToArray();
		return totalSize;
	}

	public long CalculateSize(SerializationContext context, TBase item)  {
		var serializer = GetItemSerializer(item);
		return _serializerSerializer.CalculateSize(context, serializer) + serializer.CalculateSize(context, item);
	}

	public void Serialize(TBase item, EndianBinaryWriter writer, SerializationContext context) {
		var serializer = GetItemSerializer(item);
		_serializerSerializer.Serialize(serializer, writer, context);
		serializer.Serialize(item, writer, context);
	}

	public TBase Deserialize(EndianBinaryReader reader, SerializationContext context) {
		var serializerObj = _serializerSerializer.Deserialize(reader, context);
		var serializer = ToUsableSerializer<TBase>(serializerObj);
		return serializer.Deserialize(reader, context);
	}

	private IItemSerializer<TSerializerDataType> ToUsableSerializer<TSerializerDataType>(IItemSerializer serializerObj) {
		// ensure the underlying serializer is not a Reference Serializer
		serializerObj = serializerObj.AsDereferencedSerializer();  

		// If serializer gives the type we want, use it
		if (serializerObj is IItemSerializer<TSerializerDataType> serializer)
			return serializer;

		// Cast the serializer to the type we want
		var actualDataType = serializerObj.ItemType;
		Guard.Ensure(actualDataType.FastIsSubTypeOf(typeof(TSerializerDataType)), $"Serializer object is not an {typeof(IItemSerializer<>).ToStringCS()}");
		return new CastedSerializer<TSerializerDataType>(serializerObj);
	}

	private IItemSerializer<TBase> GetItemSerializer(TBase item) {
		Type superType;
		if (item is null || typeof(TBase).IsConstructedGenericTypeOf(typeof(Nullable<>))) {
			superType = typeof(TBase);
		} else {
			superType = item.GetType();
		} 

		var serializer = _factory.GetRegisteredSerializer<TBase>(superType, false); 

		// Ensure item serializer is not a reference serializer. Handling null, cyclic and other references is the
		// responsibility of ReferenceSerializer.
		Guard.Ensure(serializer is not ReferenceSerializer<TBase>, "A PolymorphicSerializer cannot wrap a ReferenceSerializer");
		return serializer;
	}
}
