// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT NON-AI software license, see the accompanying file
// LICENSE or visit https://sphere10.com/legal/NON-AI-MIT.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Collections.Generic;
using System.Linq;


namespace Hydrogen;


/// <summary>
/// Projection index that groups item positions by a projected key using an <see cref="IndexStorageAttachment{TKey}"/>.
/// </summary>
internal sealed class ProjectionIndex<TItem, TProjection> : ProjectionIndexBase<TItem, TProjection, IndexStorageAttachment<TProjection>>, IProjectionIndex<TProjection> {

	private readonly Func<TItem, TProjection> _projection;

	public ProjectionIndex(ObjectStream<TItem> objectStream, string indexName, Func<TItem, TProjection> projection, IItemSerializer<TProjection> projectionSerializer, IEqualityComparer<TProjection> projectionComparer)
		: base(
			objectStream,
			new IndexStorageAttachment<TProjection>(objectStream.Streams, indexName, projectionSerializer, projectionComparer),
			IndexNullPolicy.ThrowOnNull
		) {
		Guard.ArgumentNotNull(projection, nameof(projection));
		Guard.Argument(projectionSerializer.IsConstantSize, nameof(projectionSerializer), "Must be a constant size serializer");
		_projection = projection;
	}

	public ILookup<TProjection, long> Values {
		get {
			CheckAttached();
			return Store;
		}
	}

	public override TProjection ApplyProjection(TItem item) => _projection.Invoke(item);

	protected override void OnAdded(TItem item, long index, TProjection keyChecksum) {
		Store.Add(index, keyChecksum);
	}

	protected override void OnUpdated(TItem item, long index, TProjection keyChecksum) {
		Store.Add(index, keyChecksum);
	}
	protected override void OnInserted(TItem item, long index, TProjection keyChecksum) {
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
		// After objectStream was cleared, we reboot the index
		Store.Attach();
	}

}
