// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT NON-AI software license, see the accompanying file
// LICENSE or visit https://sphere10.com/legal/NON-AI-MIT.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System.Collections.ObjectModel;

namespace Hydrogen;

public class CollectionSerializer<T> : CollectionSerializerBase<Collection<T>, T> {

	public CollectionSerializer(IItemSerializer<T> itemSerializer, SizeDescriptorStrategy sizeDescriptorStrategy) : base(itemSerializer, sizeDescriptorStrategy) {
	}

	protected override long GetLength(Collection<T> collection) => collection.Count;

	protected override Collection<T> Activate(long capacity) => new();

	protected override void SetItem(Collection<T> collection, long index, T item) {
		Guard.Ensure(collection.Count == index, "Unexpected index");
		collection.Add(item);
	}
}
