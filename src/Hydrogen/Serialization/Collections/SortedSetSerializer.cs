// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT NON-AI software license, see the accompanying file
// LICENSE or visit https://sphere10.com/legal/NON-AI-MIT.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Hydrogen;

public class SortedSetSerializer<TItem> : CollectionSerializerBase<SortedSet<TItem>, TItem> {

	public SortedSetSerializer(IItemSerializer<TItem> itemSerializer, SizeDescriptorStrategy sizeDescriptorStrategy = SizeDescriptorStrategy.UseCVarInt) : base(itemSerializer, sizeDescriptorStrategy) {
	}

	protected override long GetLength(SortedSet<TItem> collection) => collection.Count;

	protected override SortedSet<TItem> Activate(long capacity) => new();

	protected override void SetItem(SortedSet<TItem> collection, long index, TItem item) {
		Guard.Ensure(collection.Count == index, "Unexpected index");
		if (!collection.Add(item))
			throw new SerializationException("Duplicate item in sorted set");
	}
}
