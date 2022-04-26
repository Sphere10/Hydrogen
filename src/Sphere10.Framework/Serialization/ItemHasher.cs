using System.Runtime.CompilerServices;

namespace Sphere10.Framework {

	public class ItemHasher<TItem> : ItemSerializerDecorator<TItem>, IItemHasher<TItem> {
		private readonly CHF _hashAlgorithm;
		private readonly Endianness _endianness;

		public ItemHasher(IItemSerializer<TItem> internalSerializer, Endianness endianness = Endianness.LittleEndian)
			: this(CHF.SHA2_256, internalSerializer, endianness) {
		}

		public ItemHasher(CHF hashAlgorithm, IItemSerializer<TItem> internalSerializer = null, Endianness endianness = Endianness.LittleEndian)
			: base(internalSerializer ?? ItemSerializer<TItem>.Default) {
			_hashAlgorithm = hashAlgorithm;
			_endianness = endianness;
			DigestLength = Hashers.GetDigestSizeBytes(_hashAlgorithm);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public byte[] Hash(TItem item) {
			Guard.ArgumentNotNull(item, nameof(item));
			return Hashers.Hash(_hashAlgorithm, item, this, _endianness);
		}

		public int DigestLength { get; }
	}


}