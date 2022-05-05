﻿using System;
using System.ComponentModel;
using System.IO;

namespace Hydrogen {

	public static class ItemHasherExtensions {

		public static IItemHasher<TItem> WithNullHash<TItem>(this IItemHasher<TItem> hasher, CHF chf)
			=> WithNullHash(hasher, Tools.Array.Gen<byte>(Hashers.GetDigestSizeBytes(chf), 0));

		public static IItemHasher<TItem> WithNullHash<TItem>(this IItemHasher<TItem> hasher, byte[] nullHashValue)
			=> WithNullHash<IItemHasher<TItem>, TItem>(hasher, nullHashValue);


		public static IItemHasher<TItem> WithNullHash<TItemHasher, TItem>(this TItemHasher hasher, byte[] nullHashValue)
			where TItemHasher : IItemHasher<TItem> {
			Guard.ArgumentNotNull(hasher, nameof(hasher));
			Guard.ArgumentNotNull(nullHashValue, nameof(nullHashValue));
			return new WithNullValueItemHasher<TItem, TItemHasher>(hasher, nullHashValue);
		}
		
	}
}