using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Hydrogen;

/// <summary>
/// Implements various extensions for activating decorators, wrappers, adapters.
/// </summary>
public static class DecoratorExtensions {

	#region IReadOnlyList

	public static IReadOnlyList<TTo> WithProjection<TFrom, TTo>(this IReadOnlyList<TFrom> list, Func<TFrom, TTo> projection) => new ReadOnlyListProjection<TFrom,TTo>(list, projection);

	#endregion
	
	#region IList

	public static IExtendedList<T> AsExtended<T>(this IList<T> list) => new ExtendedListAdapter<T>(list);

	public static IList<T> AsGenericList<T>(this IList list) => new GenericListAdapter<T>(list);

	public static IList AsLegacyList<T>(this IList<T> list) => new LegacyListAdapter<T>(list);

	public static SynchronizedList<T, TInnerList> AsSynchronized_<T, TInnerList>(this TInnerList list) where TInnerList : IList<T> => new (list);

	public static SynchronizedList<T> AsSynchronized_<T>(this IList<T> list) => new (list);

	public static IReadOnlyList<T> AsReadOnly<T>(this IList<T> list) => new ReadOnlyListAdapter<T>(list);

	#endregion
	
	#region IExtendedList

	#region AsSynchronized

	public static SynchronizedExtendedList<T, TInnerList> AsSynchronized<T, TInnerList>(this TInnerList list) where TInnerList : IExtendedList<T> => new(list);

	public static SynchronizedExtendedList<T> AsSynchronized<T>(this IExtendedList<T> list) => new(list);
	
	#endregion

	#region AsObservable
	
	public static ObservableExtendedList<T, TInnerList> AsObservable<T, TInnerList>(this TInnerList list) where TInnerList : IExtendedList<T> => new(list);

	public static ObservableExtendedList<T> AsObservable<T>(this IExtendedList<T> list) => new(list);

	#endregion

	#region WithProjection

	public static IExtendedList<TTo> WithProjection<TFrom, TTo>(this IExtendedList<TFrom> source, Func<TFrom, TTo> projection, Func<TTo, TFrom> inverseProjection)
		=> new ProjectedExtendedList<TFrom,TTo>(source, projection, inverseProjection);

	#endregion

	#region AsStack

	public static StackList<T, TInnerList> AsStack<T, TInnerList>(this TInnerList list) where TInnerList : IExtendedList<T> => new(list);

	public static StackList<T> AsStack<T>(this IExtendedList<T> list) => new(list);

	#endregion

	#region AsBounded

	public static BoundedList<T, TInnerList> AsBounded<T, TInnerList>(this TInnerList list, long startIndex) where TInnerList : IExtendedList<T> => new(startIndex, list);

	public static BoundedList<T> AsBounded<T>(this IExtendedList<T> list, long startIndex) => new(startIndex, list);

	#endregion

	#region AsMerkleized

	public static MerkleListAdapter<T, TInnerList> AsMerkleized<T, TInnerList>(this TInnerList list) where TInnerList : IExtendedList<T> => new(list);

	public static MerkleListAdapter<T> AsMerkleized<T>(this IExtendedList<T>  list) => new(list);
		
	public static MerkleListAdapter<T, TInnerList> AsMerkleized<T, TInnerList>(this TInnerList list, CHF hashAlgorithm, Endianness endianness = HydrogenDefaults.Endianness) where TInnerList : IExtendedList<T> => new(list, hashAlgorithm, endianness);

	public static MerkleListAdapter<T> AsMerkleized<T>(this IExtendedList<T> list, CHF hashAlgorithm) => new(list, hashAlgorithm);

	public static MerkleListAdapter<T> AsMerkleized<T>(this IExtendedList<T> internalList, IItemSerializer<T> serializer, CHF hashAlgorithm) => new(internalList, serializer, hashAlgorithm);

	public static MerkleListAdapter<T, TInnerList> AsMerkleized<T, TInnerList>(this TInnerList list, IItemSerializer<T> serializer, CHF hashAlgorithm, Endianness endianness = HydrogenDefaults.Endianness) where TInnerList : IExtendedList<T> => new(list, serializer, hashAlgorithm, endianness);

	public static MerkleListAdapter<T, TInnerList> AsMerkleized<T, TInnerList>(this TInnerList list, IItemHasher<T> hasher, IDynamicMerkleTree merkleTreeImpl) where TInnerList : IExtendedList<T> => new(list, hasher, merkleTreeImpl);

	public static MerkleListAdapter<T> AsMerkleized<T>(this IExtendedList<T> list, IItemHasher<T> hasher, IDynamicMerkleTree merkleTreeImpl) => new(list, hasher, merkleTreeImpl);

	#endregion

	#region AsUpdateOnly

	public static UpdateOnlyList<T, TInnerList> AsUpdateOnly<T, TInnerList>(this TInnerList internalStore, Func<T> itemActivator) where TInnerList : IExtendedList<T> => new(internalStore, itemActivator);

	public static UpdateOnlyList<T> AsUpdateOnly<T>(this IExtendedList<T> internalStore, long existingItemsInStore, PreAllocationPolicy preAllocationPolicy, long blockSize, Func<T> itemActivator) => new(internalStore, existingItemsInStore, preAllocationPolicy, blockSize, itemActivator);

