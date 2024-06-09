// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT NON-AI software license, see the accompanying file
// LICENSE or visit https://sphere10.com/legal/NON-AI-MIT.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Hydrogen;

/// <summary>
/// An index that only stores the checksum of the (projected) key rather than the key itself and relies on fetching the key from storage
/// when needed to resolve checksum collisions. Works well for dynamically-sized keys.
/// </summary>
/// <typeparam name="TItem"></typeparam>
/// <typeparam name="TProjection"></typeparam>
internal sealed class ProjectionChecksumIndex<TItem, TProjection> : ProjectionIndexBase<TItem, (TProjection, int), IndexStorageAttachment<int>>, IProjectionIndex<TProjection> {
	private readonly Func<TItem, TProjection> _projection;
	private readonly IItemChecksummer<TProjection> _projectionChecksummer;
	private readonly Func<long, TProjection> _projectionHydrator;
	private readonly IEqualityComparer<TProjection> _keyComparer;
	private readonly ChecksumBasedLookup _checksumLookup;

	public ProjectionChecksumIndex(
		ObjectStream<TItem> objectStream,
		string indexName,
		Func<TItem, TProjection> projection,
		IItemChecksummer<TProjection> projectionChecksummer,
		Func<long, TProjection> projectionHydrator,
		IEqualityComparer<TProjection> keyComparer,
		IndexNullPolicy nullPolicy
	) : base(
			objectStream,
			new IndexStorageAttachment<int>(objectStream.Streams, indexName, PrimitiveSerializer<int>.Instance, EqualityComparer<int>.Default),
			nullPolicy
		) {
		Guard.ArgumentNotNull(projection, nameof(projection));
		Guard.ArgumentNotNull(projectionChecksummer, nameof(projectionChecksummer));
		Guard.ArgumentNotNull(projectionHydrator, nameof(projectionHydrator));
		Guard.ArgumentNotNull(keyComparer, nameof(keyComparer));

		_projection = projection;
		_projectionChecksummer = projectionChecksummer;
		_projectionHydrator = projectionHydrator;
		_keyComparer = keyComparer;
		_checksumLookup = new ChecksumBasedLookup(this);
	}

	public ILookup<TProjection, long> Values {
		get {
			CheckAttached();
			return _checksumLookup;
		}
	}

	public override (TProjection, int) ApplyProjection(TItem item) {
		var projection = _projection(item);
		var checksum = _projectionChecksummer.CalculateChecksum(projection);
		return (projection, checksum);
	}

	protected TProjection HydrateProjection(long index) => _projectionHydrator(index);

	protected override void OnAdded(TItem item, long index, (TProjection, int) keyChecksum) {
		var checksum = keyChecksum.Item2;
		Store.Add(index, checksum);
	}

	protected override void OnUpdated(TItem item, long index, (TProjection, int) keyChecksum) {
		var checksum = keyChecksum.Item2;
		Store.Update(index, checksum);
	}

	protected override void OnInserted(TItem item, long index, (TProjection, int) keyChecksum) {
		var checksum = keyChecksum.Item2;
		Store.Insert(index, checksum);
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

	protected override bool IsNullValue((TProjection, int) projection) => projection.Item1 is null;

	private class ChecksumBasedLookup : ILookup<TProjection, long> {
		private readonly ProjectionChecksumIndex<TItem, TProjection> _parent;

		public ChecksumBasedLookup(ProjectionChecksumIndex<TItem, TProjection> parent) {
			_parent = parent;
		}

		public IEnumerator<IGrouping<TProjection, long>> GetEnumerator()
			=> throw new NotSupportedException("Enumerating checksum stored indices is not supported");

		IEnumerator IEnumerable.GetEnumerator() {
			return GetEnumerator();
		}

		public bool Contains(TProjection key) {
			using var _ = _parent.Streams.EnterAccessScope();
			return
				 _parent
				.Store[_parent._projectionChecksummer.CalculateChecksum(key)]
				.Any(x => _parent._keyComparer.Equals(key, _parent.HydrateProjection(x)));
		}

		public int Count => _parent._checksumLookup.Count;

		public IEnumerable<long> this[TProjection key] {
			get {
				using var _ = _parent.Streams.EnterAccessScope();
				return
					_parent
					.Store[_parent._projectionChecksummer.CalculateChecksum(key)]
					.Where(x => _parent._keyComparer.Equals(key, _parent.HydrateProjection(x)));
			}
		}
	}

}
