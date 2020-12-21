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
using System.Linq;
using System.Text;
using System.Collections;
using System.Threading;

namespace Sphere10.Framework {

	/// <summary>
	/// This class uses a read writer lock to provide data synchronization, but the design of the IList interface itself can lead to
	/// race conditions. Beware of it's use. 
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public class SynchronizedList<T> : ListDecorator<T>, IReadWriteSafeObject {

		private readonly ReadWriteSafeObject _lock;

		public SynchronizedList()
			: this(new List<T>()) {
		}

		public SynchronizedList(IList<T> internalList) {
			if (internalList is T[])
				throw new ArgumentException("provided internalList was an array", nameof(internalList));
			_lock = new ReadWriteSafeObject();
		}

		public override T this[int index] {
			get { using (EnterReadScope()) return base[index]; }
			set { using (EnterWriteScope()) base[index] = value; }
		}

		public override int IndexOf(T item) { using (EnterReadScope()) return base.IndexOf(item); }
		public override void Insert(int index, T item) { using (EnterWriteScope()) base.Insert(index, item); }
		public override void RemoveAt(int index) { using (EnterWriteScope()) base.RemoveAt(index); }
		public override void Add(T item) { using (EnterWriteScope()) base.Add(item); }
		public override void Clear() { using (EnterWriteScope()) base.Clear(); }
		public override bool Contains(T item) { using (EnterReadScope()) return base.Contains(item); }
		public override int Count { get { using (EnterReadScope()) return base.Count; } }
		public override bool IsReadOnly { get { using (EnterReadScope()) return base.IsReadOnly; } }
		public override void CopyTo(T[] array, int arrayIndex) { using (EnterReadScope()) base.CopyTo(array, arrayIndex); }
		public override bool Remove(T item) { using (EnterWriteScope()) return base.Remove(item); }
		public override IEnumerator<T> GetEnumerator() { var readScope = EnterReadScope(); return base.GetEnumerator().OnDispose(readScope.Dispose); }

		public ReaderWriterLockSlim ThreadLock => _lock.ThreadLock;

		public Scope EnterReadScope() {
			return _lock.EnterReadScope();
		}

		public Scope EnterWriteScope() {
			return _lock.EnterWriteScope();
		}
	}

}

