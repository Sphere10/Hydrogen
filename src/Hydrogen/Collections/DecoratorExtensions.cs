using System;
using System.Runtime.CompilerServices;

namespace Hydrogen;

public static class DecoratorExtensions {
	
	#region IExtendedList

	public static SynchronizedExtendedList<T, TInnerList> AsSynchronized<T, TInnerList>(this TInnerList list) where TInnerList : IExtendedList<T> 
		=> new(list);
	
	public static ObservableExtendedList<T, TInnerList> AsObservable<T, TInnerList>(this TInnerList list) where TInnerList : IExtendedList<T> 
		=> new(list);

	public static StackList<T, TInnerList> AsStack<T, TInnerList>(this TInnerList list) where TInnerList : IExtendedList<T> 
		=> new(list);

	public static BoundedList<T, TInnerList> AsBounded<T, TInnerList>(this TInnerList list, long startIndex) where TInnerList : IExtendedList<T> 
		=> new(startIndex, list);

	public static MerkleListAdapter<T, TInnerList> AsMerkleized<T, TInnerList>(this TInnerList list) where TInnerList : IExtendedList<T> 
		=> new(list);
		
	public static MerkleListAdapter<T, TInnerList> AsMerkleized<T, TInnerList>(this TInnerList list, CHF hashAlgorithm, Endianness endianness = HydrogenDefaults.Endianness) where TInnerList : IExtendedList<T> 
		=> new(list, hashAlgorithm, endianness);
	
	public static MerkleListAdapter<T, TInnerList> AsMerkleized<T, TInnerList>(this TInnerList list, IItemSerializer<T> serializer, CHF hashAlgorithm, Endianness endianness = HydrogenDefaults.Endianness) where TInnerList : IExtendedList<T> 
		=> new(list, serializer, hashAlgorithm, endianness);

	public static MerkleListAdapter<T, TInnerList> AsMerkleized<T, TInnerList>(this TInnerList list, IItemHasher<T> hasher, IDynamicMerkleTree merkleTreeImpl) where TInnerList : IExtendedList<T> 
		=> new(list, hasher, merkleTreeImpl);

	public static UpdateOnlyList<T, TInnerList> AsUpdateOnly<T, TInnerList>(this TInnerList internalStore, Func<T> itemActivator) where TInnerList : IExtendedList<T> 
		=> new(internalStore, itemActivator);

	public static UpdateOnlyList<T, TInnerList> AsUpdateOnly<T, TInnerList>(this TInnerList internalStore, long preAllocatedItemCount, Func<T> itemActivator) where TInnerList : IExtendedList<T> 
		=> new(internalStore, preAllocatedItemCount, itemActivator);

	public static UpdateOnlyList<T, TInnerList> AsUpdateOnly<T, TInnerList>(this TInnerList internalStore, PreAllocationPolicy preAllocationPolicy, long blockSize, Func<T> itemActivator) where TInnerList : IExtendedList<T> 
		=> new(internalStore, preAllocationPolicy, blockSize, itemActivator);

	public static UpdateOnlyList<T, TInnerList> AsUpdateOnly<T, TInnerList>(this TInnerList internalStore, long existingItemsInInternalStore, PreAllocationPolicy preAllocationPolicy, long blockSize, Func<T> itemActivator) where TInnerList : IExtendedList<T> 
		=> new(internalStore, existingItemsInInternalStore, preAllocationPolicy, blockSize, itemActivator);

	#endregion

	#region IStreamMappedList
	
	public static StreamMappedMerkleList<T, TInnerList> AsMerkleized<T, TInnerList>(this TInnerList list, IItemHasher<T> hasher, CHF hashAlgorithm, int merkleTreeStreamIndex) where TInnerList : IStreamMappedList<T> 
		=> new(list, hasher, hashAlgorithm, merkleTreeStreamIndex);

	#endregion
}
