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

using System.Collections.Generic;
using System.Threading;

namespace Sphere10.Framework {

	public class SynchronizedExtendedList<TItem, TInternalList> : ExtendedListDecorator<TItem, TInternalList>, ISynchronizedExtendedList<TItem>
		where TInternalList : IExtendedList<TItem> {

		private readonly SynchronizedObject _lock;

		public SynchronizedExtendedList(TInternalList internalList) 
			: base(internalList){
			_lock = new SynchronizedObject();
		}

		public ISynchronizedObject<Scope, Scope> ParentSyncObject {
			get => _lock.ParentSyncObject;
			set => _lock.ParentSyncObject = value;
		}

		public ReaderWriterLockSlim ThreadLock => _lock.ThreadLock;

		public override int Count {
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

		public Scope EnterReadScope() {
			return _lock.EnterReadScope();
		}

		public Scope EnterWriteScope() {
			return _lock.EnterWriteScope();
		}

		public override int IndexOf(TItem item) {
			using (EnterReadScope())
				return base.IndexOf(item);
		}

		public override IEnumerable<int> IndexOfRange(IEnumerable<TItem> items) {
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

		public override TItem Read(int index) {
			using (EnterReadScope())
				return base.Read(index);
		}

		public override IEnumerable<TItem> ReadRange(int index, int count) {
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

		public override void Update(int index, TItem item) {
			using (EnterWriteScope())
				base.Update(index, item);
		}

		public override void UpdateRange(int index, IEnumerable<TItem> items) {
			using (EnterWriteScope()) 
				base.UpdateRange(index, items);
		}

		public override void Insert(int index, TItem item) {
			using (EnterWriteScope())
				base.Insert(index, item);
		}

		public override void InsertRange(int index, IEnumerable<TItem> items) {
			using (EnterWriteScope()) 
				base.InsertRange(index, items);
		}

		public override bool Remove(TItem item) {
			using (EnterWriteScope())
				return base.Remove(item);
		}

		public override void RemoveAt(int index) {
			using (EnterWriteScope())
				base.RemoveAt(index);
		}

		public override IEnumerable<bool> RemoveRange(IEnumerable<TItem> items) {
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
}

