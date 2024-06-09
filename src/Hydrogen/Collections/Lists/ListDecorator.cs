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

public abstract class ListDecorator<TItem, TList> : IList<TItem> where TList : IList<TItem> {
	protected readonly IList<TItem> InternalList;

	protected ListDecorator(TList internalList) {
		InternalList = internalList;
	}

	public virtual int IndexOf(TItem item) => InternalList.IndexOf(item);

	public virtual void Insert(int index, TItem item) => InternalList.Insert(index, item);

	public virtual bool Remove(TItem item) => InternalList.Remove(item);

	public virtual void RemoveAt(int index) => InternalList.RemoveAt(index);

	public virtual TItem this[int index] {
		get => InternalList[index];
		set => InternalList[index] = value;
	}

	public virtual void Add(TItem item) => InternalList.Add(item);

	public virtual void Clear() => InternalList.Clear();

	public virtual bool Contains(TItem item) => InternalList.Contains(item);

	public virtual void CopyTo(TItem[] array, int arrayIndex) => InternalList.CopyTo(array, arrayIndex);

	public virtual int Count => InternalList.Count;

	public virtual bool IsReadOnly => InternalList.IsReadOnly;

	public virtual IEnumerator<TItem> GetEnumerator() => InternalList.GetEnumerator();

	IEnumerator IEnumerable.GetEnumerator() => InternalList.GetEnumerator();

}

public abstract class ListDecorator<TItem> : ListDecorator<TItem, IList<TItem>> {
	protected ListDecorator(IList<TItem> internalList)
		: base(internalList) {
	}

}
