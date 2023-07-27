using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Hydrogen;

public static class DecoratorExtensions {

	#region IList

	public static GenericListAdapter<T> ToGenericList<T>(this IList list) => new(list);

	public static LegacyListAdapter<T> ToLegacyList<T>(this IList<T> list) => new(list);

	public static SynchronizedList<T, TInnerList> ToSynchronized_<T, TInnerList>(this TInnerList list) where TInnerList : IList<T> => new (list);

	#endregion
	
	#region IExtendedList

	#region ToSynchronized

	public static SynchronizedExtendedList<T, TInnerList> ToSynchronized<T, TInnerList>(this TInnerList list) where TInnerList : IExtendedList<T> => new(list);

	public static SynchronizedExtendedList<T> ToSynchronized<T>(this IExtendedList<T> list) => new(list);
	
	#endregion

	#region ToObservable
	
	public static ObservableExtendedList<T, TInnerList> ToObservable<T, TInnerList>(this TInnerList list) where TInnerList : IExtendedList<T> => new(list);

	public static ObservableExtendedList<T> ToObservable<T>(this IExtendedList<T> list) => new(list);

	#endregion

	#region ToStack

	public static StackList<T, TInnerList> ToStack<T, TInnerList>(this TInnerList list) where TInnerList : IExtendedList<T> => new(list);

	public static StackList<T> ToStack<T>(this IExtendedList<T> list) => new(list);

	#endregion

	#region ToBounded

	public static BoundedList<T, TInnerList> ToBounded<T, TInnerList>(this TInnerList list, long startIndex) where TInnerList : IExtendedList<T> => new(startIndex, list);

	public static BoundedList<T> ToBounded<T>(this IExtendedList<T> list, long startIndex) => new(startIndex, list);

	#endregion

	#region ToMerkleized

	public static MerkleListAdapter<T, TInnerList> ToMerkleized<T, TInnerList>(this TInnerList list) where TInnerList : IExtendedList<T> => new(list);

	public static MerkleListAdapter<T> ToMerkleized<T>(this IExtendedList<T>  list) => new(list);
		
	public static MerkleListAdapter<T, TInnerList> ToMerkleized<T, TInnerList>(this TInnerList list, CHF hashAlgorithm, Endianness endianness = HydrogenDefaults.Endianness) where TInnerList : IExtendedList<T> => new(list, hashAlgorithm, endianness);

	public static MerkleListAdapter<T> ToMerkleized<T>(this IExtendedList<T> list, CHF hashAlgorithm) => new(list, hashAlgorithm);

	public static MerkleListAdapter<T> ToMerkleized<T>(this IExtendedList<T> internalList, IItemSerializer<T> serializer, CHF hashAlgorithm) => new(internalList, serializer, hashAlgorithm);

	public static MerkleListAdapter<T, TInnerList> ToMerkleized<T, TInnerList>(this TInnerList list, IItemSerializer<T> serializer, CHF hashAlgorithm, Endianness endianness = HydrogenDefaults.Endianness) where TInnerList : IExtendedList<T> => new(list, serializer, hashAlgorithm, endianness);

	public static MerkleListAdapter<T, TInnerList> ToMerkleized<T, TInnerList>(this TInnerList list, IItemHasher<T> hasher, IDynamicMerkleTree merkleTreeImpl) where TInnerList : IExtendedList<T> => new(list, hasher, merkleTreeImpl);

	public static MerkleListAdapter<T> ToMerkleized<T>(this IExtendedList<T> list, IItemHasher<T> hasher, IDynamicMerkleTree merkleTreeImpl) => new(list, hasher, merkleTreeImpl);

	#endregion

	#region ToUpdateOnly

	public static UpdateOnlyList<T, TInnerList> ToUpdateOnly<T, TInnerList>(this TInnerList internalStore, Func<T> itemActivator) where TInnerList : IExtendedList<T> => new(internalStore, itemActivator);

	public static UpdateOnlyList<T> ToUpdateOnly<T>(this IExtendedList<T> internalStore, long existingItemsInStore, PreAllocationPolicy preAllocationPolicy, long blockSize, Func<T> itemActivator) => new(internalStore, existingItemsInStore, preAllocationPolicy, blockSize, itemActivator);

