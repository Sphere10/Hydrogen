using System.Collections.Generic;
using System.IO;
using System.Text;
using Sphere10.Framework.Collections;

namespace Sphere10.Framework;

/// <summary>
/// An <see cref="IExtendedList{T}"/> of <see cref="TItem"/> mapped onto a <see cref="Stream"/> which also maintains an <see cref="IMerkleTree"/> of it's items.
/// </summary>
/// <remarks>The stream mapping is achieved via use of an internal <see cref="IStreamMappedList{TItem}"/></remarks>
/// <typeparam name="TItem"></typeparam>
public class StreamMappedMerkleList<TItem> : MerkleListAdapter<TItem, IStreamMappedList<TItem>>, IStreamMappedList<TItem> {
	private const int MerkleTreeStreamIndex = 0;
	public StreamMappedMerkleList(Stream rootStream, int clusterSize, CHF hashAlgorithm = CHF.SHA2_256, IItemSerializer<TItem> itemSerializer = null, IEqualityComparer<TItem> itemComparer = null, ClusteredStoragePolicy policy = ClusteredStoragePolicy.Default, int recordKeySize = 0, int reservedRecords = 1, Endianness endianness = Endianness.LittleEndian)
		: this(
			  rootStream,
			  clusterSize,
			  hashAlgorithm,
			  new ItemHasher<TItem>(hashAlgorithm, itemSerializer, endianness).WithNullHash(hashAlgorithm),
			  itemSerializer,
			  itemComparer,
			  policy,
			  recordKeySize,
			  reservedRecords,
			  endianness
		) {
	}

	public StreamMappedMerkleList(Stream rootStream, int clusterSize, CHF hashAlgorithm, IItemHasher<TItem> hasher, IItemSerializer<TItem> itemSerializer = null, IEqualityComparer<TItem> itemComparer = null, ClusteredStoragePolicy policy = ClusteredStoragePolicy.Default, int recordKeySize = 0, int reservedRecords = 1, Endianness endianness = Endianness.LittleEndian)
		: this(
			  new StreamMappedList<TItem>(
				  rootStream,
				  clusterSize, 
				  itemSerializer, 
				  itemComparer,
				  policy,
				  recordKeySize,
				  reservedRecords, 
				  endianness
			  ),
			  hasher,
			  hashAlgorithm
		  ) {
		Guard.ArgumentNotNull(hasher, nameof(hasher)); // must be provided with null hash support
		Guard.ArgumentGTE(reservedRecords, 1, nameof(reservedRecords), "Must be greater than 1 to allow storage of merkle-tree");
	}

	public StreamMappedMerkleList(IStreamMappedList<TItem> clusteredList, IItemHasher<TItem> hasher, CHF hashAlgorithm)
		: base(clusteredList, hasher, new ClusteredStorageMerkleTreeStream(clusteredList.Storage, MerkleTreeStreamIndex, hashAlgorithm)) {
		Guard.Ensure(clusteredList.Storage.Header.ReservedRecords > 0, "Clustered storage requires at least 1 reserved stream to store merkle-tree");
	}

	public IClusteredStorage Storage => InternalCollection.Storage;

	public IItemSerializer<TItem> ItemSerializer => InternalCollection.ItemSerializer;

	public IEqualityComparer<TItem> ItemComparer => InternalCollection.ItemComparer;

	public ClusteredStreamScope EnterAddScope(TItem item) {
		InternalMerkleTree.Leafs.Add(ItemHasher.Hash(item));
		return InternalCollection.EnterAddScope(item);
	}

	public ClusteredStreamScope EnterInsertScope(int index, TItem item) {
		InternalMerkleTree.Leafs.Insert(index, ItemHasher.Hash(item));
		return InternalCollection.EnterInsertScope(index, item);
	}

	public ClusteredStreamScope EnterUpdateScope(int index, TItem item) {
		InternalMerkleTree.Leafs.Update(index, ItemHasher.Hash(item));
		return InternalCollection.EnterUpdateScope(index, item);
	}
}


