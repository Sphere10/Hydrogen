// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Collections.Generic;

namespace Hydrogen.ObjectSpaces;

internal class UniqueKeyIndex<TItem, TKey> : IndexBase<TItem, TKey> {
	private readonly UniqueKeyStore<TKey> _keyStore;

	public UniqueKeyIndex(ObjectContainer<TItem> container, long reservedStreamIndex, Func<TItem, TKey> projection, IEqualityComparer<TKey> keyComparer, IItemSerializer<TKey> keySerializer)
		: base(container, reservedStreamIndex, projection) {
		_keyStore = new UniqueKeyStore<TKey>(container, reservedStreamIndex, keyComparer, keySerializer);
	}

	public IReadOnlyDictionary<TKey, long> Dictionary {
		get {
			CheckAttached();
			return _keyStore.Dictionary;
		}
	}

	protected override void AttachInternal() {
		_keyStore.Attach();
	}

	protected override void DetachInternal() {
		_keyStore.Detach();
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

