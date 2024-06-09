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

public class IndexStorageAttachment<TData> : PagedListStorageAttachmentBase<TData>, ILookup<TData, long> {

	private readonly ExtendedLookup<TData, long> _lookup;

	public IndexStorageAttachment(ClusteredStreams streams, string attachmentID, IItemSerializer<TData> datumSerializer, IEqualityComparer<TData> datumComparer)
		: base(streams, attachmentID, datumSerializer) {
		Guard.ArgumentNotNull(datumComparer, nameof(datumComparer));
		_lookup = new ExtendedLookup<TData, long>(datumComparer);
	}

	public int Count => _lookup.Count;

	protected override void AttachInternal() {
		base.AttachInternal();
		HydrateStore();
	}

	public TData Read(long index) {
		CheckAttached();
		return PagedList.Read(index);
	}

	public byte[] ReadBytes(long index) {
		CheckAttached();
		PagedList.ReadItemBytes(index, 0, null, out var bytes);
		return bytes;
	}

	public void Add(long index, TData data) {
		CheckAttached();
		Guard.Argument(index == PagedList.Count, nameof(index), "Index mismatches with expected index in store");
		PagedList.Add(data);
		_lookup.Add(data, index);
	}

	public void Update(long index, TData data) {
		CheckAttached();
		var oldProjection = PagedList.Read(index);
		_lookup.Remove(oldProjection, index);
		PagedList.Update(index, data);
		_lookup.Add(data, index);
	}
	public void Insert(long index, TData data) {
		CheckAttached();
		PagedList.Insert(index, data);
		HydrateStore(); // rebuild entire memory index since indices have shifted
	}

	public void Remove(long index) {
		CheckAttached();
		PagedList.RemoveAt(index);
		HydrateStore(); // rebuild entire memory index since indices have shifted
	}

	public void Reap(long index) {
		CheckAttached();
		var oldProjection = PagedList.Read(index);
		_lookup.Remove(oldProjection, index);
		// Reap doesn't delete item, only tombstones it
	}


	public void Clear() {
		CheckAttached();
		_lookup.Clear();
		PagedList.Clear();
	}

	public bool Contains(TData key) => _lookup.Contains(key);

	public IEnumerator<IGrouping<TData, long>> GetEnumerator() => _lookup.GetEnumerator();

	IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

	public IEnumerable<long> this[TData key] => _lookup[key];

	private void HydrateStore() {
		// Loads the storage and fill out the lookup with the stored projections
		_lookup.Clear();
		using var _ = Streams.EnterAccessScope();
		var reserved = Streams.Header.ReservedStreams;
		for (var i = 0L; i < PagedList.Count; i++) {
			// reaped objects are ignored
			if (Streams.FastReadStreamDescriptorTraits(i + reserved).HasFlag(ClusteredStreamTraits.Reaped))
				continue;
			var value = PagedList.Read(i);
			_lookup.Add(value, i);
		}
	}
}
