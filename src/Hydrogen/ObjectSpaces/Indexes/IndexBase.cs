// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Runtime.CompilerServices;

namespace Hydrogen.ObjectSpaces;

/// <summary>
/// Base implementation for an index on an <see cref="ObjectContainer{TItem}"/>.
/// </summary>
/// <typeparam name="TItem">Type of item being stored in <see cref="ObjectContainer{T}"/></typeparam>
/// <typeparam name="TKey">Type of property in <see cref="TItem"/> that is the key</typeparam>
public abstract class IndexBase<TItem, TKey> : MetaDataObserverBase<TItem> {
	private readonly Func<TItem, TKey> _projection;

	protected IndexBase(ObjectContainer<TItem> container, long reservedStreamIndex, Func<TItem, TKey> projection)
		: base(container, reservedStreamIndex) {
		Guard.ArgumentNotNull(container, nameof(container));
		Guard.ArgumentNotNull(projection, nameof(projection));
		_projection = projection;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public TKey CalculateKey(TItem item) => _projection.Invoke(item);

	protected sealed override void OnAdding(object item, long index) {
		base.OnAdding(item, index);
		var itemT = (TItem)item;
		OnAdding(itemT, index, CalculateKey(itemT));
	}

	protected sealed override void OnAdded(object item, long index) {
		base.OnAdded(item, index);
		var itemT = (TItem)item;
		OnAdded(itemT, index, CalculateKey(itemT));
	}

	protected sealed override void OnInserting(object item, long index) {
		base.OnInserting(item, index);
		var itemT = (TItem)item;
		OnInserting(itemT, index, CalculateKey(itemT));
	}

	protected sealed override void OnInserted(object item, long index) {
		base.OnInserted(item, index);
		var itemT = (TItem)item;
		OnInserted(itemT, index, CalculateKey(itemT));
	}

	protected sealed override void OnUpdating(object item, long index) {
		base.OnUpdating(item, index);
		var itemT = (TItem)item;
		OnUpdating(itemT, index, CalculateKey(itemT));
	}

	protected sealed override void OnUpdated(object item, long index) {
		base.OnUpdated(item, index);
		var itemT = (TItem)item;
		OnUpdated(itemT, index, CalculateKey(itemT));
	}

	protected virtual void OnAdding(TItem item, long index, TKey key) {
	}

	protected virtual void OnAdded(TItem item, long index, TKey key) {
	}

	protected virtual void OnInserting(TItem item, long index, TKey key) {
	}

	protected virtual void OnInserted(TItem item, long index, TKey key) {
	}

	protected virtual void OnUpdating(TItem item, long index, TKey key) {
	}

	protected virtual void OnUpdated(TItem item, long index, TKey key) {
	}

	protected override void OnContainerClearing() {
		// When the container about to be cleared, we detach the observer
		CheckAttached();
		Detach();
	}

	protected override void OnContainerCleared() {
		// After container was cleared, we reboot the index
		CheckNotAttached();
		Attach();
	}

}
