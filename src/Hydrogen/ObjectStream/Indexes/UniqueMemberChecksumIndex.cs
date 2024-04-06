// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Hydrogen;

/// <summary>
/// An index that only stores the checksum of the key rather than the key itself and relies on fetching the key from the objectStream when needed for comparisons.
/// </summary>
/// <typeparam name="TItem"></typeparam>
/// <typeparam name="TKey"></typeparam>
internal class UniqueMemberChecksumIndex<TItem, TKey> : IndexBase<TItem, int, MemberStore<int>>, IUniqueMemberIndex<TKey> {
	private readonly Func<TItem, TKey> _projection;
	private readonly IItemChecksummer<TKey> _keyChecksummer;
	private readonly Func<long, TKey> _keyFetcher;
	private readonly IEqualityComparer<TKey> _keyComparer;
	private readonly KeyChecksumDictionary _checksummedDictionary;

	public UniqueMemberChecksumIndex(ObjectStream<TItem> objectStream, long reservedStreamIndex, Func<TItem, TKey> projection, IItemChecksummer<TKey> keyChecksummer, Func<long, TKey> keyFetcher, IEqualityComparer<TKey> keyComparer)
		: base(
			objectStream,
			x => keyChecksummer.CalculateChecksum(projection.Invoke(x)),
			new MemberStore<int>(objectStream.Streams, reservedStreamIndex, EqualityComparer<int>.Default, PrimitiveSerializer<int>.Instance)
		) {
		Guard.ArgumentNotNull(objectStream, nameof(objectStream));
		Guard.ArgumentNotNull(projection, nameof(projection));
		Guard.ArgumentNotNull(keyChecksummer, nameof(keyChecksummer));
		Guard.ArgumentNotNull(keyFetcher, nameof(keyFetcher));
		Guard.ArgumentNotNull(keyComparer, nameof(keyComparer));

		_projection = projection;
		_keyChecksummer = keyChecksummer;
		_keyFetcher = keyFetcher;
		_keyComparer = keyComparer;
		_checksummedDictionary = new KeyChecksumDictionary(this);
	}

	public IReadOnlyDictionary<TKey, long> Dictionary {
		get {
			CheckAttached();
			return _checksummedDictionary;
		}
	}

	protected override void OnAdding(TItem item, long index, int checksum) {
		base.OnAdding(item, index, checksum);
		if (!IsUnique(item, checksum, null, out var clashIndex)) 
			throw new InvalidOperationException($"Unable to add {typeof(TItem).ToStringCS()} as a unique key (checksummed) violation occurs with item at {clashIndex}");
	}

	protected override void OnUpdating(TItem item, long index, int checksum) {
		base.OnUpdating(item, index, checksum);
		if (!IsUnique(item, checksum, index, out var clashIndex)) 
			throw new InvalidOperationException($"Unable to update {typeof(TItem).ToStringCS()} at {index} as a unique key (checksummed) violation occurs with item at {clashIndex}");
	}

	protected override void OnInserting(TItem item, long index, int checksum) {
		base.OnInserting(item, index, checksum);
		if (!IsUnique(item, checksum, index, out var clashIndex)) 
			throw new InvalidOperationException($"Unable to insert {typeof(TItem).ToStringCS()} at {index} as a unique key (checksummed) violation occurs with item at {clashIndex}");
	}

	private bool IsUnique(TItem item, int checksum, long? exemptIndex, out long clashIndex) {
		var key = _projection(item);
		if (_checksummedDictionary.TryGetValue(key, checksum, out var foundIndex)) {
			if (foundIndex != exemptIndex) {
				clashIndex = foundIndex;
				return false;
			}
		}
		clashIndex = default;
		return true;
	}

	private class KeyChecksumDictionary : IReadOnlyDictionary<TKey, long> {
		private readonly UniqueMemberChecksumIndex<TItem, TKey> _parent;
		
		public KeyChecksumDictionary(UniqueMemberChecksumIndex<TItem, TKey> parent) {
			_parent = parent;
		}


		public IEnumerator<KeyValuePair<TKey, long>> GetEnumerator() {
			throw new NotImplementedException();
		}

		IEnumerator IEnumerable.GetEnumerator() {
			return GetEnumerator();
		}

		public int Count => _parent.Store.Lookup.Count;

		public bool ContainsKey(TKey key) 
			=> TryGetValue(key, out _);

		public bool TryGetValue(TKey key, out long value) {
			var keyChecksum = _parent._keyChecksummer.CalculateChecksum(key);
			return TryGetValue(key, keyChecksum, out value);
		}

		public bool TryGetValue(TKey key, int keyChecksum, out long value) {
			var candidates = _parent.Store.Lookup[keyChecksum];
			var matches = candidates.Where(x => _parent._keyComparer.Equals(_parent._keyFetcher(x), key)).ToArray();
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

		public IEnumerable<TKey> Keys => _parent
			.Store
			.Lookup
			.GetEnumerator()
			.AsEnumerable()
			.Select(x => _parent._keyFetcher(x.Key));

		public IEnumerable<long> Values => _parent.Store.Lookup.GetEnumerator().AsEnumerable().Select(x => x.Single());
	}

}



