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

namespace Hydrogen.ObjectSpaces;

internal class NonUniqueKeyIndex<TItem, TKey> : IndexBase<TItem, TKey> {
	private readonly NonUniqueKeyStore<TKey> _keyStore;

	public NonUniqueKeyIndex(ObjectContainer<TItem> container, long reservedStreamIndex, Func<TItem, TKey> projection, IEqualityComparer<TKey> keyComparer, IItemSerializer<TKey> keySerializer)
		: base(
			container,
			reservedStreamIndex,
			projection
		) {
		_keyStore = new NonUniqueKeyStore<TKey>(container, reservedStreamIndex, keyComparer, keySerializer);
	}

	public ILookup<TKey, long> Lookup {
		get {
			CheckAttached();
			return _keyStore.Lookup;
		}
	}
	
	protected override void AttachInternal() {
		_keyStore.Attach();
	}

	protected override void DetachInternal() {
		_keyStore.Attach();
	}

	protected override void OnAdded(TItem item, long index, TKey key) {
		_keyStore.Add(index, key);
	}

	protected override void OnInserted(TItem item, long index, TKey key) {
		_keyStore.Insert(index, key);
	}

	protected override void OnUpdated(TItem item, long index, TKey key) {
		_keyStore.Update(index, key);
	}

	protected override void OnRemoved(long index) {
		_keyStore.Remove(index);
	}

	protected override void OnReaped(long index) {
		_keyStore.Reap(index);
	}

}

