// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT NON-AI software license, see the accompanying file
// LICENSE or visit https://sphere10.com/legal/NON-AI-MIT.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Collections;
using System.Collections.Generic;

namespace Hydrogen;

public class PagedListStorageAttachment<TData> : PagedListStorageAttachmentBase<TData>, IExtendedList<TData> {

	public PagedListStorageAttachment(ClusteredStreams streams, string attachmentID, IItemSerializer<TData> datumSerializer)
		: base(streams, attachmentID, datumSerializer) {
	}

	#region IsReadOnly

	public bool IsReadOnly {
		get {
			CheckAttached();
			using var _ = Streams.EnterAccessScope();
			return PagedList.IsReadOnly;
		}
	}

	#endregion

	#region Count

	public long Count {
		get {
			CheckAttached();
			using var _ = Streams.EnterAccessScope();
			return PagedList.Count;
		}
	}

	int ICollection<TData>.Count => checked((int)Count);

	int IReadOnlyCollection<TData>.Count => checked((int)Count);

	#endregion

	#region Read

	public IEnumerable<TData> ReadRange(long index, long count) {
		CheckAttached();
		using var _ = Streams.EnterAccessScope();
		return PagedList.ReadRange(index, count);
	}

	public TData Read(long index) {
		CheckAttached();
		using var _ = Streams.EnterAccessScope();
		return PagedList.Read(index);
	}


	public byte[] ReadBytes(long index) {
		CheckAttached();
		using var _ = Streams.EnterAccessScope();
		PagedList.ReadItemBytes(index, 0, null, out var results);
		return results;
	}

	#endregion

	#region Contains

	public IEnumerable<bool> ContainsRange(IEnumerable<TData> items) {
		CheckAttached();
		using var _ = Streams.EnterAccessScope();
		return PagedList.ContainsRange(items);
	}

	public bool Contains(TData item) {
		CheckAttached();
		using var _ = Streams.EnterAccessScope();
		return PagedList.Contains(item);
	}

	bool ICollection<TData>.Contains(TData item) => Contains(item);

	bool IReadOnlyExtendedCollection<TData>.Contains(TData item) => Contains(item);


	#endregion
	
	#region IndexOf

	public IEnumerable<long> IndexOfRange(IEnumerable<TData> items) {
		CheckAttached();
		using var _ = Streams.EnterAccessScope();
		return PagedList.IndexOfRange(items);
	}

	public long IndexOfL(TData item) {
		CheckAttached();
		using var _ = Streams.EnterAccessScope();
		return PagedList.IndexOfL(item);
	}

	public int IndexOf(TData item) {
		CheckAttached();
		using var _ = Streams.EnterAccessScope();
		return PagedList.IndexOf(item);
	}

	int IList<TData>.IndexOf(TData item) => IndexOf(item);

	#endregion

	#region Add

	public void AddRange(IEnumerable<TData> items) {
		CheckAttached();
		using var _ = Streams.EnterAccessScope();
		PagedList.AddRange(items);
	}

	public void Add(TData item) {
		CheckAttached();
		using var _ = Streams.EnterAccessScope();
		PagedList.Add(item);
	}

	void IWriteOnlyExtendedCollection<TData>.Add(TData item) {
		throw new NotImplementedException();
	}

	void ICollection<TData>.Add(TData item) => Add(item);


	#endregion

	#region Insert

	public void InsertRange(long index, IEnumerable<TData> items) {
		CheckAttached();
		using var _ = Streams.EnterAccessScope();
		PagedList.InsertRange(index, items);
	}

	public void Insert(long index, TData item) {
		CheckAttached();
		using var _ = Streams.EnterAccessScope();
		PagedList.Insert(index, item);
	}

	void IWriteOnlyExtendedList<TData>.Insert(long index, TData item) => Insert(index, item);

	void IList<TData>.Insert(int index, TData item) => Insert(index, item);

	#endregion

	#region Update
	
	public void UpdateRange(long index, IEnumerable<TData> items) {
		CheckAttached();
		using var _ = Streams.EnterAccessScope();
		PagedList.UpdateRange(index, items);
	}
	
	public void Update(long index, TData item) {
		CheckAttached();
		using var _ = Streams.EnterAccessScope();
		PagedList.Update(index, item);
	}

	#endregion

	#region Remove

	public void RemoveRange(long index, long count) {
		CheckAttached();
		using var _ = Streams.EnterAccessScope();
		PagedList.RemoveRange(index, count);		
	}

	public IEnumerable<bool> RemoveRange(IEnumerable<TData> items) {
		CheckAttached();
		using var _ = Streams.EnterAccessScope();
		return PagedList.RemoveRange(items);
	}

	public bool Remove(TData item) {
		CheckAttached();
		using var _ = Streams.EnterAccessScope();
		return PagedList.Remove(item);
	}

	bool IWriteOnlyExtendedCollection<TData>.Remove(TData item) => Remove(item);	

	bool ICollection<TData>.Remove(TData item) => Remove(item);

	public void RemoveAt(long index)  {
		CheckAttached();
		using var _ = Streams.EnterAccessScope();
		PagedList.RemoveAt(index);
	}

	public void RemoveAt(int index) => RemoveAt((long)index);

	void IList<TData>.RemoveAt(int index) => RemoveAt((long)index);

	#endregion

	#region Clear

	public void Clear() {
		CheckAttached();
		using var _ = Streams.EnterAccessScope();
		PagedList.Clear();
	}

	void IWriteOnlyExtendedCollection<TData>.Clear() => Clear();

	void ICollection<TData>.Clear() => Clear();

	#endregion

	#region CopyTo

	public void CopyTo(TData[] array, int arrayIndex) {
		CheckAttached();
		using var _ = Streams.EnterAccessScope();
		PagedList.CopyTo(array, arrayIndex);
	}

	void IReadOnlyExtendedCollection<TData>.CopyTo(TData[] array, int arrayIndex) => CopyTo(array, arrayIndex);

	void ICollection<TData>.CopyTo(TData[] array, int arrayIndex) => CopyTo(array, arrayIndex);

	#endregion

	#region GetEnumerator

	public IEnumerator<TData> GetEnumerator() {
		CheckAttached();
		var scope = Streams.EnterAccessScope();
		try {
			return 
				PagedList
					.GetEnumerator()
					.OnDispose(scope.Dispose);
		} catch {
			scope.Dispose();
			throw;
		}		
	}

	IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

	#endregion

	#region Operator: []

	public TData this[long index] {
		get => PagedList.Read(index);
		set => PagedList.Update(index, value);
	}

	public TData this[int index] {
		get => this[(long)index];
		set => this[(long)index] = value;
	}

	#endregion

}
