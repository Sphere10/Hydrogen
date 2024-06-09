// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT NON-AI software license, see the accompanying file
// LICENSE or visit https://sphere10.com/legal/NON-AI-MIT.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Collections;
using System.Collections.Generic;

namespace Hydrogen;

public class StackAdapter<T> : IStack<T> {
	
	private readonly IExtendedList<T> _internalList;

	public StackAdapter(IExtendedList<T> internalExtendedList)  {
		Guard.ArgumentNotNull(internalExtendedList, nameof(internalExtendedList));
		_internalList = internalExtendedList;
	}
	
	public virtual long Count => _internalList.Count;

	int ICollection<T>.Count => checked((int)Count);

	public virtual bool IsReadOnly => _internalList.IsReadOnly;

	public virtual bool TryPeek(out T value) {
		var count = Count;
		var itemIX = count - 1;
		if (itemIX < 0) {
			value = default(T);
			return false;
		}
		value = _internalList.Read(itemIX);
		return true;
	}

	public virtual bool TryPop(out T value) {
		var count = Count;
		if (count <= 0) {
			value = default(T);
			return false;
		}
		value = _internalList.Read(count - 1);
		_internalList.RemoveAt(count - 1);
		return true;
	}

	public virtual void Push(T item) => _internalList.Add(item);

	public virtual void Clear() => _internalList.Clear();

	public virtual IEnumerator<T> GetEnumerator() => _internalList.GetEnumerator();

	IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
	
	public void Add(T item) => Push(item);

	public bool Contains(T item) => throw new NotSupportedException("Not supported on this collection");

	public void CopyTo(T[] array, int arrayIndex) => throw new NotSupportedException("Not supported on this collection");

	public bool Remove(T item) => throw new NotSupportedException("Not supported on this collection");
	
}
