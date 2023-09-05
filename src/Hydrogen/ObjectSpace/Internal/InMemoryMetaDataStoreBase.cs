using System;

namespace Hydrogen.ObjectSpace.MetaData;

internal abstract class InMemoryMetaDataStoreBase<TData> : MetaDataStoreDecorator<TData> {

	protected InMemoryMetaDataStoreBase(IMetaDataStore<TData> innerMetaDataStore) : base(innerMetaDataStore) {
		InnerStore.Loaded += _ => LoadMetaData();
	}

	public override void Add(long index, TData key) {
		ValidateMemoryAdd(key, index);
		base.Add(index, key);
		AddToMemory(key, index);
	}

	public override void Update(long index, TData key) {
		var oldKey = Read(index);
		ValidateMemoryUpdate(oldKey, key, index);
		base.Update(index, key);
		UpdateMemory(oldKey, key, index);
	}

	public override void Insert(long index, TData key) {
		ValidateMemoryInsert(key, index);
		base.Insert(index, key);
		InsertInMemory(key, index);
	}

	public override void Remove(long index) {
		var key = Read(index);
		ValidateMemoryRemove(key, index);
		base.Remove(index);
		RemoveFromMemory(key, index);
	}

	public override void Reap(long index) {
		var key = Read(index);
		ValidateMemoryReap(key, index);
		base.Reap(index);
		ReapFromMemory(key, index);
	}

	public override void Clear() {
		ValidateMemoryClear();
	}

	protected void LoadMetaData() {
		ClearMemory();
		// read the stream and populate the index
		var streamCount = base.Count;
		var containerCount = Container.Count;
		Guard.Ensure(streamCount == containerCount, "Container and meta-data store stream are out of sync.");
		using var _ = Container.EnterAccessScope();
		var reserved = Container.StreamContainer.Header.ReservedStreams;
		for(var i = 0L; i < streamCount; i++) {
			// reaped objects are not loaded into memory
			if (Container.StreamContainer.FastReadStreamDescriptorTraits(i + reserved).HasFlag(ClusteredStreamTraits.Reaped))
				continue;
			var key = base.Read(i);
			AddToMemory(key, i);
		}
	}

	protected virtual void ValidateMemoryRead(long index) {
	}

	protected virtual void ValidateMemoryAdd(TData key, long index) {
	}

	protected virtual void ValidateMemoryUpdate(TData oldKey, TData newKey, long index) {
	}

	protected virtual void ValidateMemoryInsert(TData key, long index) {
	}

	protected virtual void ValidateMemoryRemove(TData key, long index) {
	}

	protected virtual void ValidateMemoryReap(TData key, long index) {
	}

	protected virtual void ValidateMemoryClear() {
	}

	protected abstract void ClearMemory();

	protected abstract TData ReadFromMemory(long index);

	protected abstract void AddToMemory(TData data, long index);

	protected abstract void UpdateMemory(TData oldData, TData newData, long index);

	protected abstract void InsertInMemory(TData data, long index);

	protected abstract void RemoveFromMemory(TData data, long index);

	protected abstract void ReapFromMemory(TData data, long index);

}
