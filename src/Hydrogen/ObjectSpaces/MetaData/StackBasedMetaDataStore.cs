// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Collections;
using System.Collections.Generic;

namespace Hydrogen.ObjectSpaces;

/// <summary>
/// Stores meta-data about an <see cref="ObjectContainer"/>'s items as a Stack within in a reserved stream inside the <see cref="ObjectContainer"/>.
/// This is used within a <see cref="RecyclableIndexIndex" /> to store the recyclable indicies of deleted items and has no memory overhead. 
/// </summary>
/// <remarks>The metadata datum must be constant-sized for all items to ensure fast addressing.</remarks>
/// <typeparam name="TData">The type of the item meta-data being stored</typeparam>
internal class StackBasedMetaDataStore<TData> : MetaDataStoreBase<TData>, IStack<TData> {
	
	private StreamPagedList<TData> _inStreamIndex;

	public StackBasedMetaDataStore(ClusteredStreams streams, long reservedStreamIndex, IItemSerializer<TData> datumSerializer) 
		: base(streams, reservedStreamIndex) {
		Guard.ArgumentNotNull(datumSerializer, nameof(datumSerializer));
		Guard.Argument(datumSerializer.IsConstantSize, nameof(datumSerializer), "Datum serializer must be a constant-length serializer.");
		DatumSerializer = datumSerializer;
	}

	public override long Count => _inStreamIndex.Count;

	public IStack<TData> Stack => this;

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

	public override void Update(long index, TData data) 
		=> throw new NotSupportedException("Update operation is not supported in a stack-based store");

	public override void Insert(long index, TData data) 
		=> throw new NotSupportedException("Insert operation is not supported in a stack-based store");

	public override void Remove(long index) {
		using var _ = Streams.EnterAccessScope();
		var key = Read(index);
		ValidateRemove(key, index);
		_inStreamIndex.RemoveAt(index);
	}

	public override void Reap(long index) 
		=> throw new NotSupportedException("Reap operation is not supported in a stack-based store");

	public override void Clear() {
		using var _ = Streams.EnterAccessScope();
		ValidateClear();
		_inStreamIndex.Clear();
	}

	protected virtual void ValidateRead(long index) {
		var c = Count;
		Guard.Ensure(c > 0 && index == c - 1, "Can only read last item in a stack-based store");
	}
	

	protected virtual void ValidateAdd(TData key, long index) {
		Guard.Ensure(index == Count, "Can only add as last item in a stack-based store");
	}

	protected virtual void ValidateRemove(TData key, long index) {
		var c = Count;
		Guard.Ensure(c > 0 && index == c - 1, "Can only remove last item in a stack-based store");
	}

	protected virtual void ValidateClear() {
	}


	#region IStack<TData>

    int ICollection<TData>.Count => ((IList<TData>)_inStreamIndex).Count;

    bool ICollection<TData>.IsReadOnly => _inStreamIndex.IsReadOnly;

    void ICollection<TData>.Add(TData item) => _inStreamIndex.Add(item);

    void ICollection<TData>.Clear() => _inStreamIndex.Clear();

    bool ICollection<TData>.Contains(TData item) => throw new NotSupportedException("Operation restricted in stack-based store");

    void ICollection<TData>.CopyTo(TData[] array, int arrayIndex) => throw new NotSupportedException("Operation restricted in stack-based store");

    bool ICollection<TData>.Remove(TData item) => throw new NotSupportedException("Operation restricted in stack-based store");

	bool IStack<TData>.TryPeek(out TData value) {
		using var _ = Streams.EnterAccessScope();
		var count = _inStreamIndex.Count;
		if (count > 0) {
			value = Read(count);
			return true;
		}
		value = default;
		return false;
	}

	bool IStack<TData>.TryPop(out TData value) {
		using var _ = Streams.EnterAccessScope();
		var count = _inStreamIndex.Count;
		if (count > 0) {
			value = Read(count - 1);
			Remove(count - 1);
			return true;
		}
		value = default;
		return false;
	}

	void IStack<TData>.Push(TData item) {
		using var _ = Streams.EnterAccessScope();
		Add(Count, item);
	}

	#endregion

	public IEnumerator<TData> GetEnumerator() {
		throw new NotSupportedException("Enumeration is prohibited in a stack-based store");
	}

	IEnumerator IEnumerable.GetEnumerator() {
		return GetEnumerator();
	}
}
