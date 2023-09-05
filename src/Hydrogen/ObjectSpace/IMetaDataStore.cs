using System;

namespace Hydrogen.ObjectSpace.MetaData;

public interface IMetaDataStore<TData> : ILoadable, IDisposable {

	ObjectContainer Container { get; }

	long Count { get; }

	TData Read(long index);

	byte[] ReadBytes(long index);

	void Add(long index, TData data);

	void Update(long index, TData data);

	void Insert(long index, TData data);

	void Remove(long index);

	void Reap(long index);

	void Clear();

}