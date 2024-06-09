// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT NON-AI software license, see the accompanying file
// LICENSE or visit https://sphere10.com/legal/NON-AI-MIT.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System.Collections.Generic;

namespace Hydrogen;

public class ListSerializer<T> : CollectionSerializerBase<List<T>, T> {

	public ListSerializer(IItemSerializer<T> itemSerializer, SizeDescriptorStrategy sizeDescriptorStrategy = SizeDescriptorStrategy.UseCVarInt) : base(itemSerializer, sizeDescriptorStrategy) {
	}

	protected override long GetLength(List<T> collection) => collection.Count;

	protected override List<T> Activate(long capacity) => new((int)capacity);

	protected override void SetItem(List<T> collection, long index, T item) {
		Guard.Ensure(collection.Count == index, "Unexpected index");
		collection.Add(item);
	}
}
