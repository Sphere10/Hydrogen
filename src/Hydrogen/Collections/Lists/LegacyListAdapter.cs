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

public sealed class LegacyListAdapter<T> : IList {
	private readonly IList<T> _genericList;

	public LegacyListAdapter(IList<T> wrappedList) {
		Guard.ArgumentNotNull(wrappedList, nameof(wrappedList));
		_genericList = wrappedList;
	}

	public int Add(object value) {
		_genericList.Add((T)value);
		return _genericList.Count - 1;
	}

	public void Clear() {
		_genericList.Clear();
	}

	public bool Contains(object value) {
		return _genericList.Contains((T)value);
	}

	public int IndexOf(object value) {
		return _genericList.IndexOf((T)value);
	}

	public void Insert(int index, object value) {
		_genericList.Insert(index, (T)value);
	}

	public bool IsFixedSize => false;

	public bool IsReadOnly => _genericList.IsReadOnly;

	public void Remove(object value) {
		_genericList.Remove((T)value);
	}

	public void RemoveAt(int index) {
		_genericList.RemoveAt(index);
	}

	public object this[int index] {
		get => _genericList[index];
		set => _genericList[index] = (T)value;
	}

	public void CopyTo(Array array, int index) {
		_genericList.CopyTo((T[])array, index);
	}

	public int Count => _genericList.Count;

	public bool IsSynchronized => false;

	public object SyncRoot => this;

	public IEnumerator GetEnumerator() {
		return _genericList.GetEnumerator();
	}
}