	public static UpdateOnlyList<T, TInnerList> AsUpdateOnly<T, TInnerList>(this TInnerList internalStore, long preAllocatedItemCount, Func<T> itemActivator) where TInnerList : IExtendedList<T> => new(internalStore, preAllocatedItemCount, itemActivator);

	public static UpdateOnlyList<T, TInnerList> AsUpdateOnly<T, TInnerList>(this TInnerList internalStore, PreAllocationPolicy preAllocationPolicy, long blockSize, Func<T> itemActivator) where TInnerList : IExtendedList<T> => new(internalStore, preAllocationPolicy, blockSize, itemActivator);

	public static UpdateOnlyList<T, TInnerList> AsUpdateOnly<T, TInnerList>(this TInnerList internalStore, long existingItemsInInternalStore, PreAllocationPolicy preAllocationPolicy, long blockSize, Func<T> itemActivator) where TInnerList : IExtendedList<T> => new(internalStore, existingItemsInInternalStore, preAllocationPolicy, blockSize, itemActivator);

	#endregion

	#region AsSorted

	public static ISortedList<T> AsSorted<T>(this IExtendedList<T> list) => new SortedList<T>(list);

	#endregion

	#endregion

	#region IStreamMappedList
	
	public static StreamMappedMerkleList<T, TInnerList> AsMerkleized<T, TInnerList>(this TInnerList list, IItemHasher<T> hasher, CHF hashAlgorithm, int merkleTreeStreamIndex) where TInnerList : IStreamMappedList<T> => new(list, hasher, hashAlgorithm, merkleTreeStreamIndex);

	public static StreamMappedMerkleList<T> AsMerkleized<T>(this IStreamMappedList<T> list, IItemHasher<T> hasher, CHF hashAlgorithm, int merkleTreeStreamIndex) => new(list, hasher, hashAlgorithm, merkleTreeStreamIndex);

	#endregion

	#region IMemoryPagedBuffer

	public static MerklePagedBuffer AsMerkleizedBuffer(this IMemoryPagedBuffer buffer, CHF hashAlgorithm) => new(buffer, hashAlgorithm);

	public static MerklePagedBuffer AsMerkleizedBuffer(this IMemoryPagedBuffer buffer, IDynamicMerkleTree merkleTree) => new(buffer, merkleTree);

	#endregion

	#region IDictionary
	
	#region AsSynchronized

	public static SynchronizedDictionary<TKey, TValue, TInner> AsSynchronized<TInner, TKey, TValue>(this TInner dictionary) where TInner : IDictionary<TKey, TValue> => new(dictionary);

	public static SynchronizedDictionary<TKey, TValue> AsSynchronized<TKey, TValue>(this IDictionary<TKey, TValue> dictionary) => new(dictionary);
	
	#endregion

	#region AsObservable

	public static ObservableDictionary<TKey, TValue, TInner> AsObservable<TKey, TValue, TInner>(this TInner dictionary) where TInner : IDictionary<TKey, TValue> => new(dictionary);

	public static ObservableDictionary<TKey, TValue> AsObservable<TKey, TValue>(this IDictionary<TKey, TValue> dictionary) => new(dictionary);


	#endregion

	#region AsRepository

	public static DictionaryRepositoryAdapter<TEntity, TIdentity, TInner> AsRepository<TEntity, TIdentity, TInner>(this TInner dictionary, Func<TEntity, TIdentity> identityFunc) where TInner : IDictionary<TIdentity, TEntity> => new(dictionary, identityFunc);

	public static DictionaryRepositoryAdapter<TEntity, TIdentity> AsRepository<TEntity, TIdentity>(this IDictionary<TIdentity, TEntity> dictionary, Func<TEntity, TIdentity> identityFunc) => new(dictionary, identityFunc);

	#endregion

	#endregion

	#region ISet

	#region AsSynchronized

	public static SynchronizedSet<T, TInner> AsSynchronizedSet<T, TInner>(this TInner set) where TInner : ISet<T> => new(set);

	public static SynchronizedSet<T> AsSynchronizedSet<T>(this ISet<T> list) => new(list);

	#endregion

	#endregion

	#region IStreamMappedHashSet
	
	public static StreamMappedMerkleHashSet<T, TInner> AsMerkleized<T, TInner>(this TInner set, IMerkleTree merkleTree) where TInner : IStreamMappedHashSet<T> => new(set, merkleTree);

	public static StreamMappedMerkleHashSet<T> AsMerkleized<T>(this IStreamMappedHashSet<T> set, IMerkleTree merkleTree) => new(set, merkleTree);

	#endregion

	#region IComparer

	public static IComparer<T> AsInverted<T>(this IComparer<T> comparer) {
		if (comparer is InvertedComparer<T> originalAsInverted) {
			return originalAsInverted.OriginalComparer;
		}
		return new InvertedComparer<T>(comparer);
	}

	public static IEqualityComparer<T> AsEqualityComparer<T>(this IComparer<T> comparer, IItemChecksummer<T> checksummer = null) {
		return new EqualityComparerAdapter<T>(comparer, checksummer);
	}

	#endregion
}
