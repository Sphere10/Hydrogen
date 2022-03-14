using System.Runtime.CompilerServices;

namespace Sphere10.Framework {

	public class ItemHasher<TItem> : ItemSerializerDecorator<TItem>, IItemHasher<TItem> {
		private readonly CHF _hashAlgorithm;

		public ItemHasher(IItemSerializer<TItem> internalSerializer)
			: this(CHF.SHA2_256, internalSerializer) {
		}

		public ItemHasher(CHF hashAlgorithm, IItemSerializer<TItem> internalSerializer)
			: base(internalSerializer) {
			_hashAlgorithm = hashAlgorithm;
			DigestLength = Hashers.GetDigestSizeBytes(_hashAlgorithm);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public byte[] Hash(TItem @object) => Hashers.Hash(_hashAlgorithm, @object, this);

		public int DigestLength { get; }
	}

}