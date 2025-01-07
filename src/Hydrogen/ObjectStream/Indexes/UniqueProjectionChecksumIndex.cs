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
/// An index that only stores the projection of the projection rather than the projection itself and relies on fetching the projection from the objectStream when needed for comparisons.
/// </summary>
/// <typeparam name="TItem"></typeparam>
/// <typeparam name="TKey"></typeparam>
internal class UniqueProjectionChecksumIndex<TItem, TKey> : ProjectionIndexBase<TItem, (TKey, int), IndexStorageAttachment<int>>, IUniqueProjectionIndex<TKey> {
	private readonly Func<TItem, TKey> _projection;
	private readonly IItemChecksummer<TKey> _keyChecksummer;
	private readonly Func<long, TKey> _keyHydrator;
	private readonly IEqualityComparer<TKey> _keyComparer;
	private readonly ChecksumBasedDictionary _keyDictionary;

	public UniqueProjectionChecksumIndex(
		ObjectStream<TItem> objectStream,
		string indexName,
		Func<TItem, TKey> projection,
		IItemChecksummer<TKey> keyChecksummer,
		Func<long, TKey> keyHydrator,
		IEqualityComparer<TKey> keyComparer,
		IndexNullPolicy indexNullPolicy
	) : base(
			objectStream,
			new IndexStorageAttachment<int>(objectStream.Streams, indexName, PrimitiveSerializer<int>.Instance, EqualityComparer<int>.Default),
			indexNullPolicy
		) {
		Guard.ArgumentNotNull(projection, nameof(projection));
		Guard.ArgumentNotNull(keyChecksummer, nameof(keyChecksummer));
		Guard.ArgumentNotNull(keyHydrator, nameof(keyHydrator));
		Guard.ArgumentNotNull(keyComparer, nameof(keyComparer));

		_projection = projection;
		_keyChecksummer = keyChecksummer;
		_keyHydrator = keyHydrator;
		_keyComparer = keyComparer;
		_keyDictionary = new ChecksumBasedDictionary(this);
	}

	public IReadOnlyDictionary<TKey, long> Values {
		get {
			CheckAttached();
			return _keyDictionary;
		}
	}

	public override (TKey, int) ApplyProjection(TItem item) {
		var key = _projection(item);
		var checksum = _keyChecksummer.CalculateChecksum(key);
		return (key, checksum);
	}

	protected override void OnAdding(TItem item, long index, (TKey, int) keyChecksum) {
		if (!IsUnique(keyChecksum, null, out var clashIndex)) 
			throw new InvalidOperationException($"Unable to add {typeof(TItem).ToStringCS()} as a unique projection (checksummed) violation occurs on projected key '{AttachmentID}' with value '{keyChecksum.Item1?.ToString() ?? "NULL"}' ({keyChecksum.Item2}) with index {clashIndex}");
	}

	protected override void OnAdded(TItem item, long index, (TKey, int) keyChecksum) {
		var checksum = keyChecksum.Item2;
		Store.Add(index, checksum);
	}

	protected override void OnUpdating(TItem item, long index, (TKey, int) keyChecksum) {
		if (!IsUnique(keyChecksum, index, out var clashIndex)) 
			throw new InvalidOperationException($"Unable to update {typeof(TItem).ToStringCS()} as a unique projection (checksummed) violation occurs on projected key '{AttachmentID}' with value '{keyChecksum.Item1?.ToString() ?? "NULL"}' ({keyChecksum.Item2}) with index {clashIndex}");
	}

	protected override void OnUpdated(TItem item, long index, (TKey, int) keyChecksum) {
		var checksum = keyChecksum.Item2;
		Store.Update(index, checksum);
	}

	protected override void OnInserting(TItem item, long index, (TKey, int) keyChecksum) {
		if (!IsUnique(keyChecksum, index, out var clashIndex)) 
			throw new InvalidOperationException($"Unable to insert {typeof(TItem).ToStringCS()} as a unique projection (checksummed) violation occurs on projected key ' {AttachmentID}' with value '{keyChecksum.Item1?.ToString() ?? "NULL"}' ({keyChecksum.Item2}) with index {clashIndex}");
	}

	protected override void OnInserted(TItem item, long index, (TKey, int) keyChecksum) {
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

	protected override bool IsNullValue((TKey, int) projection) => projection.Item1 is null;

	private bool IsUnique((TKey, int) keyChecksum, long? exemptIndex, out long clashIndex) {
		if (_keyDictionary.TryGetValue(keyChecksum, out var foundIndex)) {
			if (foundIndex != exemptIndex) {
				clashIndex = foundIndex;
				return false;
			}
		}
		clashIndex = default;
		return true;
	}

	private class ChecksumBasedDictionary : IReadOnlyDictionary<TKey, long> {
		private readonly UniqueProjectionChecksumIndex<TItem, TKey> _parent;
		
		public ChecksumBasedDictionary(UniqueProjectionChecksumIndex<TItem, TKey> parent) {
			_parent = parent;
		}


		public IEnumerator<KeyValuePair<TKey, long>> GetEnumerator() {
			throw new NotImplementedException();
		}

		IEnumerator IEnumerable.GetEnumerator() {
			return GetEnumerator();
		}

		public int Count => _parent.Store.Count;

		public bool ContainsKey(TKey key) 
			=> TryGetValue(key, out _);

		public bool TryGetValue(TKey key, out long value) {
			var checksum = _parent._keyChecksummer.CalculateChecksum(key);
			return TryGetValue((key, checksum), out value);
		}

		public bool TryGetValue((TKey, int) keyChecksum, out long value) {
			var key = keyChecksum.Item1;
			var checksum = keyChecksum.Item2;
			var indexCandidates = _parent.Store[checksum];
			// TODO: optimization possible here, if matches.Length == 1 skip key comparison (most cases)
			var matches = indexCandidates.Where(x => _parent._keyComparer.Equals(_parent._keyHydrator(x), key)).ToArray();
			switch (matches.Length) {
				case 0:
					value = -1;
					return false;
				case 1:
					value = matches[0];
					return true;
				default:
					throw new InvalidOperationException("Multiple keys found in dictionary (data corruption)");
			}
		}

		public long this[TKey key] {
			get {
				if (!TryGetValue(key, out long value)) 
					throw new InvalidOperationException($"Key not found '{key}'");
				return value;
			}
		}

		public IEnumerable<TKey> Keys => _parent.Store.Select(grouping => _parent._keyHydrator(grouping.Key));

		public IEnumerable<long> Values => _parent.Store.SelectMany(x => x);
	}

}



