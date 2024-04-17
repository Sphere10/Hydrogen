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

internal sealed class UniqueProjectionIndex<TItem, TKey> : ProjectionIndexBase<TItem, TKey, UniqueKeyStorageAttachment<TKey>>, IUniqueProjectionIndex<TKey> {
	private readonly Func<TItem, TKey> _projection;

	public UniqueProjectionIndex(ObjectStream<TItem> objectStream, string indexName, Func<TItem, TKey> projection, IItemSerializer<TKey> keySerializer, IEqualityComparer<TKey> keyComparer)
		: base(
			objectStream, 
			new UniqueKeyStorageAttachment<TKey>(objectStream.Streams, indexName, keySerializer, keyComparer)
		)  {
		_projection = projection;
	}

	public IReadOnlyDictionary<TKey, long> Values {
		get {
			CheckAttached();
			return Store;
		}
	}

	public override TKey ApplyProjection(TItem item) => _projection.Invoke(item);


	protected override void OnAdding(TItem item, long index, TKey key) {
		if (!IsUnique(key, null, out var clashIndex)) 
			throw new InvalidOperationException($"Unable to add {typeof(TItem).ToStringCS()} as a unique keyChecksum violation occurs with item at {clashIndex}");
	}

	protected override void OnAdded(TItem item, long index, TKey keyChecksum) {
		Store.Add(index, keyChecksum);
	}

	protected override void OnUpdating(TItem item, long index, TKey key) {
		if (!IsUnique(key, index, out var clashIndex)) 
			throw new InvalidOperationException($"Unable to update {typeof(TItem).ToStringCS()} at {index} as a unique keyChecksum violation occurs with item at {clashIndex}");
	}

	protected override void OnUpdated(TItem item, long index, TKey keyChecksum) {
		Store.Update(index, keyChecksum);
	}

	protected override void OnInserting(TItem item, long index, TKey key) {
		if (!IsUnique(key, index, out var clashIndex)) 
			throw new InvalidOperationException($"Unable to insert {typeof(TItem).ToStringCS()} at {index} as a unique keyChecksum violation occurs with item at {clashIndex}");
	}

	protected override void OnInserted(TItem item, long index, TKey keyChecksum) {
		Store.Insert(index, keyChecksum);
	}

	protected override void OnRemoved(long index) {
		Store.Remove(index);
	}

	protected override void OnReaped(long index) {
		Store.Reap(index);
	}

	protected override void OnContainerClearing() {
		Store.Clear();
		Store.Detach();
	}

	protected override void OnContainerCleared() {
		Store.Attach();
	}
	
	private bool IsUnique(TKey key, long? exemptIndex, out long clashIndex) {
		if (Store.TryGetValue(key, out var foundIndex)) {
			if (foundIndex != exemptIndex) {
				clashIndex = foundIndex;
				return false;
			}
		}
		clashIndex = default;
		return true;
	}
}
