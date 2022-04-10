using System.Collections.Generic;
using System.IO;
using System.Text;
using Sphere10.Framework.Collections;

namespace Sphere10.Framework;

/// <summary>
/// An <see cref="IExtendedList{T}"/> of <see cref="TItem"/> mapped onto a <see cref="Stream"/> which also maintains an <see cref="IMerkleTree"/> of it's items.
/// </summary>
/// <remarks>The stream mapping is achieved via use of an internal <see cref="IClusteredList{TItem}"/></remarks>
/// <typeparam name="TItem"></typeparam>
public class StreamMappedMerkleList<TItem> : MerkleListAdapter<TItem> {

	public StreamMappedMerkleList(Stream rootStream, int clusterSize, CHF hashAlgorithm, IItemSerializer<TItem> itemSerializer = null, IEqualityComparer<TItem> itemComparer = null, ClusteredStoragePolicy policy = ClusteredStoragePolicy.Default, Endianness endianness = Endianness.LittleEndian)
		: this(
			  rootStream,
			  clusterSize,
			  hashAlgorithm,
			  new ItemHasher<TItem>(hashAlgorithm, itemSerializer ?? ItemSerializer<TItem>.Default),
			  itemSerializer ?? ItemSerializer<TItem>.Default,
			  itemComparer,
			  policy,
			  endianness
			) {
	}

	public StreamMappedMerkleList(Stream rootStream, int clusterSize, CHF hashAlgorithm, IItemHasher<TItem> hasher, IItemSerializer<TItem> itemSerializer = null, IEqualityComparer<TItem> itemComparer = null, ClusteredStoragePolicy policy = ClusteredStoragePolicy.Default, Endianness endianness = Endianness.LittleEndian)
		: this(
			  new ClusteredList<TItem>(rootStream, clusterSize, itemSerializer, itemComparer, policy, 0, 1, endianness),
			  hasher ?? new ItemHasher<TItem>(hashAlgorithm, itemSerializer),
			  hashAlgorithm
			  ) {
	}

	public StreamMappedMerkleList(IClusteredList<TItem> clusteredList, IItemHasher<TItem> hasher, CHF hashAlgorithm)
		: base(clusteredList, hasher, new ClusteredStorageMerkleTreeStream(clusteredList.Storage, 0, hashAlgorithm)) {
	}

}