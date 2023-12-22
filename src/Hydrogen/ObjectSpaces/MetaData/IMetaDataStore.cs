using System;

namespace Hydrogen.ObjectSpaces;

/// <summary>
/// Stores meta-data about items in an <see cref="ObjectContainer"/>.
/// </summary>
/// <typeparam name="TData">The type of the meta-data datum.</typeparam>
internal interface IMetaDataStore<TData> : IObjectContainerAttachment {

	IItemSerializer<TData> DatumSerializer { get; }

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
