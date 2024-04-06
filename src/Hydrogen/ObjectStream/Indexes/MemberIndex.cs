// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Collections.Generic;
using System.Linq;

namespace Hydrogen;

internal class MemberIndex<TItem, TKey> : IndexBase<TItem, TKey, MemberStore<TKey>>, IMemberIndex<TKey> {

	public MemberIndex(ObjectStream<TItem> objectStream, long reservedStreamIndex, Func<TItem, TKey> projection, IEqualityComparer<TKey> keyComparer, IItemSerializer<TKey> keySerializer)
		: base(
			objectStream,
			projection,
			new MemberStore<TKey>(objectStream.Streams, reservedStreamIndex, keyComparer, keySerializer)
		) {
	}

	public virtual ILookup<TKey, long> Lookup {
		get {
			CheckAttached();
			return Store.Lookup;
		}
	}

}

