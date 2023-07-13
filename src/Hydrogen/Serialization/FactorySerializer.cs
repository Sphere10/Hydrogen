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
public class FactorySerializer<TBase> : IItemSerializer<TBase>, IFactorySerializer<TBase> {
	private readonly IDictionary<ushort, IItemSerializer<TBase>> _concreteLookup;
	private readonly IBijectiveDictionary<Type, ushort> _typeCodeMap;


	public FactorySerializer() {
		_concreteLookup = new Dictionary<ushort, IItemSerializer<TBase>>();
		_typeCodeMap = new BijectiveDictionary<Type, ushort>();
	}

	public IEnumerable<Type> RegisteredTypes => _typeCodeMap.Keys;

	public void RegisterSerializer<TConcrete>(IItemSerializer<TConcrete> concreteSerializer) where TConcrete : TBase
		=> RegisterSerializer(GenerateTypeCode(), concreteSerializer);

	public void RegisterSerializer<TConcrete>(ushort typeCode, IItemSerializer<TConcrete> concreteSerializer) where TConcrete : TBase {
		Guard.Argument(!_typeCodeMap.ContainsValue(typeCode), nameof(typeCode), $"Type code {typeCode} for type '{typeof(TConcrete).Name}' is already used for another serializer");
		var concreteType = typeof(TConcrete);
		Guard.Argument(!_typeCodeMap.ContainsKey(concreteType), nameof(TConcrete), "Type already registered");
		_concreteLookup.Add(typeCode, new CastedSerializer<TBase, TConcrete>(concreteSerializer));
		_typeCodeMap.Add(typeof(TConcrete), typeCode);
	}

	public bool IsStaticSize => false;

	public long StaticSize => -1;

	public long CalculateTotalSize(IEnumerable<TBase> items, bool calculateIndividualItems, out long[] itemSizes) {
		var itemSizesL = new List<long>();
		var totalSize = items.Aggregate(
			0L,
			(i, o) => {
				var itemSize = GetConcreteSerializer(GetTypeCode(o.GetType())).CalculateSize(o);
				if (calculateIndividualItems)
					itemSizesL.Add(itemSize);
				return itemSize;
			});
		itemSizes = itemSizesL.ToArray();
		return totalSize;
	}

	public long CalculateSize(TBase item) => GetConcreteSerializer(item).CalculateSize(item);

	public bool TrySerialize(TBase item, EndianBinaryWriter writer, out long bytesWritten) {
		var typeCode = GetTypeCode(item);
		writer.Write(typeCode);
		return GetConcreteSerializer(typeCode).TrySerialize(item, writer, out bytesWritten);
	}

	public bool TryDeserialize(long byteSize, EndianBinaryReader reader, out TBase item) {
		var typeCode = reader.ReadUInt16();
		return GetConcreteSerializer(typeCode).TryDeserialize(byteSize, reader, out item);
	}

	public ushort GetTypeCode<TConcrete>(TConcrete item) where TConcrete : TBase => GetTypeCode(item.GetType());

	public ushort GetTypeCode(Type type) => _typeCodeMap[type];

	public ushort GenerateTypeCode() => _typeCodeMap.Count > 0 ? (ushort)(_typeCodeMap.Values.MaxByEx(x => x) + 1) : (ushort)0;

	private IItemSerializer<TBase> GetConcreteSerializer(TBase item) => GetConcreteSerializer(GetTypeCode(item));

	private IItemSerializer<TBase> GetConcreteSerializer(ushort typeCode) => _concreteLookup[typeCode];
}
