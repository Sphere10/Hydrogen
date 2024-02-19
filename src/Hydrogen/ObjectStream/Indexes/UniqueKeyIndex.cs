// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Collections.Generic;

namespace Hydrogen;

internal class UniqueKeyIndex<TItem, TKey> : IndexBase<TItem, TKey, UniqueKeyStore<TKey>> {

	public UniqueKeyIndex(ObjectStream<TItem> objectStream, long reservedStreamIndex, Func<TItem, TKey> projection, IEqualityComparer<TKey> keyComparer, IItemSerializer<TKey> keySerializer)
		: base( objectStream, projection, new UniqueKeyStore<TKey>(objectStream.Streams, reservedStreamIndex, keyComparer, keySerializer)) {
	}

	public IReadOnlyDictionary<TKey, long> Dictionary => KeyStore.Dictionary;

}

