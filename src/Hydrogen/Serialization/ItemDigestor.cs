using System.Runtime.CompilerServices;

namespace Hydrogen;

public class ItemDigestor<TItem> : ItemSerializerDecorator<TItem>, IItemDigestor<TItem> {
	private readonly CHF _hashAlgorithm;
	private readonly Endianness _endianness;

	public ItemDigestor(IItemSerializer<TItem> internalSerializer, Endianness endianness = Endianness.LittleEndian)
		: this(CHF.SHA2_256, internalSerializer, endianness) {
	}

	public ItemDigestor(CHF hashAlgorithm, IItemSerializer<TItem> internalSerializer = null, Endianness endianness = Endianness.LittleEndian)
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