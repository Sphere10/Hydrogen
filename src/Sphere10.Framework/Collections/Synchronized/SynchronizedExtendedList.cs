//-----------------------------------------------------------------------
// <copyright file="SynchronizedListEx.cs" company="Sphere 10 Software">
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

	public class SynchronizedExtendedList<T> : ExtendedListDecorator<T>, IReadWriteSafeObject {

		private readonly ReadWriteSafeObject _lock;

		public SynchronizedExtendedList() 
			: this(new ExtendedList<T>()) {
		}

		public SynchronizedExtendedList(IExtendedList<T> internalList) 
			: base(internalList){
			_lock = new ReadWriteSafeObject();
		}

		public ReaderWriterLockSlim ThreadLock => _lock.ThreadLock;

		public override int Count {
			get {
				using (EnterReadScope())
					return InternalExtendedList.Count;
			}
		}

		public override bool IsReadOnly {
			get {
				using (EnterReadScope())
					return InternalExtendedList.IsReadOnly;
			}
		}

		public Scope EnterReadScope() {
			return _lock.EnterReadScope();
		}

		public Scope EnterWriteScope() {
			return _lock.EnterWriteScope();
		}

		public override int IndexOf(T item) {
			using (EnterReadScope())
				return base.IndexOf(item);
		}

		public override IEnumerable<int> IndexOfRange(IEnumerable<T> items) {
			using (EnterReadScope())
				return base.IndexOfRange(items);
		}


		public override bool Contains(T item) {
			using (EnterReadScope())
				return base.Contains(item);
		}

		public override IEnumerable<bool> ContainsRange(IEnumerable<T> items) {
			using (EnterReadScope())
				return base.ContainsRange(items);
		}

		public override T Read(int index) {
			using (EnterReadScope())
				return base.Read(index);
		}

		public override IEnumerable<T> ReadRange(int index, int count) {
			using (EnterReadScope())
				return base.ReadRange(index, count);
		}

		public override void Add(T item) {
			using (EnterWriteScope())
				base.Add(item);
		}

		public override void AddRange(IEnumerable<T> items) {
			using (EnterWriteScope())
				base.AddRange(items);
		}

		public override void Update(int index, T item) {
			using (EnterWriteScope())
				base.Update(index, item);
		}

		public override void UpdateRange(int index, IEnumerable<T> items) {
			using (EnterWriteScope()) 
				base.UpdateRange(index, items);
		}

		public override void Insert(int index, T item) {
			using (EnterWriteScope())
				base.Insert(index, item);
		}

		public override void InsertRange(int index, IEnumerable<T> items) {
			using (EnterWriteScope()) 
				base.InsertRange(index, items);
		}

		public override bool Remove(T item) {
			using (EnterWriteScope())
				return base.Remove(item);
		}

		public override void RemoveAt(int index) {
			using (EnterWriteScope())
				base.RemoveAt(index);
		}

		public override IEnumerable<bool> RemoveRange(IEnumerable<T> items) {
			using (EnterWriteScope())
				return base.RemoveRange(items);
		}

		public override void RemoveRange(int index, int count) {
			using (EnterWriteScope()) 
				base.RemoveRange(index, count);
		}

		public override void Clear() {
			using (EnterWriteScope())
				base.Clear();
		}

		public override void CopyTo(T[] array, int arrayIndex) {
			using (EnterReadScope())
				base.CopyTo(array, arrayIndex);
		}

		public override IEnumerator<T> GetEnumerator() {
			var readScope = EnterReadScope(); 
			return base.GetEnumerator().OnDispose(readScope.Dispose);
		}
	}
}

