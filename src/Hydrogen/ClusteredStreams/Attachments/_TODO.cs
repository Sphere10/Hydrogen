// These are left here for future in case more decomposition required

//using Hydrogen;
//using System.Linq;

//public interface IMetaDataStorage<TKey> {

//	long Count { get; }

//	TKey Read(long index);

//	byte[] ReadBytes(long index);

//	void Add(long index, TKey key);

//	void Update(long index, TKey key);

//	void Insert(long index, TKey key);

//	void Remove(long index);

//	void Reap(long index);

//	void Clear();
//}

//public interface IUniqueKeyStorage<TKey> : IMetaDataStorage<TKey>,  IReadOnlyDictionaryList<TKey, long> {
//}

//public interface IIndexStorage<TKey> : IMetaDataStorage<TKey>,  ILookup<TKey, long> {
//}