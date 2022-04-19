using System.Runtime.CompilerServices;

namespace Sphere10.Framework {

	public class ItemHasher<TItem> : ItemSerializerDecorator<TItem>, IItemHasher<TItem> {
		private readonly CHF _hashAlgorithm;

		public ItemHasher(IItemSerializer<TItem> internalSerializer)
			: this(CHF.SHA2_256, internalSerializer) {
		}

		public ItemHasher(CHF hashAlgorithm, IItemSerializer<TItem> internalSerializer = null)
			: base(internalSerializer ?? ItemSerializer<TItem>.Default) {
			_hashAlgorithm = hashAlgorithm;
			DigestLength = Hashers.GetDigestSizeBytes(_hashAlgorithm);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public byte[] Hash(TItem item) {
			Guard.ArgumentNotNull(item, nameof(item));
			return Hashers.Hash(_hashAlgorithm, item, this);
		}

		public int DigestLength { get; }
	}


}