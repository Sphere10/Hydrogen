// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Collections.Generic;

namespace Hydrogen;

internal class UniqueKeyIndex<TItem, TKey> : IndexBase<TItem, TKey, UniqueKeyStore<TKey>>, IUniqueKeyIndex<TKey> {

	public UniqueKeyIndex(ObjectStream<TItem> objectStream, long reservedStreamIndex, Func<TItem, TKey> projection, IEqualityComparer<TKey> keyComparer, IItemSerializer<TKey> keySerializer)
		: base( objectStream, projection, new UniqueKeyStore<TKey>(objectStream.Streams, reservedStreamIndex, keyComparer, keySerializer)) {
	}

	public IReadOnlyDictionary<TKey, long> Dictionary => KeyStore.Dictionary;

	protected override void OnAdding(TItem item, long index, TKey key) {
		base.OnAdding(item, index, key);
		if (!IsUnique(key, null, out var clashIndex)) 
			throw new InvalidOperationException($"Unable to add {typeof(TItem).ToStringCS()} as a unique key violation occurs with item at {clashIndex}");
	}

	protected override void OnUpdating(TItem item, long index, TKey key) {
		base.OnUpdating(item, index, key);
		if (!IsUnique(key, index, out var clashIndex)) 
			throw new InvalidOperationException($"Unable to update {typeof(TItem).ToStringCS()} at {index} as a unique key violation occurs with item at {clashIndex}");
	}

	protected override void OnInserting(TItem item, long index, TKey key) {
		base.OnInserting(item, index, key);
		if (!IsUnique(key, index, out var clashIndex)) 
			throw new InvalidOperationException($"Unable to insert {typeof(TItem).ToStringCS()} at {index} as a unique key violation occurs with item at {clashIndex}");
	}

	private bool IsUnique(TKey key, long? exemptIndex, out long clashIndex) {
		if (Dictionary.TryGetValue(key, out var foundIndex)) {
			if (foundIndex != exemptIndex) {
				clashIndex = foundIndex;
				return false;
			}
		}
		clashIndex = default;
		return true;
	}

}

