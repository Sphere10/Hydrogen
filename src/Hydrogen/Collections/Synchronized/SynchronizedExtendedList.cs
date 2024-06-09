// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT NON-AI software license, see the accompanying file
// LICENSE or visit https://sphere10.com/legal/NON-AI-MIT.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Collections.Generic;
using System.Threading;

namespace Hydrogen;

public class SynchronizedExtendedList<TItem, TInternalList> : ExtendedListDecorator<TItem, TInternalList>, ISynchronizedExtendedList<TItem>
	where TInternalList : IExtendedList<TItem> {

	private readonly SynchronizedObject _lock;

	public SynchronizedExtendedList(TInternalList internalList)
		: base(internalList) {
		_lock = new SynchronizedObject();
	}

	public ISynchronizedObject ParentSyncObject {
		get => _lock.ParentSyncObject;
		set => _lock.ParentSyncObject = value;
	}

	public ReaderWriterLockSlim ThreadLock => _lock.ThreadLock;

	public override long Count {
		get {
			using (EnterReadScope())
				return InternalCollection.Count;
		}
	}

	public override bool IsReadOnly {
		get {
			using (EnterReadScope())
				return InternalCollection.IsReadOnly;
		}
	}

	public IDisposable EnterReadScope() {
		return _lock.EnterReadScope();
	}

	public IDisposable EnterWriteScope() {
		return _lock.EnterWriteScope();
	}

	public override long IndexOfL(TItem item) {
		using (EnterReadScope())
			return base.IndexOfL(item);
	}

	public override IEnumerable<long> IndexOfRange(IEnumerable<TItem> items) {
		using (EnterReadScope())
			return base.IndexOfRange(items);
	}


	public override bool Contains(TItem item) {
		using (EnterReadScope())
			return base.Contains(item);
	}

	public override IEnumerable<bool> ContainsRange(IEnumerable<TItem> items) {
		using (EnterReadScope())
			return base.ContainsRange(items);
	}

	public override TItem Read(long index) {
		using (EnterReadScope())
			return base.Read(index);
	}

	public override IEnumerable<TItem> ReadRange(long index, long count) {
		using (EnterReadScope())
			return base.ReadRange(index, count);
	}

	public override void Add(TItem item) {
		using (EnterWriteScope())
			base.Add(item);
	}

	public override void AddRange(IEnumerable<TItem> items) {
		using (EnterWriteScope())
			base.AddRange(items);
	}

	public override void Update(long index, TItem item) {
		using (EnterWriteScope())
			base.Update(index, item);
	}

	public override void UpdateRange(long index, IEnumerable<TItem> items) {
		using (EnterWriteScope())
			base.UpdateRange(index, items);
	}

	public override void Insert(long index, TItem item) {
		using (EnterWriteScope())
			base.Insert(index, item);
	}

	public override void InsertRange(long index, IEnumerable<TItem> items) {
		using (EnterWriteScope())
			base.InsertRange(index, items);
	}

	public override bool Remove(TItem item) {
		using (EnterWriteScope())
			return base.Remove(item);
	}

	public override void RemoveAt(long index) {
		using (EnterWriteScope())
			base.RemoveAt(index);
	}

	public override IEnumerable<bool> RemoveRange(IEnumerable<TItem> items) {
		using (EnterWriteScope())
			return base.RemoveRange(items);
	}

	public override void RemoveRange(long index, long count) {
		using (EnterWriteScope())
			base.RemoveRange(index, count);
	}

	public override void Clear() {
		using (EnterWriteScope())
			base.Clear();
	}

	public override void CopyTo(TItem[] array, int arrayIndex) {
		using (EnterReadScope())
			base.CopyTo(array, arrayIndex);
	}

	public override IEnumerator<TItem> GetEnumerator() {
		var readScope = EnterReadScope();
		return base.GetEnumerator().OnDispose(readScope.Dispose);
	}
}


public class SynchronizedExtendedList<TItem> : SynchronizedExtendedList<TItem, IExtendedList<TItem>>, ISynchronizedExtendedList<TItem> {

	public SynchronizedExtendedList()
		: this(new ExtendedList<TItem>()) {
	}

	public SynchronizedExtendedList(IExtendedList<TItem> internalList)
		: base(internalList) {
	}
}
