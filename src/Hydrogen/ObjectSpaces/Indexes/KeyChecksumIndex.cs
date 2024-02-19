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
/// An index that only stores the checksum of the key rather than the key itself and relies on fetching the key from the objectStream when needed for comparisons.
/// </summary>
/// <typeparam name="TItem"></typeparam>
/// <typeparam name="TKey"></typeparam>
internal class KeyChecksumIndex<TItem, TKey> : IndexBase<TItem, int, NonUniqueKeyStore<int>> {
	private readonly IItemChecksummer<TKey> _keyChecksummer;
	private readonly Func<long, TKey> _keyFetcher;
	private readonly IEqualityComparer<TKey> _keyComparer;
	private readonly KeyChecksumLookup _checksummedLookup;

	public KeyChecksumIndex(ObjectStream<TItem> objectStream, long reservedStreamIndex, Func<TItem, TKey> projection, IItemChecksummer<TKey> keyChecksummer, Func<long, TKey> keyFetcher, IEqualityComparer<TKey> keyComparer)
		: base(
			objectStream,
			x => keyChecksummer.CalculateChecksum(projection.Invoke(x)),
			new NonUniqueKeyStore<int>(objectStream, reservedStreamIndex, EqualityComparer<int>.Default, PrimitiveSerializer<int>.Instance)
		) {
		_keyChecksummer = keyChecksummer;
		_keyFetcher = keyFetcher;
		_keyComparer = keyComparer;
		_checksummedLookup = new KeyChecksumLookup(this);
	}

	public ILookup<TKey, long> Lookup {
		get {
			CheckAttached();
			return _checksummedLookup;
		}
	}

	private class KeyChecksumLookup : ILookup<TKey, long> {
		private readonly KeyChecksumIndex<TItem, TKey> _parent;
		
		public KeyChecksumLookup(KeyChecksumIndex<TItem, TKey> parent) {
			_parent = parent;
		}

		public IEnumerator<IGrouping<TKey, long>> GetEnumerator()
			=> throw new NotSupportedException("Enumerating checksum stored indices is not supported");

		IEnumerator IEnumerable.GetEnumerator() {
			return GetEnumerator();
		}

		public bool Contains(TKey key)
			=> _parent
			.KeyStore
			.Lookup[_parent._keyChecksummer.CalculateChecksum(key)]
			.Any(x => _parent._keyComparer.Equals(key, _parent._keyFetcher(x)));

		public int Count => _parent.KeyStore.Lookup.Count;

		public IEnumerable<long> this[TKey key]
			=> _parent
				.KeyStore
				.Lookup[_parent._keyChecksummer.CalculateChecksum(key)]
				.Where(x => _parent._keyComparer.Equals(key, _parent._keyFetcher(x)));

	}

}



