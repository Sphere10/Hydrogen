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
/// A serializer that re-interprets the arguments of another serializer arguments (useful for casting)
/// </summary>
public class CastedSerializer<TItem> : IItemSerializer<TItem> {
	private readonly IItemSerializer _serializer;

	public CastedSerializer(IItemSerializer innerSerializer) {
		_serializer = innerSerializer;
	}

	Type IItemSizer.ItemType => _serializer.ItemType;

	public bool SupportsNull => _serializer.SupportsNull;

	public bool IsConstantSize => _serializer.IsConstantSize;

	public long ConstantSize => _serializer.ConstantSize;

	public long CalculateTotalSize(IEnumerable<TItem> items, bool calculateIndividualItems, out long[] itemSizes) 
		=> _serializer.CalculateTotalSize(items.Cast<object>(), calculateIndividualItems, out itemSizes);

	public long CalculateSize(SerializationContext context, TItem item) 
		=> _serializer.CalculateSize(context, item);

	public void Serialize(TItem item, EndianBinaryWriter writer, SerializationContext context) 
		=> _serializer.Serialize(item, writer, context);

	public TItem Deserialize(EndianBinaryReader reader, SerializationContext context) 
		=> (TItem)_serializer.Deserialize(reader, context);
}
