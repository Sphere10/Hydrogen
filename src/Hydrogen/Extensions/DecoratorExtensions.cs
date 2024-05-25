using Hydrogen.Mapping;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Hydrogen;

/// <summary>
/// Implements various extensions for activating decorators, wrappers, adapters.
/// </summary>
public static partial class DecoratorExtensions {

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

	public static StackAdapter<T> AsStack<T>(this IExtendedList<T> list) => new(list);

	#endregion

	#region AsStackList

	public static StackList<T, TInnerList> AsStackList<T, TInnerList>(this TInnerList list) where TInnerList : IExtendedList<T> => new(list);

	public static StackList<T> AsStackList<T>(this IExtendedList<T> list) => new(list);

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

	//#region IStreamMappedList
	
	//public static StreamMappedMerkleList<T> AsMerkleized<T>(this IStreamMappedList<T> list, IItemHasher<T> hasher, CHF hashAlgorithm, int merkleTreeStreamIndex) => new(list, hasher, hashAlgorithm, merkleTreeStreamIndex);

	//#endregion

	#region IMemoryPagedBuffer

	public static MerklePagedBuffer AsMerkleizedBuffer(this IMemoryPagedBuffer buffer, CHF hashAlgorithm) => new(buffer, hashAlgorithm);

	public static MerklePagedBuffer AsMerkleizedBuffer(this IMemoryPagedBuffer buffer, IDynamicMerkleTree merkleTree) => new(buffer, merkleTree);

	#endregion

	#region IReadOnlyDictionary

	#region AsProjection

	public static IReadOnlyDictionary<TProjectedKey, TProjectedValue> WithProjection<TKey, TValue, TProjectedKey, TProjectedValue>(
		this IReadOnlyDictionary<TKey, TValue> dictionary,
		Func<TKey, TProjectedKey> keyProjection,
		Func<TProjectedKey, TKey> inverseKeyProjection,
		Func<TValue, TProjectedValue> valueProjection
	) => new ProjectedReadOnlyDictionary<TKey, TValue, TProjectedKey, TProjectedValue>(dictionary, keyProjection, inverseKeyProjection, valueProjection);

	public static IReadOnlyDictionary<TProjectedKey, TValue> WithKeyProjection<TKey, TValue, TProjectedKey>(this IReadOnlyDictionary<TKey, TValue> dictionary, Func<TKey, TProjectedKey> keyProjection, Func<TProjectedKey, TKey> inverseKeyProjection) 
		=> dictionary.WithProjection(keyProjection, inverseKeyProjection, v => v);

	public static IReadOnlyDictionary<TKey, TProjectedValue> WithValueProjection<TKey, TValue, TProjectedValue>(this IReadOnlyDictionary<TKey, TValue> dictionary, Func<TValue, TProjectedValue> valueProjection) 
		=> dictionary.WithProjection(k => k, k => k, valueProjection);

	#endregion

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

	#region AsBijective

	public static BijectiveDictionary<TKey, TValue > AsBijection<TKey, TValue>(this IDictionary<TKey, TValue> dictionary) => dictionary.AsBijection(new Dictionary<TValue, TKey>());

