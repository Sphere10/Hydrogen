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

/// <summary>
/// An index for an <see cref="ObjectContainer{TItem}"/> that tracks a key of type <see cref="TKey"/> from items of type <see cref="{TItem}"/>
/// being tracked in the container.
/// </summary>
/// <typeparam name="TItem">The type of the item stored in the container</typeparam>
/// <typeparam name="TKey">The type of the key from the item being indexed</typeparam>
internal class KeyIndex<TItem, TKey> : IndexBase<TItem, TKey> {
	private MemoryCachedMetaDataLookup<TKey> _lookupStore;
	private readonly IEqualityComparer<TKey> _keyComparer;
	private readonly IItemSerializer<TKey> _keySerializer;

	public KeyIndex(ObjectContainer<TItem> container, long reservedStreamIndex, Func<TItem, TKey> projection, IEqualityComparer<TKey> keyComparer, IItemSerializer<TKey> keySerializer)
		: base(container, reservedStreamIndex, projection) {
		_keyComparer = keyComparer;
		_keySerializer = keySerializer;
		_lookupStore = new MemoryCachedMetaDataLookup<TKey>(
			new ListBasedMetaDataStore<TKey>(
				Container, 
				ReservedStreamIndex, 
				_keySerializer
			),
			_keyComparer
		);
	}

	public ILookup<TKey, long> Lookup {
		get {
			CheckAttached();
			return _lookupStore.Lookup;
		}
	}

	protected override void AttachInternal() {
		// BUG: the base class attaches to the stream, but so does the component lookup
		_lookupStore.Attach();
	}

	protected override void DetachInternal() {
		_lookupStore.Detach();
		_lookupStore = null;
	}


	protected override void OnAdded(TItem item, long index, TKey key) {
		_lookupStore.Add(index, key);
	}

	protected override void OnInserted(TItem item, long index, TKey key) {
		_lookupStore.Insert(index, key);
	}

	protected override void OnUpdated(TItem item, long index, TKey key) {
		_lookupStore.Update(index, key);
	}

	protected override void OnRemoved(long index) {
		_lookupStore.Remove(index);
	}

	protected override void OnReaped(long index) {
		_lookupStore.Reap(index);
	}


}
