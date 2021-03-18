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
using System.Threading;

namespace Sphere10.Framework {

	public class SynchronizedSet<T> : SetDecorator<T>, IThreadSafeObject {

		private readonly ThreadSafeObject _lock;

		public SynchronizedSet()
			: this(new HashSet<T>()) {
		}

		public SynchronizedSet(IEqualityComparer<T> equalityComparer)
			: this(new HashSet<T>(equalityComparer)) {
		}

		public SynchronizedSet(ISet<T> internalSet)
			: base(internalSet) {
			_lock = new ThreadSafeObject();
		}

		public ReaderWriterLockSlim ThreadLock => _lock.ThreadLock;

		public Scope EnterReadScope() {
			return _lock.EnterReadScope();
		}

		public Scope EnterWriteScope() {
			return _lock.EnterWriteScope();
		}

		public override IEnumerator<T> GetEnumerator() {
			var readScope = EnterReadScope();
			return
				InternalSet
				.GetEnumerator()
				.OnDispose(readScope.Dispose);
		}

		public override bool Add(T item) {
			return InternalSet.Add(item);
		}

		public override void ExceptWith(IEnumerable<T> other) {
			InternalSet.ExceptWith(other);
		}

		public override void IntersectWith(IEnumerable<T> other) {
			InternalSet.IntersectWith(other);
		}

		public override bool IsProperSubsetOf(IEnumerable<T> other) {
			return InternalSet.IsProperSubsetOf(other);
		}

		public override bool IsProperSupersetOf(IEnumerable<T> other) {
			return InternalSet.IsProperSupersetOf(other);
		}

		public override bool IsSubsetOf(IEnumerable<T> other) {
			return InternalSet.IsSubsetOf(other);
		}

		public override bool IsSupersetOf(IEnumerable<T> other) {
			return InternalSet.IsSupersetOf(other);
		}

		public override bool Overlaps(IEnumerable<T> other) {
			return InternalSet.Overlaps(other);
		}

		public override bool SetEquals(IEnumerable<T> other) {
			return InternalSet.SetEquals(other);
		}

		public override void SymmetricExceptWith(IEnumerable<T> other) {
			InternalSet.SymmetricExceptWith(other);
		}

		public override void UnionWith(IEnumerable<T> other) {
			InternalSet.UnionWith(other);
		}

		public override void Clear() {
			InternalSet.Clear();
		}

		public override bool Contains(T item) {
			return InternalSet.Contains(item);
		}

		public override void CopyTo(T[] array, int arrayIndex) {
			InternalSet.CopyTo(array, arrayIndex);
		}

		public override bool Remove(T item) {
			return InternalSet.Remove(item);
		}

		public override int Count => InternalSet.Count;

		public override bool IsReadOnly => InternalSet.IsReadOnly;
	}

}

