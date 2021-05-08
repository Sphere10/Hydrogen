//-----------------------------------------------------------------------
// <copyright file="SynchronizedList.cs" company="Sphere 10 Software">
//
// Copyright (c) Sphere 10 Software. All rights reserved. (http://www.sphere10.com)
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// <author>Herman Schoenfeld</author>
// <date>2018</date>
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading;

namespace Sphere10.Framework {

	/// <summary>
	/// This class uses a read writer lock to provide data synchronization, but the design of the IList interface itself can lead to
	/// race conditions. Beware of it's use. 
	/// </summary>
	/// <typeparam name="TItem"></typeparam>
	public class SynchronizedList<TItem, TList> : ListDecorator<TItem, TList>, ISynchronizedObject
		where TList : IList<TItem> {

		private readonly SynchronizedObject _lock;

		public SynchronizedList(TList internalList) : base(internalList) {
			if (internalList is TItem[])
				throw new ArgumentException("provided internalList was an array", nameof(internalList));
			_lock = new SynchronizedObject();
		}

		public override TItem this[int index] {
			get { using (EnterReadScope()) return base[index]; }
			set { using (EnterWriteScope()) base[index] = value; }
		}

		public override int IndexOf(TItem item) { using (EnterReadScope()) return base.IndexOf(item); }
		public override void Insert(int index, TItem item) { using (EnterWriteScope()) base.Insert(index, item); }
		public override void RemoveAt(int index) { using (EnterWriteScope()) base.RemoveAt(index); }
		public override void Add(TItem item) { using (EnterWriteScope()) base.Add(item); }
		public override void Clear() { using (EnterWriteScope()) base.Clear(); }
		public override bool Contains(TItem item) { using (EnterReadScope()) return base.Contains(item); }
		public override int Count { get { using (EnterReadScope()) return base.Count; } }
		public override bool IsReadOnly { get { using (EnterReadScope()) return base.IsReadOnly; } }
		public override void CopyTo(TItem[] array, int arrayIndex) { using (EnterReadScope()) base.CopyTo(array, arrayIndex); }
		public override bool Remove(TItem item) { using (EnterWriteScope()) return base.Remove(item); }
		public override IEnumerator<TItem> GetEnumerator() { var readScope = EnterReadScope(); return base.GetEnumerator().OnDispose(readScope.Dispose); }

		public ReaderWriterLockSlim ThreadLock => _lock.ThreadLock;

		public Scope EnterReadScope() {
			return _lock.EnterReadScope();
		}

		public Scope EnterWriteScope() {
			return _lock.EnterWriteScope();
		}
	}

	public class SynchronizedList<TItem> : SynchronizedList<TItem, IList<TItem>> {
		public SynchronizedList()
			: this(new List<TItem>()) {
		}

		public SynchronizedList(IList<TItem> internalList) 
			: base(internalList) {
		}

	}
}

