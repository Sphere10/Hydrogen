// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

namespace Hydrogen.ObjectSpaces;

/// <summary>
/// Base implementation for memory-caching an <see cref="IMetaDataStore{TData}"/>.
/// </summary>
/// <remarks>This derives from <see cref="MetaDataStoreDecorator{TData,TInner}"/> as it always wraps an internal
/// <see cref="IMetaDataStore{TData}"/> which persists the data in the <see cref="ObjectContainer"/>.  </remarks>
/// <typeparam name="TData">The type of the meta-data being stored in memory</typeparam>
internal abstract class MemoryCachedMetaDataStoreBase<TData> : MetaDataStoreDecorator<TData> {

	protected MemoryCachedMetaDataStoreBase(IMetaDataStore<TData> innerMetaDataStore) 
		: base(innerMetaDataStore) {
	}

	public override void Attach() {
		base.Attach();
		ClearMemory();
		LoadMetaData();
	}

	public override void Detach() {
		base.Detach();
		ClearMemory();
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

	protected virtual void LoadMetaData() {
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
