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

namespace Hydrogen;

/// <summary>
/// A Serializer that works for base-level objects that delegates actual serialization to registered concrete-level serializers. Serialization is wrapped with the
/// type-code which permits selection of correct concrete type.
/// </summary>
/// <typeparam name="TBase">The type of object which is serialized/deserialized</typeparam>
public class FactorySerializer<TBase> : IItemSerializer<TBase> {
	private readonly SerializerFactory _factory;
	private readonly SerializerSerializer _serializerSerializer;


	public FactorySerializer(bool supportsNull = false) {
		_factory = new SerializerFactory();
		_serializerSerializer = new SerializerSerializer(_factory);
		SupportsNull = supportsNull;
	}

	public IEnumerable<Type> RegisteredTypes => _factory.RegisteredTypes;

	public bool SupportsNull { get; }

	public bool IsConstantLength => false;

	public long ConstantLength => -1;

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

	public void SerializeInternal(TBase item, EndianBinaryWriter writer) {
		var serializer = _factory.GetSerializer<TBase>(item.GetType());
		_serializerSerializer.SerializeInternal(serializer, writer);
		serializer.SerializeInternal(item, writer);
	}

	public TBase DeserializeInternal(long byteSize, EndianBinaryReader reader) {
		var from = reader.BaseStream.Position;
		var serializerObj = _serializerSerializer.DeserializeInternal(byteSize, reader);
		var serializerSize = reader.BaseStream.Position - from;
		var serializer = _serializerSerializer.GetTypedSerializer<TBase>(serializerObj);
		return serializer.DeserializeInternal(byteSize - serializerSize, reader);
	}
	
}
