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

namespace Hydrogen.ObjectSpaces;

/// <summary>
/// An index that only stores the checksum of the key rather than the key itself and relies on fetching the key from the container when needed for comparisons.
/// </summary>
/// <typeparam name="TItem"></typeparam>
/// <typeparam name="TKey"></typeparam>
internal class UniqueKeyChecksumIndex<TItem, TKey> : IndexBase<TItem, int, NonUniqueKeyStore<int>> {
	private readonly IItemChecksummer<TKey> _keyChecksummer;
	private readonly Func<long, TKey> _keyFetcher;
	private readonly IEqualityComparer<TKey> _keyComparer;
	private readonly KeyChecksumDictionary _checksummedDictionary;

	public UniqueKeyChecksumIndex(ObjectContainer<TItem> container, long reservedStreamIndex, Func<TItem, TKey> projection, IItemChecksummer<TKey> keyChecksummer, Func<long, TKey> keyFetcher, IEqualityComparer<TKey> keyComparer)
		: base(
			container,
			x => keyChecksummer.CalculateChecksum(projection.Invoke(x)),
			new NonUniqueKeyStore<int>(container, reservedStreamIndex, EqualityComparer<int>.Default, PrimitiveSerializer<int>.Instance)
		) {
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

	private class KeyChecksumDictionary : IReadOnlyDictionary<TKey, long> {
		private readonly UniqueKeyChecksumIndex<TItem, TKey> _parent;
		
		public KeyChecksumDictionary(UniqueKeyChecksumIndex<TItem, TKey> parent) {
			_parent = parent;
		}


		public IEnumerator<KeyValuePair<TKey, long>> GetEnumerator() {
			throw new NotImplementedException();
		}

		IEnumerator IEnumerable.GetEnumerator() {
			return GetEnumerator();
		}

		public int Count => _parent.KeyStore.Lookup.Count;

		public bool ContainsKey(TKey key) 
			=> TryGetValue(key, out _);

		public bool TryGetValue(TKey key, out long value) {
			var keyChecksum = _parent._keyChecksummer.CalculateChecksum(key);
			var candidates = _parent.KeyStore.Lookup[keyChecksum];
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
			.KeyStore
			.Lookup
			.GetEnumerator()
			.AsEnumerable()
			.Select(x => _parent._keyFetcher(x.Key));

		public IEnumerable<long> Values => _parent.KeyStore.Lookup.GetEnumerator().AsEnumerable().Select(x => x.Single());
	}

}



