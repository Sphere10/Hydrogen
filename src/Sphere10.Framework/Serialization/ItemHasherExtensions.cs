using System;
using System.ComponentModel;
using System.IO;

namespace Sphere10.Framework {

	public static class ItemHasherExtensions {

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