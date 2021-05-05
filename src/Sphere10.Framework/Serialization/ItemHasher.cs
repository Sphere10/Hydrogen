using System;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;

namespace Sphere10.Framework {

	public class ItemHasher<TItem> : ItemSerializerDecorator<TItem>, IItemHasher<TItem> {
		private readonly CHF _hashAlgorithm;

		public ItemHasher(IItemSerializer<TItem> internalSerializer)
			: this(CHF.SHA2_256, internalSerializer) {
		}

		public ItemHasher(CHF hashAlgorithm, IItemSerializer<TItem> internalSerializer)
			: base(internalSerializer) {
			_hashAlgorithm = hashAlgorithm;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public byte[] Hash(TItem @object) => Hashers.Hash(_hashAlgorithm, @object, this);
	}

}