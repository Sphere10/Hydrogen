using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Runtime.CompilerServices;
using static Hydrogen.AMS;

namespace Hydrogen;

/// <summary>
/// Implements various extensions for activating decorators, wrappers, adapters.
/// </summary>
public static class DecoratorExtensions {

	#region ICollection

	public static ICollection<TProjection> AsProjection<T, TProjection>(this ICollection<T> collection, Func<T, TProjection> projection, Func<TProjection, T> inverseProjection) => new ProjectedCollection<T, TProjection>(collection, projection, inverseProjection);

	public static IExtendedCollection<T> AsExtended<T>(this ICollection<T> collection) => new ExtendedCollectionAdapter<T>(collection);
	
	#endregion

	#region IReadOnlyList

	public static IReadOnlyList<TTo> WithProjection<TFrom, TTo>(this IReadOnlyList<TFrom> list, Func<TFrom, TTo> projection) => new ReadOnlyListProjection<TFrom, TTo>(list, projection);

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
		=> new ProjectedExtendedList<TFrom, TTo>(source, projection, inverseProjection);

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

	public static MerkleListAdapter<T> AsMerkleized<T>(this IExtendedList<T> list) => new(list);
		
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

	#region AsProjection

	public static IDictionary<TProjectedKey, TProjectedValue> WithProjection<TKey, TValue, TProjectedKey, TProjectedValue>(
		this IDictionary<TKey, TValue> dictionary,
		Func<TKey, TProjectedKey> keyProjection,
		Func<TProjectedKey, TKey> inverseKeyProjection,
		Func<TValue, TProjectedValue> valueProjection,
		Func<TProjectedValue, TValue> inverseValueProjection
	) => new ProjectedDictionary<TKey, TValue, TProjectedKey, TProjectedValue>(dictionary, keyProjection, inverseKeyProjection, valueProjection, inverseValueProjection);

	public static IDictionary<TProjectedKey, TValue> WithKeyProjection<TKey, TValue, TProjectedKey>(this IDictionary<TKey, TValue> dictionary, Func<TKey, TProjectedKey> keyProjection, Func<TProjectedKey, TKey> inverseKeyProjection) 
		=> dictionary.WithProjection(keyProjection, inverseKeyProjection, v => v, v => v);

	public static IDictionary<TKey, TProjectedValue> WithValueProjection<TKey, TValue, TProjectedValue>(this IDictionary<TKey, TValue> dictionary, Func<TValue, TProjectedValue> valueProjection, Func<TProjectedValue, TValue> inverseValueProjection) 
		=> dictionary.WithProjection(k => k, k => k, valueProjection, inverseValueProjection);

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

	#region IEqualityComparer

	public static IEqualityComparer<T> AsNegation<T>(this IEqualityComparer<T> comparer) => new NegatedEqualityComparer<T>(comparer);

	public static IEqualityComparer<TTo> AsProjection<TFrom, TTo>(this IEqualityComparer<TFrom> sourceComparer, Func<TTo, TFrom> inverseProjection) => new ProjectedEqualityComparer<TFrom, TTo>(sourceComparer, inverseProjection);

	public static IEqualityComparer<TTo> AsProjection<TFrom, TTo>(this IEqualityComparer<TFrom> sourceComparer, Func<TFrom, TTo> projection, Func<TTo, TFrom> inverseProjection) => sourceComparer.AsProjection(inverseProjection);

	public static IEqualityComparer<object> AsPacked<TItem>(this IEqualityComparer<TItem> sourceComparer) => PackedEqualityComparer.Pack(sourceComparer);

	public static IEqualityComparer<T> ThenBy<T>(this IEqualityComparer<T> primary, IEqualityComparer<T> secondary) => new CompositeEqualityComparer<T>(primary, secondary);

	public static IEqualityComparer<T> ThenBy<T, TKey>(this IEqualityComparer<T> primary, Func<T, TKey> projection, IEqualityComparer<TKey> keyComparer = null) => new CompositeEqualityComparer<T>(primary, new ProjectionEqualityComparer<T,TKey>(projection, keyComparer));
	
	#endregion

	#region IComparer
	/// <summary>
	/// Reverses the original comparer; if it was already a reverse comparer,
	/// the previous version was reversed (rather than reversing twice).
	/// In other words, for any comparer X, X==X.Reverse().Reverse().
	/// </summary>
	public static IComparer<T> AsInverted<T>(this IComparer<T> comparer) {
		if (comparer is InvertedComparer<T> originalAsInverted) {
			return originalAsInverted.OriginalComparer;
		}
		return new InvertedComparer<T>(comparer);
	}

	public static IEqualityComparer<T> AsEqualityComparer<T>(this IComparer<T> comparer, IItemChecksummer<T> checksummer = null) => new EqualityComparerAdapter<T>(comparer, checksummer);

	public static IComparer<TTo> AsProjection<TFrom, TTo>(this IComparer<TFrom> sourceComparer, Func<TTo, TFrom> inverseProjection) => new ProjectedComparer<TFrom, TTo>(sourceComparer, inverseProjection);
	
	public static IComparer<TTo> AsProjection<TFrom, TTo>(this IComparer<TFrom> sourceComparer, Func<TFrom, TTo> projection, Func<TTo, TFrom> inverseProjection) => sourceComparer.AsProjection(inverseProjection);

	public static IComparer<object> AsPacked<TItem>(this IComparer<TItem> sourceComparer) => PackedComparer.Pack(sourceComparer);

	/// <summary>
	/// Combines a comparer with a second comparer to implement composite sort
	/// behaviour.
	/// </summary>
	public static IComparer<T> ThenBy<T>(this IComparer<T> firstComparer, IComparer<T> secondComparer) => new CompositeComparer<T>(firstComparer, secondComparer);

	public static IComparer<T> ThenByDescending<T>(this IComparer<T> firstComparer, IComparer<T> secondComparer) => firstComparer.ThenBy(secondComparer.AsInverted());

	/// <summary>
	/// Combines a comparer with a projection to implement composite sort behaviour.
	/// </summary>
	public static IComparer<T> ThenBy<T, TKey>(this IComparer<T> firstComparer, Func<T, TKey> projection, IComparer<TKey> keyComparer = null) => new CompositeComparer<T>(firstComparer, new ProjectionComparer<T, TKey>(projection, keyComparer));

	public static IComparer<T> ThenByDescending<T, TKey>(this IComparer<T> firstComparer, Func<T, TKey> projection, IComparer<TKey> keyComparer = null) => firstComparer.ThenBy(projection, (keyComparer ?? Comparer<TKey>.Default).AsInverted());

	#endregion

}