	public static UpdateOnlyList<T, TInnerList> ToUpdateOnly<T, TInnerList>(this TInnerList internalStore, long preAllocatedItemCount, Func<T> itemActivator) where TInnerList : IExtendedList<T> => new(internalStore, preAllocatedItemCount, itemActivator);

	public static UpdateOnlyList<T, TInnerList> ToUpdateOnly<T, TInnerList>(this TInnerList internalStore, PreAllocationPolicy preAllocationPolicy, long blockSize, Func<T> itemActivator) where TInnerList : IExtendedList<T> => new(internalStore, preAllocationPolicy, blockSize, itemActivator);

	public static UpdateOnlyList<T, TInnerList> ToUpdateOnly<T, TInnerList>(this TInnerList internalStore, long existingItemsInInternalStore, PreAllocationPolicy preAllocationPolicy, long blockSize, Func<T> itemActivator) where TInnerList : IExtendedList<T> => new(internalStore, existingItemsInInternalStore, preAllocationPolicy, blockSize, itemActivator);

	#endregion

	#endregion

	#region IStreamMappedList
	
	public static StreamMappedMerkleList<T, TInnerList> ToMerkleized<T, TInnerList>(this TInnerList list, IItemHasher<T> hasher, CHF hashAlgorithm, int merkleTreeStreamIndex) where TInnerList : IStreamMappedList<T> => new(list, hasher, hashAlgorithm, merkleTreeStreamIndex);

	public static StreamMappedMerkleList<T> ToMerkleized<T>(this IStreamMappedList<T> list, IItemHasher<T> hasher, CHF hashAlgorithm, int merkleTreeStreamIndex) => new(list, hasher, hashAlgorithm, merkleTreeStreamIndex);

	#endregion

	#region IMemoryPagedBuffer

	public static MerklePagedBuffer ToMerkleizedBuffer(this IMemoryPagedBuffer buffer, CHF hashAlgorithm) => new(buffer, hashAlgorithm);

	public static MerklePagedBuffer ToMerkleizedBuffer(this IMemoryPagedBuffer buffer, IDynamicMerkleTree merkleTree) => new(buffer, merkleTree);

	#endregion

	#region IDictionary
	
	#region ToSynchronized

	public static SynchronizedDictionary<TKey, TValue, TInner> ToSynchronized<TInner, TKey, TValue>(this TInner dictionary) where TInner : IDictionary<TKey, TValue> => new(dictionary);

	public static SynchronizedDictionary<TKey, TValue> ToSynchronized<TKey, TValue>(this IDictionary<TKey, TValue> dictionary) => new(dictionary);
	
	#endregion

	#region ToObservable

	public static ObservableDictionary<TKey, TValue, TInner> ToObservable<TKey, TValue, TInner>(this TInner dictionary) where TInner : IDictionary<TKey, TValue> => new(dictionary);

	public static ObservableDictionary<TKey, TValue> ToObservable<TKey, TValue>(this IDictionary<TKey, TValue> dictionary) => new(dictionary);


	#endregion

	#region ToRepository

	public static DictionaryRepositoryAdapter<TEntity, TIdentity, TInner> ToRepository<TEntity, TIdentity, TInner>(this TInner dictionary, Func<TEntity, TIdentity> identityFunc) where TInner : IDictionary<TIdentity, TEntity> => new(dictionary, identityFunc);

	public static DictionaryRepositoryAdapter<TEntity, TIdentity> ToRepository<TEntity, TIdentity>(this IDictionary<TIdentity, TEntity> dictionary, Func<TEntity, TIdentity> identityFunc) => new(dictionary, identityFunc);

	#endregion

	#endregion

	#region ISet

	#region ToSynchronized

	public static SynchronizedSet<T, TInner> ToSynchronizedSet<T, TInner>(this TInner set) where TInner : ISet<T> => new(set);

	public static SynchronizedSet<T> ToSynchronizedSet<T>(this ISet<T> list) => new(list);

	#endregion

	#endregion

	#region IStreamMappedHashSet
	
	public static StreamMappedMerkleHashSet<T, TInner> ToMerkleized<T, TInner>(this TInner set, IMerkleTree merkleTree) where TInner : IStreamMappedHashSet<T> => new(set, merkleTree);

	public static StreamMappedMerkleHashSet<T> ToMerkleized<T>(this IStreamMappedHashSet<T> set, IMerkleTree merkleTree) => new(set, merkleTree);

	#endregion

}