	public static BijectiveDictionary<TKey, TValue > AsBijection<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, IDictionary<TValue, TKey> internalBijectiveDictionary) => new(dictionary, internalBijectiveDictionary);

	#endregion

	#endregion

	#region ILookup

	#region AsProjection

	public static ILookup<TProjectedKey, TProjectedValue> WithProjection<TKey, TValue, TProjectedKey, TProjectedValue>(
		this ILookup<TKey, TValue> dictionary,
		Func<TKey, TProjectedKey> keyProjection,
		Func<TProjectedKey, TKey> inverseKeyProjection,
		Func<TValue, TProjectedValue> valueProjection
	) => new ProjectedLookup<TKey,TValue,TProjectedKey,TProjectedValue>(dictionary, keyProjection, inverseKeyProjection, valueProjection);

	public static ILookup<TProjectedKey, TValue> WithKeyProjection<TKey, TValue, TProjectedKey>(this ILookup<TKey, TValue> dictionary, Func<TKey, TProjectedKey> keyProjection, Func<TProjectedKey, TKey> inverseKeyProjection) 
		=> dictionary.WithProjection(keyProjection, inverseKeyProjection, v => v);

	public static ILookup<TKey, TProjectedValue> WithValueProjection<TKey, TValue, TProjectedValue>(this ILookup<TKey, TValue> dictionary, Func<TValue, TProjectedValue> valueProjection) 
		=> dictionary.WithProjection(k => k, k => k, valueProjection);

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

	#region Transactional Stream

	public static TransactionalStream<TStream> AsTransactional<TStream>(this TStream stream, ITransactionalObject innerTransactionalObject) 
		where TStream : Stream => new (stream, innerTransactionalObject);

	#endregion

	#region Serializer

	private static readonly MethodInfo _asUnwrappedGenericMethod = typeof(DecoratorExtensions).GetGenericMethod(nameof(AsUnwrapped), 1);
	private static readonly MethodInfo _asWrappedGenericMethod1 = typeof(DecoratorExtensions).GetGenericMethods(nameof(AsWrapped), 1).FirstOrDefault(x => x.GetParameters().Length == 1);
	private static readonly MethodInfo _asWrappedGenericMethod3 = typeof(DecoratorExtensions).GetGenericMethods(nameof(AsWrapped), 1).FirstOrDefault(x => x.GetParameters().Length == 3);
	private static readonly MethodInfo _asReferenceSerializerMethod = typeof(DecoratorExtensions).GetGenericMethod(nameof(AsReferenceSerializer), 1);
	private static readonly MethodInfo _asDereferencedSerializer = typeof(DecoratorExtensions).GetGenericMethod(nameof(AsDereferencedSerializer), 1);
	private static readonly MethodInfo _asCastedSerializer = typeof(DecoratorExtensions).GetGenericMethod(nameof(AsCastedSerializer), 2);	
	

	public static IItemSerializer AsWrapped(this IItemSerializer serializer) 
		=> _asWrappedGenericMethod1.MakeGenericMethod(serializer.ItemType).Invoke(null, [serializer]) as IItemSerializer;

	public static IItemSerializer AsWrapped(this IItemSerializer serializer, SerializerFactory factory, ReferenceSerializerMode referenceMode) 
		=> _asWrappedGenericMethod3.MakeGenericMethod(serializer.ItemType).Invoke(null, [serializer, factory, referenceMode]) as IItemSerializer;

	public static IItemSerializer AsUnwrapped(this IItemSerializer serializer) 
		=> _asUnwrappedGenericMethod.MakeGenericMethod(serializer.ItemType).Invoke(null, [serializer]) as IItemSerializer;

	public static IItemSerializer AsCastedSerializer(this IItemSerializer serializer, Type baseType)
		=> _asCastedSerializer.MakeGenericMethod(serializer.ItemType, baseType).Invoke(null, [serializer]) as IItemSerializer;

	public static IItemSerializer AsReferenceSerializer(this IItemSerializer serializer, ReferenceSerializerMode mode = ReferenceSerializerMode.Default)
		=> _asReferenceSerializerMethod.MakeGenericMethod(serializer.ItemType).Invoke(null, [serializer, mode]) as IItemSerializer;

	public static IItemSerializer AsDereferencedSerializer(this IItemSerializer serializer)
		=> _asDereferencedSerializer.MakeGenericMethod(serializer.ItemType).Invoke(null, [serializer]) as IItemSerializer;

	public static IItemSerializer<T> AsWrapped<T>(this IItemSerializer<T> serializer)  
		=> serializer.AsWrapped(SerializerFactory.Default, ReferenceSerializerMode.Default);

	public static IItemSerializer<T> AsWrapped<T>(this IItemSerializer<T> serializer, SerializerFactory factory, ReferenceSerializerMode referenceMode)  {
		var itemType = serializer.ItemType;
		if (!itemType.IsSealed && serializer is not PolymorphicSerializer<T>) {
			serializer = serializer.AsPolymorphicSerializer(factory);
		}  if (!itemType.IsValueType) {
			serializer = serializer.AsReferenceSerializer(referenceMode);
		}
		return serializer;
	}

	public static IItemSerializer<T> AsUnwrapped<T>(this IItemSerializer<T> serializer) 
		=> serializer switch {
			ReferenceSerializer<T> referenceSerializer => referenceSerializer.Internal.AsUnwrapped(),
			PolymorphicSerializer<T> polymorphicSerializer when !typeof(T).IsAbstract => polymorphicSerializer.Factory.GetPureSerializer<T>(),
			_ => serializer
		};

	public static IItemSerializer<TTo> AsCastedSerializer<TFrom, TTo>(this IItemSerializer<TFrom> serializer)
		=> new CastedSerializer<TFrom, TTo>(serializer);

	public static IItemSerializer<T> AsNullableSerializer<T>(this IItemSerializer<T> serializer)
		=> (typeof(T).IsValueType || serializer.SupportsNull) ? serializer : new ReferenceSerializer<T>(serializer, ReferenceSerializerMode.SupportNull);

	public static IItemSerializer<T> AsReferenceSerializer<T>(this IItemSerializer<T> serializer, ReferenceSerializerMode mode = ReferenceSerializerMode.Default)
		=> typeof(T).IsValueType ? serializer : new ReferenceSerializer<T>(serializer, mode);

	public static IItemSerializer<T> AsPolymorphicSerializer<T>(this IItemSerializer<T> serializer, SerializerFactory factory)
		=> new PolymorphicSerializer<T>(factory, serializer);
	public static IItemSerializer<T> AsDereferencedSerializer<T>(this IItemSerializer<T> serializer)
		=> (serializer is ReferenceSerializer<T> referenceSerializer) ? referenceSerializer.Internal : serializer;

	public static IItemSerializer<T> AsNullableConstantSize<T>(this IItemSerializer<T> serializer) {
		if (serializer is ConstantSizeReferenceTypeSerializer<T>)
			return serializer;

		var referenceTypeSerializer = serializer is ReferenceSerializer<T> rts ? rts : new ReferenceSerializer<T>(serializer);
		return new ConstantSizeReferenceTypeSerializer<T>(referenceTypeSerializer);
	}

	public static IItemSerializer<T> WithNullSubstitution<T>(this IItemSerializer<T> serializer, T nullSubstitution, IEqualityComparer<T> comparer = null)
		=> new WithNullSubstitutionSerializer<T>(serializer, nullSubstitution, comparer);

	public static IItemSerializer<TItem> AsConstantSize<TItem>(this IItemSerializer<TItem> serializer, int length) 
		=> new PaddedSerializer<TItem>(length, serializer);

	#endregion
}
