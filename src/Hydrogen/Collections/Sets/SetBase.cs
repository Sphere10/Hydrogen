// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT NON-AI software license, see the accompanying file
// LICENSE or visit https://sphere10.com/legal/NON-AI-MIT.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Hydrogen;

public abstract class SetBase<TItem> : ISet<TItem> {
	protected readonly IEqualityComparer<TItem> Comparer;

	protected SetBase() : this(EqualityComparer<TItem>.Default) {
	}

	protected SetBase(IEqualityComparer<TItem> comparer) {
		Guard.ArgumentNotNull(comparer, nameof(comparer));
		Comparer = comparer;
	}

	public abstract int Count { get; }

	public abstract bool IsReadOnly { get; }

	void ICollection<TItem>.Add(TItem item) {
		Guard.ArgumentNotNull(item, nameof(item));
		((ISet<TItem>)this).Add(item);
	}

	public abstract bool Add(TItem item);

	public abstract void Clear();

	public abstract bool Contains(TItem item);

	public abstract bool Remove(TItem item);

	public virtual void ExceptWith(IEnumerable<TItem> other) {
		foreach (var item in other)
			Remove(item);
	}

	public virtual void SymmetricExceptWith(IEnumerable<TItem> other) {
		var addables = new List<TItem>();
		var removables = new List<TItem>();

		foreach (var item in other)
			if (Contains(item))
				removables.Add(item); // item present in both, so remove it
			else
				addables.Add(item); // item not in set, add it

		foreach (var item in removables)
			Remove(item);

		foreach (var item in addables)
			Add(item);
	}

	public virtual void UnionWith(IEnumerable<TItem> other) {
		foreach (var item in other)
			if (!Contains(item))
				Add(item);
	}

	public virtual void IntersectWith(IEnumerable<TItem> other) {
		var otherSet = other as ISet<TItem> ?? other.ToHashSet();
		var addables = new List<TItem>();
		var removables = new List<TItem>();

		foreach (var item in this)
			if (otherSet.Contains(item))
				addables.Add(item); // item not in set, add it		
			else
				removables.Add(item); // item present in both, so remove it

		foreach (var item in removables)
			Remove(item);

		foreach (var item in addables)
			Add(item);
	}

	public virtual bool IsSubsetOf(IEnumerable<TItem> other)
		=> (other as ISet<TItem> ?? other.ToHashSet()).ContainsAll(this, Comparer);

	public virtual bool IsSupersetOf(IEnumerable<TItem> other)
		=> this.ContainsAll(other);

	public virtual bool IsProperSubsetOf(IEnumerable<TItem> other) {
		var otherSet = other as ISet<TItem> ?? other.ToHashSet();
		return Count < otherSet.Count && IsSubsetOf(otherSet);
	}

	public virtual bool IsProperSupersetOf(IEnumerable<TItem> other) {
		var otherSet = other as ISet<TItem> ?? other.ToHashSet();
		return Count > otherSet.Count && IsSupersetOf(otherSet);
	}

	public virtual bool Overlaps(IEnumerable<TItem> other)
		=> this.ContainsAny(other as ISet<TItem> ?? other.ToHashSet());

	public virtual bool SetEquals(IEnumerable<TItem> other) {
		var otherColl = other as ICollection<TItem> ?? other.ToArray();
		return Count == otherColl.Count && this.ContainsAll(otherColl); // note: will call ICollection.Contains (abstract here)
	}

	public abstract void CopyTo(TItem[] array, int arrayIndex);

	public abstract IEnumerator<TItem> GetEnumerator();

	IEnumerator IEnumerable.GetEnumerator() {
		return GetEnumerator();
	}


}
