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

/// <summary>
/// A serializer that re-interprets the arguments of another serializer arguments (useful for casting)
/// </summary>
public class CastedSerializer<TFrom, TTo> : IItemSerializer<TTo>  {
	private readonly IItemSerializer<TFrom> _serializer;

	public CastedSerializer(IItemSerializer<TFrom> innerSerializer) {
		_serializer = innerSerializer;
	}

	Type IItemSizer.ItemType => typeof(TFrom);	// NOTE: Only casts TItem -> TBase, doesn't serialize all instances of TBase

	public bool SupportsNull => _serializer.SupportsNull;

	public bool IsConstantSize => _serializer.IsConstantSize;

	public long ConstantSize => _serializer.ConstantSize;

	public long CalculateTotalSize(SerializationContext context, IEnumerable<TTo> items, bool calculateIndividualItems, out long[] itemSizes) 
		=> _serializer.CalculateTotalSize(context, items.Cast<TFrom>(), calculateIndividualItems, out itemSizes);

	public long CalculateSize(SerializationContext context, TTo item) 
		=> _serializer.CalculateSize(context, (TFrom)(object)item);

	public void Serialize(TTo item, EndianBinaryWriter writer, SerializationContext context) 
		=> _serializer.Serialize((TFrom)(object)item, writer, context);

	public TTo Deserialize(EndianBinaryReader reader, SerializationContext context) 
		=> (TTo)(object)_serializer.Deserialize(reader, context);
}
