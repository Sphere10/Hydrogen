// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

namespace Hydrogen.ObjectSpaces;

/// <summary>
/// Stores meta-data about an <see cref="ObjectContainer"/>'s items as a list within in a reserved stream inside the <see cref="ObjectContainer"/>.
/// The index of the metadata datum correlates to that of the item in the container (i.e. metadata at index N belongs to item N)
/// Can be used to store anything about the items  (i.e. checksums, keys, etc).
/// </summary>
/// <remarks>The metadata datum must be constant-sized for all items to ensure fast addressing.</remarks>
/// <typeparam name="TData">The type of the item meta-data being stored</typeparam>
internal class ListBasedMetaDataStore<TData> : MetaDataStoreBase<TData> {
	
	private StreamPagedList<TData> _inStreamIndex;
	
	public ListBasedMetaDataStore(ClusteredStreams streams, long reservedStreamIndex, IItemSerializer<TData> datumSerializer) 
		: base(streams, reservedStreamIndex) {
		Guard.ArgumentNotNull(datumSerializer, nameof(datumSerializer));
		Guard.Argument(datumSerializer.IsConstantSize, nameof(datumSerializer), "Datum serializer must be a constant-length serializer.");
		DatumSerializer = datumSerializer;
	}

	public override long Count => _inStreamIndex.Count;

	public IExtendedList<TData> List => _inStreamIndex;

	protected IItemSerializer<TData> DatumSerializer { get; }

	protected override void AttachInternal () {
		_inStreamIndex = new StreamPagedList<TData>(
			DatumSerializer, 
			AttachmentStream, 
			Streams.Endianness,
			false, 
			true
		);
	}

	protected override void DetachInternal() {
		_inStreamIndex = null;		
	}

	public override TData Read(long index) {
		using var _ = Streams.EnterAccessScope();
		ValidateRead(index);
		return _inStreamIndex.Read(index);
	}

	public override byte[] ReadBytes(long index) {
		using var _ = Streams.EnterAccessScope();
		ValidateRead(index);
		_inStreamIndex.ReadItemBytes(index, 0, null, out var bytes);
		return bytes;
	}

	public override void Add(long index, TData key) {
		using var _ = Streams.EnterAccessScope();
		ValidateAdd(key, index);
		Guard.Ensure(index == _inStreamIndex.Count, "Index must be equal to the current count.");
		_inStreamIndex.Add(key);
	}

	public override void Update(long index, TData key) {
		using var _ = Streams.EnterAccessScope();
		var oldKey = Read(index);
		ValidateUpdate(oldKey, key, index);
		_inStreamIndex[index] = key;
	}

	public override void Insert(long index, TData key) {
		using var _ = Streams.EnterAccessScope();
		ValidateInsert(key, index);
		_inStreamIndex.ShuffleRightInsert(index, key);
	}

	public override void Remove(long index) {
		using var _ = Streams.EnterAccessScope();
		var key = Read(index);
		ValidateRemove(key, index);
		_inStreamIndex.ShuffleLeftRemoveAt(index);
	}

	public override void Reap(long index) {
		using var _ = Streams.EnterAccessScope();
		var key = Read(index);
		ValidateReap(key, index);
		// A reaped object's value is ignored. Instead the index is marked as reaped.
		//_inStreamIndex.Update(index, default);
	}

	public override void Clear() {
		using var _ = Streams.EnterAccessScope();
		ValidateClear();
		_inStreamIndex.Clear();
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
