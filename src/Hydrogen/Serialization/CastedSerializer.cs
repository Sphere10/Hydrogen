// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System.Collections.Generic;
using System.Linq;

namespace Hydrogen;

/// <summary>
/// Converts a serializer for <see cref="TConcrete"/> to a serializer of it's base-type <see cref="TBase"/>
/// </summary>
/// <typeparam name="TBase">The type that this serializer will serialize</typeparam>
/// <typeparam name="TConcrete">The concrete type of the supplied serializer which is converted</typeparam>
public class CastedSerializer<TBase, TConcrete> : IItemSerializer<TBase> where TConcrete : TBase {
	private readonly IItemSerializer<TConcrete> _concreteSerializer;

	public CastedSerializer(IItemSerializer<TConcrete> concreteSerializer) {
		_concreteSerializer = concreteSerializer;
	}

	public bool IsStaticSize => _concreteSerializer.IsStaticSize;

	public long StaticSize => _concreteSerializer.StaticSize;

	public long CalculateTotalSize(IEnumerable<TBase> items, bool calculateIndividualItems, out long[] itemSizes)
		=> _concreteSerializer.CalculateTotalSize(items.Cast<TConcrete>(), calculateIndividualItems, out itemSizes);

	public long CalculateSize(TBase item) => _concreteSerializer.CalculateSize((TConcrete)item);

	public void SerializeInternal(TBase item, EndianBinaryWriter writer)
		=> _concreteSerializer.Serialize((TConcrete)item, writer);

	public TBase DeserializeInternal(long byteSize, EndianBinaryReader reader) 
		=> _concreteSerializer.Deserialize(byteSize, reader);

}
