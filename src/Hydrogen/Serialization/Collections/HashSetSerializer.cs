// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Hydrogen;

public class HashSetSerializer<TItem> : CollectionSerializerBase<HashSet<TItem>, TItem> {

	public HashSetSerializer(IItemSerializer<TItem> itemSerializer, SizeDescriptorStrategy sizeDescriptorStrategy) 
		: base(itemSerializer, sizeDescriptorStrategy) {
	}

	protected override long GetLength(HashSet<TItem> collection) => collection.Count;

	protected override HashSet<TItem> Activate(long capacity) => new (checked((int)capacity));

	protected override void SetItem(HashSet<TItem> collection, long index, TItem item) {
		Guard.Ensure(collection.Count == index, "Unexpected index");
		if (!collection.Add(item))
			throw new SerializationException("Duplicate item in sorted set");
	}
}
