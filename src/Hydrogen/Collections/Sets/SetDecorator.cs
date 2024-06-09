// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT NON-AI software license, see the accompanying file
// LICENSE or visit https://sphere10.com/legal/NON-AI-MIT.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System.Collections;
using System.Collections.Generic;

namespace Hydrogen;

public abstract class SetDecorator<TItem, TSet> : ISet<TItem> where TSet : ISet<TItem> {

	protected readonly TSet InternalSet;

	protected SetDecorator(TSet internalSet) {
		InternalSet = internalSet;
	}

	public virtual IEnumerator<TItem> GetEnumerator() => InternalSet.GetEnumerator();

	IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

	public virtual bool Add(TItem item) => InternalSet.Add(item);

	void ICollection<TItem>.Add(TItem item) => Add(item);

	public virtual void ExceptWith(IEnumerable<TItem> other) => InternalSet.ExceptWith(other);

	public virtual void IntersectWith(IEnumerable<TItem> other) => InternalSet.IntersectWith(other);

	public virtual bool IsProperSubsetOf(IEnumerable<TItem> other) => InternalSet.IsProperSubsetOf(other);

	public virtual bool IsProperSupersetOf(IEnumerable<TItem> other) => InternalSet.IsProperSupersetOf(other);

	public virtual bool IsSubsetOf(IEnumerable<TItem> other) => InternalSet.IsSubsetOf(other);

	public virtual bool IsSupersetOf(IEnumerable<TItem> other) => InternalSet.IsSupersetOf(other);

	public virtual bool Overlaps(IEnumerable<TItem> other) => InternalSet.Overlaps(other);

	public virtual bool SetEquals(IEnumerable<TItem> other) => InternalSet.SetEquals(other);

	public virtual void SymmetricExceptWith(IEnumerable<TItem> other) => InternalSet.SymmetricExceptWith(other);

	public virtual void UnionWith(IEnumerable<TItem> other) => InternalSet.UnionWith(other);

	public virtual void Clear() => InternalSet.Clear();

	public virtual bool Contains(TItem item) => InternalSet.Contains(item);

	public virtual void CopyTo(TItem[] array, int arrayIndex) => InternalSet.CopyTo(array, arrayIndex);

	public virtual bool Remove(TItem item) => InternalSet.Remove(item);

	public virtual int Count => InternalSet.Count;

	public virtual bool IsReadOnly => InternalSet.IsReadOnly;

}

public class SetDecorator<TItem> : SetDecorator<TItem, ISet<TItem>> {
	public SetDecorator(ISet<TItem> internalSet) : base(internalSet) {
	}
}