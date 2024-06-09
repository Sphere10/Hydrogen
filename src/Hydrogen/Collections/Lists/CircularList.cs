// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT NON-AI software license, see the accompanying file
// LICENSE or visit https://sphere10.com/legal/NON-AI-MIT.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Collections.Generic;

namespace Hydrogen;

public class CircularList<TItem> : SingularListBase<TItem> {

	public readonly IExtendedList<TItem> _list;
	private long _startIndex;
	private readonly long _maxSize;

	public CircularList(long maxSize)
		: this(new ExtendedList<TItem>(maxSize), maxSize) {
	}

	public CircularList(IExtendedList<TItem> list, long maxSize) {
		_startIndex = 0;
		_maxSize = maxSize;
		_list = list;
	}

	public override long Count => _list.Count;

	public override bool IsReadOnly => false;

	public override void Add(TItem item) {
		if (_list.Count < _maxSize) {
			_list.Add(item);
		} else {
			// Override the last item before the new first item of the list
			_list[_startIndex] = item;
			_startIndex = (_startIndex + 1) % _maxSize;
		}
		UpdateVersion();
	}

	public override TItem Read(long index) {
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

	public override void Insert(long index, TItem item) {
		throw new NotSupportedException();
	}

	public override void Update(long index, TItem item) {
		var realIndex = (_startIndex + index) % _maxSize;
		_list[realIndex] = item;
		UpdateVersion();
	}

	public override bool Remove(TItem item) {
		throw new NotSupportedException();
	}

	public override void RemoveAt(long index) {
		throw new NotSupportedException();
	}

	public override bool Contains(TItem item) {
		return _list.Contains(item);
	}

	public override long IndexOfL(TItem item) {
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
		var startIndexI = Tools.Collection.CheckNotImplemented64bitAddressingIndex(_startIndex);
		for (int i = 0; i < _list.Count; i++) {
			var realIndex = (startIndexI + i) % _maxSize;
			array[arrayIndex + i] = _list[realIndex];
		}
	}

}
