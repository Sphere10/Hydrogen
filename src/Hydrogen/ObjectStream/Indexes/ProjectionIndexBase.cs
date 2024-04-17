// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

namespace Hydrogen;

/// <summary>
/// Base implementation for an index on an <see cref="ObjectStream{T}"/>.
/// </summary>
/// <typeparam name="TItem">Type of item being stored in <see cref="ObjectStream{T}"/></typeparam>
/// <typeparam name="TProjection">Type of property in <see cref="TItem"/> that is the keyChecksum</typeparam>
/// <typeparam name="TStore">Type of store used to store item member values</typeparam>
public abstract class ProjectionIndexBase<TItem, TProjection, TStore> : IndexBase<TStore> where TStore : IClusteredStreamsAttachment {
	protected new ObjectStream<TItem> Objects;
	protected ProjectionIndexBase(ObjectStream<TItem> objectStream, TStore store)
		: base(objectStream, store) {
		
		Guard.Ensure(!store.IsAttached, "Store must not be attached already");
		Objects = (ObjectStream<TItem>)base.Objects;
	}

	public abstract TProjection ApplyProjection(TItem item);

	protected sealed override void OnAdding(object item, long index) {
		base.OnAdding(item, index);
		var itemT = (TItem)item;
		OnAdding(itemT, index, ApplyProjection(itemT));
	}

	protected sealed override void OnAdded(object item, long index) {
		base.OnAdded(item, index);
		var itemT = (TItem)item;
		OnAdded(itemT, index, ApplyProjection(itemT));
	}

	protected sealed override void OnInserting(object item, long index) {
		base.OnInserting(item, index);
		var itemT = (TItem)item;
		OnInserting(itemT, index, ApplyProjection(itemT));
	}

	protected sealed override void OnInserted(object item, long index) {
		base.OnInserted(item, index);
		var itemT = (TItem)item;
		OnInserted(itemT, index, ApplyProjection(itemT));
	}

	protected sealed override void OnUpdating(object item, long index) {
		base.OnUpdating(item, index);
		var itemT = (TItem)item;
		OnUpdating(itemT, index, ApplyProjection(itemT));
	}

	protected sealed override void OnUpdated(object item, long index) {
		base.OnUpdated(item, index);
		var itemT = (TItem)item;
		OnUpdated(itemT, index, ApplyProjection(itemT));
	}

	protected virtual void OnAdding(TItem item, long index, TProjection projection) {
	}

	protected virtual void OnAdded(TItem item, long index, TProjection keyChecksum) {
	}

	protected virtual void OnInserting(TItem item, long index, TProjection projection) {
	}

	protected virtual void OnInserted(TItem item, long index, TProjection keyChecksum) {
	}

	protected virtual void OnUpdating(TItem item, long index, TProjection projection) {
	}

	protected virtual void OnUpdated(TItem item, long index, TProjection keyChecksum) {
	}

	protected override void OnRemoved(long index) {
	}

	protected override void OnReaped(long index) {
	}

}
