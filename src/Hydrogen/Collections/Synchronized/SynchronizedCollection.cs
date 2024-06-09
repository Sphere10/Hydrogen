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

/// <summary>
/// This class uses a read writer lock to provide data synchronization.
/// </summary>
public class SynchronizedCollection<TItem, TConcrete> : CollectionDecorator<TItem, TConcrete>, ISynchronizedObject where TConcrete : ICollection<TItem> {
	private readonly SynchronizedObject _lock;

	public SynchronizedCollection(TConcrete internalList)
		: base(internalList) {
		_lock = new SynchronizedObject();
	}

	public override void Add(TItem item) {
		using (EnterWriteScope()) base.Add(item);
	}
	public override void Clear() {
		using (EnterWriteScope()) base.Clear();
	}
	public override bool Contains(TItem item) {
		using (EnterReadScope()) return base.Contains(item);
	}

	public override int Count {
		get {
			using (EnterReadScope()) return base.Count;
		}
	}

	public override bool IsReadOnly {
		get {
			using (EnterReadScope()) return base.IsReadOnly;
		}
	}

	public override void CopyTo(TItem[] array, int arrayIndex) {
		using (EnterReadScope()) base.CopyTo(array, arrayIndex);
	}
	public override bool Remove(TItem item) {
		using (EnterWriteScope()) return base.Remove(item);
	}
	public override IEnumerator<TItem> GetEnumerator() {
		var readScope = EnterReadScope();
		return base.GetEnumerator().OnDispose(readScope.Dispose);
	}

	public ISynchronizedObject ParentSyncObject {
		get => _lock.ParentSyncObject;
		set => _lock.ParentSyncObject = value;
	}

	public ReaderWriterLockSlim ThreadLock => _lock.ThreadLock;

	public IDisposable EnterReadScope() {
		return _lock.EnterReadScope();
	}

	public IDisposable EnterWriteScope() {
		return _lock.EnterWriteScope();
	}
}


public class SynchronizedCollection<TItem> : SynchronizedCollection<TItem, ICollection<TItem>> {

	public SynchronizedCollection()
		: this(new List<TItem>()) {
	}

	public SynchronizedCollection(ICollection<TItem> internalList)
		: base(internalList) {
	}
}
