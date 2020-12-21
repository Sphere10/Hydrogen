using System.Collections;
using System.Collections.Generic;

namespace Sphere10.Framework {

	public abstract class SetDecorator<T> : ISet<T> {

		protected readonly ISet<T> InternalSet;

		protected SetDecorator(ISet<T> internalSet) {
			InternalSet = internalSet;
		}


		public virtual IEnumerator<T> GetEnumerator() {
			return InternalSet.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator() {
			return GetEnumerator();
		}

		public virtual bool Add(T item) {
			return InternalSet.Add(item);
		}

		void ICollection<T>.Add(T item) {
			Add(item);
		}

		public virtual void ExceptWith(IEnumerable<T> other) {
			InternalSet.ExceptWith(other);
		}

		public virtual void IntersectWith(IEnumerable<T> other) {
			InternalSet.IntersectWith(other);
		}

		public virtual bool IsProperSubsetOf(IEnumerable<T> other) {
			return InternalSet.IsProperSubsetOf(other);
		}

		public virtual bool IsProperSupersetOf(IEnumerable<T> other) {
			return InternalSet.IsProperSupersetOf(other);
		}

		public virtual bool IsSubsetOf(IEnumerable<T> other) {
			return InternalSet.IsSubsetOf(other);
		}

		public virtual bool IsSupersetOf(IEnumerable<T> other) {
			return InternalSet.IsSupersetOf(other);
		}

		public virtual bool Overlaps(IEnumerable<T> other) {
			return InternalSet.Overlaps(other);
		}

		public virtual bool SetEquals(IEnumerable<T> other) {
			return InternalSet.SetEquals(other);
		}

		public virtual void SymmetricExceptWith(IEnumerable<T> other) {
			InternalSet.SymmetricExceptWith(other);
		}

		public virtual void UnionWith(IEnumerable<T> other) {
			InternalSet.UnionWith(other);
		}

		public virtual void Clear() {
			InternalSet.Clear();
		}

		public virtual bool Contains(T item) {
			return InternalSet.Contains(item);
		}

		public virtual void CopyTo(T[] array, int arrayIndex) {
			InternalSet.CopyTo(array, arrayIndex);
		}

		public virtual bool Remove(T item) {
			return InternalSet.Remove(item);
		}

		public virtual int Count => InternalSet.Count;

		public virtual bool IsReadOnly => InternalSet.IsReadOnly;

	}

}