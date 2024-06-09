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

namespace Hydrogen;

public class UniqueKeyStorageAttachment<TKey> : PagedListStorageAttachmentBase<TKey>, IReadOnlyDictionary<TKey, long> {
	
	private readonly Dictionary<TKey, long> _dictionary;

	public UniqueKeyStorageAttachment(ClusteredStreams streams, string attachmentID, IItemSerializer<TKey> keySerializer, IEqualityComparer<TKey> keyComparer)
		: base(streams, attachmentID, keySerializer) {
		Guard.ArgumentNotNull(keyComparer, nameof(keyComparer));
		_dictionary = new Dictionary<TKey, long>(keyComparer);
	}
	
	public int Count => _dictionary.Count;
	
	public IEnumerable<TKey> Keys => _dictionary.Keys;

	public IEnumerable<long> Values => _dictionary.Values;

	public bool ContainsKey(TKey key) => _dictionary.ContainsKey(key);

	public bool TryGetValue(TKey key, out long value) => _dictionary.TryGetValue(key, out value);

	public TKey Read(long index) {
		CheckAttached();
		return PagedList.Read(index);
	}

	public byte[] ReadBytes(long index) {
		CheckAttached();
		PagedList.ReadItemBytes(index, 0, null, out var bytes);
		return bytes;
	}

	public void Add(long index, TKey key) {
		CheckAttached();
		Guard.Argument(index == PagedList.Count, nameof(index), "Index mismatches with expected index in store");
		_dictionary.Add(key, index);
		PagedList.Add(key);
	}

	public void Update(long index, TKey key) {
		CheckAttached();
		var oldKey = PagedList.Read(index);
		_dictionary.Remove(oldKey);
		_dictionary.Add(key, index);
		PagedList.Update(index, key);
	}

	public void Insert(long index, TKey key) {
		CheckAttached();
		Guard.Ensure(!_dictionary.ContainsKey(key));
		PagedList.Insert(index, key);
		HydrateStore(); // rebuild entire memory index since indices have shifted
	}

	public void Remove(long index) {
		CheckAttached();
		PagedList.RemoveAt(index);
		HydrateStore(); // rebuild entire memory index since indices have shifted
	}

	public void Reap(long index) {
		CheckAttached();
		var oldKey = PagedList.Read(index);
		_dictionary.Remove(oldKey);
		// Reap doesn't delete item, only tombstones it so indices are preserved
	}

	public void Clear() {
		CheckAttached();
		_dictionary.Clear();
		PagedList.Clear();
	}
	
	public IEnumerator<KeyValuePair<TKey, long>> GetEnumerator() {
		CheckAttached();
		return _dictionary.GetEnumerator();
	}

	IEnumerator IEnumerable.GetEnumerator() {
		return GetEnumerator();
	}
	

	public long this[TKey key] => _dictionary[key];

	protected override void AttachInternal() {
		base.AttachInternal();
		HydrateStore();
	}

	private void HydrateStore() {
		// Loads the storage and fill out the lookup with the stored projections
		_dictionary.Clear();
		using var _ = Streams.EnterAccessScope();
		var reserved = Streams.Header.ReservedStreams;
		for (var i = 0L; i < PagedList.Count; i++) {
			// reaped objects are ignored
			if (Streams.FastReadStreamDescriptorTraits(i + reserved).HasFlag(ClusteredStreamTraits.Reaped))
				continue;
			var key = PagedList.Read(i);
			_dictionary.Add(key, i);
		}
	}

}
