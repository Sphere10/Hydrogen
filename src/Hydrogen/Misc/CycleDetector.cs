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

/// <summary>
/// Detects when a cycle occurs in a sequence of items. (NOT TESTED)
/// </summary>
/// <remarks>A cycle is defined when a subsequence has repeated once.</remarks>
/// <example>
/// Adding events to a <see cref="CycleDetector{T}"/> where T is <see cref="int"/>:
///   1, 2, 3, 1, 2, 3 will return true on 6'th <see cref="Add"/>, false on rest.
///   4, 1, 4, 1 will return true on 4'th, false on rest.
///   7, 7 will return true on 2nd, false on rest.
/// </example>
public class CycleDetector<T> {
	private readonly List<T> _items;
	private int _level;
	private readonly IEqualityComparer<T> _comparer;

	public CycleDetector()
		: this(EqualityComparer<T>.Default) {
	}

	public CycleDetector(IEqualityComparer<T> comparer) {
		_items = new List<T>();
		_level = 0;
		_comparer = comparer;
		LastCycle = Array.Empty<T>();
	}

	public bool CaptureCycle { get; set; } = true;

	public T[] LastCycle { get; private set; }

	public bool Add(T item) {
		if (_items.Count == 0) {
			_items.Add(item);
			return false;
		}

		if (_comparer.Equals(_items[_level], item)) {
			_level++;
		} else {
			_items.AddRangeSequentially(_items.ReadRangeSequentially(0, _level));
			_items.Add(item);
			_level = 0;
		}

		if (_level == _items.Count) {
			if (CaptureCycle)
				LastCycle = _items.ToArray();
			Reset();
			return true;
		}

		return false;
	}

	public void Reset() {
		_items.Clear();
		_level = 0;
	}
}
