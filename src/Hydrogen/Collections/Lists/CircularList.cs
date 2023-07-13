// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Collections.Generic;

namespace Hydrogen;

public class CircularList<TItem> : SingularListBase<TItem> {

	public readonly IList<TItem> _list;
	private int _startIndex;
	private readonly int _maxSize;

	public CircularList(int maxSize)
		: this(new List<TItem>(maxSize), maxSize) {
	}

	public CircularList(IList<TItem> list, int maxSize) {
		_startIndex = 0;
		_maxSize = maxSize;
		_list = list;
	}

	public override int Count => _list.Count;

	public override bool IsReadOnly => false;

	public override void Add(TItem item) {
		if (_list.Count < _maxSize) {
			_list.Add(item);
		} else {
			// Override the last item before the new first item of the list
			_list[_startIndex] = item;
			_startIndex = (_startIndex + 1) % _maxSize;
		}
	}

	public override TItem Read(int index) {
		if (index < 0 || index >= _list.Count)
			throw new IndexOutOfRangeException();

		return _list[(_startIndex + index) % _maxSize];

	}

	public override IEnumerator<TItem> GetEnumerator() {
		for (var i = 0; i < _list.Count; i++) {
			var realIndex = (_startIndex + i) % _maxSize;
			yield return _list[realIndex];
		}
	}

	public override void Insert(int index, TItem item) {
		throw new NotSupportedException();
	}

	public override void Update(int index, TItem item) {
		var realIndex = (_startIndex + index) % _maxSize;
		_list[realIndex] = item;
	}

	public override bool Remove(TItem item) {
		throw new NotSupportedException();
	}

	public override void RemoveAt(int index) {
		throw new NotSupportedException();
	}

	public override bool Contains(TItem item) {
		return _list.Contains(item);
	}

	public override int IndexOf(TItem item) {
		var baseIndex = _list.IndexOf(item);
		if (baseIndex == -1) // not found?
			return -1;

		if (baseIndex >= _startIndex) {
			return baseIndex - _startIndex;
		}
		return _list.Count - _startIndex + baseIndex;
	}

	public override void Clear() {
		_startIndex = 0;
		_list.Clear();
	}

	public override void CopyTo(TItem[] array, int arrayIndex) {
		for (int i = 0; i < _list.Count; i++) {
			int realIndex = (_startIndex + i) % _maxSize;
			array[arrayIndex + i] = _list[realIndex];
		}
	}

}
