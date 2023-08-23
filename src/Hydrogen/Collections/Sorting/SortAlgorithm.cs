// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System.Collections.Generic;

namespace Hydrogen;

public abstract class SortAlgorithm<T> {
	protected IComparer<T> Comparer;

	protected SortAlgorithm(IComparer<T> comparer = null) {
		Comparer = comparer ?? Comparer<T>.Default;
	}

	public abstract void Sort(IExtendedList<T> list);

	protected virtual void Swap(IExtendedList<T> list, long leftIdx, long rightIdx) {
		(list[leftIdx], list[rightIdx]) = (list[rightIdx], list[leftIdx]);
	}
}
