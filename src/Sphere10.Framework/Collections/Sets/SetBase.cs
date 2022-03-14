using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Sphere10.Framework {

	public abstract class SetBase<TItem> : ISet<TItem> {
		protected readonly IEqualityComparer<TItem> Comparer;

		protected SetBase(IEqualityComparer<TItem> comparer) {
			Guard.ArgumentNotNull(comparer, nameof(comparer));
			Comparer = comparer;
		}

		public abstract int Count { get; }

		public abstract bool IsReadOnly { get; }

		public abstract IEnumerator<TItem> GetEnumerator();

		IEnumerator IEnumerable.GetEnumerator() {
			return GetEnumerator();
		}

		void ICollection<TItem>.Add(TItem item) {
			Guard.ArgumentNotNull(item, nameof(item));
			((ISet<TItem>)this).Add(item);
		}

		public abstract bool Add(TItem item);

		public abstract void Clear();

		public abstract bool Contains(TItem item);

		public abstract bool Remove(TItem item);

		public abstract void CopyTo(TItem[] array, int arrayIndex);

		public virtual void ExceptWith(IEnumerable<TItem> other) {
			foreach (var item in other)
				Remove(item);
		}

		public virtual void IntersectWith(IEnumerable<TItem> other) {
			var otherSet = other as ISet<TItem> ?? other.ToHashSet();
			
			var intersection = otherSet.Where(Contains).ToList();
			Clear();
			foreach (var item in intersection)
				Add(item);
		}

		public virtual bool IsProperSubsetOf(IEnumerable<TItem> other) {
			var otherSet = other as ISet<TItem> ?? other.ToHashSet();
			return Count < otherSet.Count && IsSubsetOf(otherSet);
		}

		public virtual bool IsProperSupersetOf(IEnumerable<TItem> other) {
			var otherSet = other as ISet<TItem> ?? other.ToHashSet();
			return Count > otherSet.Count && IsSupersetOf(otherSet);
		}

		public virtual bool IsSubsetOf(IEnumerable<TItem> other) 
			=> (other as ISet<TItem> ?? other.ToHashSet()).ContainsAll(this);

		public virtual bool IsSupersetOf(IEnumerable<TItem> other)
			=> this.ContainsAll(other);

		public virtual bool Overlaps(IEnumerable<TItem> other)
			=> this.ContainsAny(other as ISet<TItem> ?? other.ToHashSet());

		public virtual bool SetEquals(IEnumerable<TItem> other) {
			var otherColl = other as ICollection<TItem> ?? other.ToArray();
			return Count == otherColl.Count && this.ContainsAll(otherColl);
		}

		public virtual void SymmetricExceptWith(IEnumerable<TItem> other) {
			throw new System.NotImplementedException();
		}

		public virtual void UnionWith(IEnumerable<TItem> other) {
			throw new System.NotImplementedException();
		}


	}

}
