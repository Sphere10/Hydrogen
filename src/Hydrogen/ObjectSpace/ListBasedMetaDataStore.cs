// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.


namespace Hydrogen;

/// <summary>
/// Stores meta-data about an <see cref="ObjectContainer"/>'s items as a list within in a reserved stream.
/// The index of the metadata datum correlates to that of the item in the container.
/// Can be used to store anything about the items  (i.e. checksums, keys, etc).
/// </summary>
/// <typeparam name="TData">The type of the item meta-data being stored</typeparam>
internal class ListBasedMetaDataStore<TData> : MetaDataProviderBase, IMetaDataStore<TData> {
	
	private StreamPagedList<TData> _inStreamIndex;
	

	public ListBasedMetaDataStore(ObjectContainer objectContainer, long reservedStreamIndex, long offset, IItemSerializer<TData> datumSerializer) 
		: base(objectContainer, reservedStreamIndex, offset) {
		Guard.ArgumentNotNull(objectContainer, nameof(objectContainer));
		Guard.ArgumentNotNull(datumSerializer, nameof(datumSerializer));
		Guard.Argument(datumSerializer.IsStaticSize, nameof(datumSerializer), "Datum serializer must be a constant-length serializer.");
		DatumSerializer = datumSerializer;
	}

	public new ObjectContainer Container => base.Container;
	
	public IItemSerializer<TData> DatumSerializer { get; }

	public long Count => _inStreamIndex.Count;

	public TData Read(long index) {
		using var _ = Container.EnterAccessScope();
		ValidateRead(index);
		return _inStreamIndex.Read(index);
	}

	public byte[] ReadBytes(long index) {
		using var _ = Container.EnterAccessScope();
		ValidateRead(index);
		_inStreamIndex.ReadItemBytes(index, 0, null, out var bytes);
		return bytes;
	}

	public void Add(long index, TData key) {
		using var _ = Container.EnterAccessScope();
		ValidateAdd(key, index);
		Guard.Ensure(index == _inStreamIndex.Count, "Index must be equal to the current count.");
		_inStreamIndex.Add(key);
	}

	public void Update(long index, TData key) {
		using var _ = Container.EnterAccessScope();
		var oldKey = Read(index);
		ValidateUpdate(oldKey, key, index);
		_inStreamIndex[index] = key;
	}

	public void Insert(long index, TData key) {
		using var _ = Container.EnterAccessScope();
		ValidateInsert(key, index);
		_inStreamIndex.ShuffleRightInsert(index, key);
	}

	public void Remove(long index) {
		using var _ = Container.EnterAccessScope();
		var key = Read(index);
		ValidateRemove(key, index);
		_inStreamIndex.ShuffleLeftRemoveAt(index);
	}

	public void Reap(long index) {
		using var _ = Container.EnterAccessScope();
		var key = Read(index);
		ValidateReap(key, index);
		// A reaped object's value is ignored. Instead the index is marked as reaped.
		//_inStreamIndex.Update(index, default);
	}

	public void Clear() {
		using var _ = Container.EnterAccessScope();
		ValidateClear();
		_inStreamIndex.Clear();
	}

	protected override void OnLoaded() {
		_inStreamIndex = new StreamPagedList<TData>(DatumSerializer, Stream, Container.StreamContainer.Endianness, false, true);
	}

	protected virtual void ValidateRead(long index) {
	}

	protected virtual void ValidateAdd(TData key, long index) {
	}

	protected virtual void ValidateUpdate(TData oldKey, TData newKey, long index) {
	}

	protected virtual void ValidateInsert(TData key, long index) {
	}

	protected virtual void ValidateRemove(TData key, long index) {
	}

	protected virtual void ValidateReap(TData key, long index) {
	}

	protected virtual void ValidateClear() {
	}

}
