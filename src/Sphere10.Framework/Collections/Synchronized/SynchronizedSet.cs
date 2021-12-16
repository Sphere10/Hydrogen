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

using System.Collections.Generic;
using System.Threading;

namespace Sphere10.Framework {

	public class SynchronizedSet<TItem, TSet> : SetDecorator<TItem, TSet>, ISynchronizedObject where TSet : ISet<TItem> {

		private readonly SynchronizedObject _lock;

		public SynchronizedSet(TSet internalSet)
			: base(internalSet) {
			_lock = new SynchronizedObject();
		}

		public ISynchronizedObject<Scope, Scope> ParentSyncObject {
			get => _lock.ParentSyncObject;
			set => _lock.ParentSyncObject = value;
		}

		public ReaderWriterLockSlim ThreadLock => _lock.ThreadLock;

		public Scope EnterReadScope() {
			return _lock.EnterReadScope();
		}

		public Scope EnterWriteScope() {
			return _lock.EnterWriteScope();
		}

		public override IEnumerator<TItem> GetEnumerator() {
			var readScope = EnterReadScope();
			return
				InternalSet
				.GetEnumerator()
				.OnDispose(readScope.Dispose);
		}

		public override bool Add(TItem item) {
			return InternalSet.Add(item);
		}

		public override void ExceptWith(IEnumerable<TItem> other) {
			InternalSet.ExceptWith(other);
		}

		public override void IntersectWith(IEnumerable<TItem> other) {
			InternalSet.IntersectWith(other);
		}

		public override bool IsProperSubsetOf(IEnumerable<TItem> other) {
			return InternalSet.IsProperSubsetOf(other);
		}

		public override bool IsProperSupersetOf(IEnumerable<TItem> other) {
			return InternalSet.IsProperSupersetOf(other);
		}

		public override bool IsSubsetOf(IEnumerable<TItem> other) {
			return InternalSet.IsSubsetOf(other);
		}

		public override bool IsSupersetOf(IEnumerable<TItem> other) {
			return InternalSet.IsSupersetOf(other);
		}

		public override bool Overlaps(IEnumerable<TItem> other) {
			return InternalSet.Overlaps(other);
		}

		public override bool SetEquals(IEnumerable<TItem> other) {
			return InternalSet.SetEquals(other);
		}

		public override void SymmetricExceptWith(IEnumerable<TItem> other) {
			InternalSet.SymmetricExceptWith(other);
		}

		public override void UnionWith(IEnumerable<TItem> other) {
			InternalSet.UnionWith(other);
		}

		public override void Clear() {
			InternalSet.Clear();
		}

		public override bool Contains(TItem item) {
			return InternalSet.Contains(item);
		}

		public override void CopyTo(TItem[] array, int arrayIndex) {
			InternalSet.CopyTo(array, arrayIndex);
		}

		public override bool Remove(TItem item) {
			return InternalSet.Remove(item);
		}

		public override int Count => InternalSet.Count;

		public override bool IsReadOnly => InternalSet.IsReadOnly;
	}

	public sealed class SynchronizedSet<TItem> : SynchronizedSet<TItem, ISet<TItem>> {

		public SynchronizedSet()
			: this(EqualityComparer<TItem>.Default) {
		}

		public SynchronizedSet(IEqualityComparer<TItem> equalityComparer)
			: this(new HashSet<TItem>(equalityComparer)) {
		}

		public SynchronizedSet(ISet<TItem> internalSet)
			: base(internalSet) {
		}
	}
}

