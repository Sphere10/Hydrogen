// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT NON-AI software license, see the accompanying file
// LICENSE or visit https://sphere10.com/legal/NON-AI-MIT.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Collections.Generic;
using System.Linq;

namespace Hydrogen;

public class PolymorphicSerializer<TItem> : IItemSerializer<TItem> {
	private readonly SerializerFactory _factory;
	private readonly SerializerSerializer _serializerSerializer;
	private readonly IItemSerializer<TItem> _pureSerializer;

	public PolymorphicSerializer(SerializerFactory factory)
		: this(factory, !typeof(TItem).IsAbstract ? factory.GetPureSerializer<TItem>() : null) {
	}

	public PolymorphicSerializer(SerializerFactory factory, IItemSerializer<TItem> pureSerializer) {
		Guard.ArgumentNotNull(factory, nameof(factory));
		if (typeof(TItem).IsAbstract)
			Guard.Argument(pureSerializer is null, nameof(pureSerializer), "Must be null for abstract types");
		else
			Guard.Argument(pureSerializer is not null, nameof(pureSerializer), "Must not be null for non-abstract types");

		_factory = factory;
		_serializerSerializer = new SerializerSerializer(_factory);
		_pureSerializer = pureSerializer;
	}

	public IItemSerializer<TItem> PureSerializer => _pureSerializer ?? throw new InvalidOperationException("Abstract types have no pure serializer");

	public SerializerFactory Factory => _factory;

	public bool SupportsNull => false;

	public bool IsConstantSize => false;

	public long ConstantSize => -1;

	public long CalculateTotalSize(SerializationContext context, IEnumerable<TItem> items, bool calculateIndividualItems, out long[] itemSizes) {
		var itemSizesL = new List<long>();
		var totalSize = items.Aggregate(
			0L,
			(i, o) => {
				var itemSize = GetPackedItemSerializer(context, DetermineExplicitSerializationType(o)).PackedCalculateSize(o);
				if (calculateIndividualItems)
					itemSizesL.Add(itemSize);
				return itemSize;
			});
		itemSizes = itemSizesL.ToArray();
		return totalSize;
	}

	public long CalculateSize(SerializationContext context, TItem item)  {
		var serializer = GetPackedItemSerializer(context, DetermineExplicitSerializationType(item));
		return _serializerSerializer.CalculateSize(context, serializer) + serializer.PackedCalculateSize(context, item);
	}

	public void Serialize(TItem item, EndianBinaryWriter writer, SerializationContext context) {
		var serializer = GetPackedItemSerializer(context, DetermineExplicitSerializationType(item));
		_serializerSerializer.Serialize(serializer, writer, context);
		serializer.PackedSerialize(item, writer, context);
	}

	public TItem Deserialize(EndianBinaryReader reader, SerializationContext context) {
		var serializerObj = _serializerSerializer.Deserialize(reader, context);
		var serializer = ToUsableSerializer(serializerObj);
		return serializer.Deserialize(reader, context);
	}

	private Type DetermineExplicitSerializationType(TItem item) 
		=> item is null || typeof(TItem).IsConstructedGenericTypeOf(typeof(Nullable<>)) ? typeof(TItem) : item.GetType();

	private IItemSerializer GetPackedItemSerializer(SerializationContext context, Type dataType) {
		if (dataType == typeof(TItem))
			return PureSerializer;

		var factory = context.HasEphemeralFactory ? context.EphemeralFactory : _factory;

		if (!factory.TryGetPureSerializer(dataType, out var serializer, out var missingSerializers)) {
			if (!context.HasEphemeralFactory)
				throw new InvalidOperationException($"No pure serializer for {dataType.ToStringCS()} was found");
			foreach(var missing in missingSerializers) 
				SerializerBuilder.FactoryAssemble(context.EphemeralFactory, missing, true);
			serializer = context.EphemeralFactory.GetPureSerializer(dataType);
		}
		return serializer;
	}

	private IItemSerializer<TItem> ToUsableSerializer(IItemSerializer serializer) {
		if (serializer.ItemType != typeof(TItem))
			return (IItemSerializer<TItem>)serializer.AsCastedSerializer(typeof(TItem));
		return (IItemSerializer<TItem>)serializer;
	}

}

public static class PolymorphicSerializer {
	public static IItemSerializer Create(SerializerFactory factory, Type itemType) 
		=> (IItemSerializer)typeof(PolymorphicSerializer<>)
			.MakeGenericType(itemType)
			.ActivateWithCompatibleArgs(factory);
}