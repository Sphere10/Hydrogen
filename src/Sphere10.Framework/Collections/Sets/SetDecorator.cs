using System.Collections;
using System.Collections.Generic;

namespace Sphere10.Framework {

	public abstract class SetDecorator<TItem, TSet> : ISet<TItem> where TSet : ISet<TItem> {

		protected readonly TSet InternalSet;

		protected SetDecorator(TSet internalSet) {
			InternalSet = internalSet;
		}

		public virtual IEnumerator<TItem> GetEnumerator() {
			return InternalSet.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator() {
			return GetEnumerator();
		}

		public virtual bool Add(TItem item) {
			return InternalSet.Add(item);
		}

		void ICollection<TItem>.Add(TItem item) {
			Add(item);
		}

		public virtual void ExceptWith(IEnumerable<TItem> other) {
			InternalSet.ExceptWith(other);
		}

		public virtual void IntersectWith(IEnumerable<TItem> other) {
			InternalSet.IntersectWith(other);
		}

		public virtual bool IsProperSubsetOf(IEnumerable<TItem> other) {
			return InternalSet.IsProperSubsetOf(other);
		}

		public virtual bool IsProperSupersetOf(IEnumerable<TItem> other) {
			return InternalSet.IsProperSupersetOf(other);
		}

		public virtual bool IsSubsetOf(IEnumerable<TItem> other) {
			return InternalSet.IsSubsetOf(other);
		}

		public virtual bool IsSupersetOf(IEnumerable<TItem> other) {
			return InternalSet.IsSupersetOf(other);
		}

		public virtual bool Overlaps(IEnumerable<TItem> other) {
			return InternalSet.Overlaps(other);
		}

		public virtual bool SetEquals(IEnumerable<TItem> other) {
			return InternalSet.SetEquals(other);
		}

		public virtual void SymmetricExceptWith(IEnumerable<TItem> other) {
			InternalSet.SymmetricExceptWith(other);
		}

		public virtual void UnionWith(IEnumerable<TItem> other) {
			InternalSet.UnionWith(other);
		}

		public virtual void Clear() {
			InternalSet.Clear();
		}

		public virtual bool Contains(TItem item) {
			return InternalSet.Contains(item);
		}

		public virtual void CopyTo(TItem[] array, int arrayIndex) {
			InternalSet.CopyTo(array, arrayIndex);
		}

		public virtual bool Remove(TItem item) {
			return InternalSet.Remove(item);
		}

		public virtual int Count => InternalSet.Count;

		public virtual bool IsReadOnly => InternalSet.IsReadOnly;

	}

}