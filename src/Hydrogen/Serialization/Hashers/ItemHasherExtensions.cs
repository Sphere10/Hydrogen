// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT NON-AI software license, see the accompanying file
// LICENSE or visit https://sphere10.com/legal/NON-AI-MIT.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

namespace Hydrogen;

public static class ItemHasherExtensions {

	public static IItemHasher<TItem> WithNullHash<TItem>(this IItemHasher<TItem> hasher, CHF chf)
		=> WithNullHash(hasher, Hashers.ZeroHash(chf));

	public static IItemHasher<TItem> WithNullHash<TItem>(this IItemHasher<TItem> hasher, byte[] nullHashValue)
		=> WithNullHash<IItemHasher<TItem>, TItem>(hasher, nullHashValue);


	public static IItemHasher<TItem> WithNullHash<TItemHasher, TItem>(this TItemHasher hasher, byte[] nullHashValue)
		where TItemHasher : IItemHasher<TItem> {
		Guard.ArgumentNotNull(hasher, nameof(hasher));
		Guard.ArgumentNotNull(nullHashValue, nameof(nullHashValue));
		return new WithNullValueItemHasher<TItem>(hasher, nullHashValue);
	}

}
