// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT NON-AI software license, see the accompanying file
// LICENSE or visit https://sphere10.com/legal/NON-AI-MIT.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System.Collections.Generic;

namespace Hydrogen.Misc;

/// <summary>
/// Compares two sequences.
/// </summary>
/// <typeparam name="T">Type of item in the sequences.</typeparam>
public class EnumerableComparer<T> : IComparer<IEnumerable<T>> {
	private readonly IComparer<T> _elementComparer;

	public EnumerableComparer() {
		_elementComparer = Comparer<T>.Default;
	}

	public EnumerableComparer(IComparer<T> comparer) {
		_elementComparer = comparer;
	}

	public int Compare(IEnumerable<T> x, IEnumerable<T> y) {
		using IEnumerator<T> leftIt = x.GetEnumerator();
		using IEnumerator<T> rightIt = y.GetEnumerator();
		while (true) {
			bool left = leftIt.MoveNext();
			bool right = rightIt.MoveNext();

			if (!(left || right)) return 0;

			if (!left) return -1;
			if (!right) return 1;

			int itemResult = _elementComparer.Compare(leftIt.Current, rightIt.Current);
			if (itemResult != 0) return itemResult;
		}

	}
}
