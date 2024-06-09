// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT NON-AI software license, see the accompanying file
// LICENSE or visit https://sphere10.com/legal/NON-AI-MIT.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System.Collections.Generic;

namespace Hydrogen;

public sealed class QuickSorter<T> : SortAlgorithm<T> {

	public QuickSorter(IComparer<T> comparer = null) : base(comparer) {
	}

	public override void Sort(IExtendedList<T> list) {
		var listCount = list.Count;
		if (listCount > 0)
			SortRecursive(list, 0, listCount - 1);
	}

	private void SortRecursive(IExtendedList<T> list, long left, long right) {
		var segmentLeft = left;
		var segmentRight = right;
		var pivot = list[(left + right) / 2];
		do {
			while (Comparer.Compare(list[segmentLeft], pivot) < 0 && segmentLeft < right) {
				segmentLeft++;
			}

			while (Comparer.Compare(pivot, list[segmentRight]) < 0 && segmentRight > left) {
				segmentRight--;
			}

			if (segmentLeft <= segmentRight) {
				Swap(list, segmentLeft, segmentRight);
				segmentLeft++;
				segmentRight--;
			}

		} while (segmentLeft <= segmentRight);

		if (left < segmentRight) {
			SortRecursive(list, left, segmentRight);
		}

		if (segmentLeft < right) {
			SortRecursive(list, segmentLeft, right);
		}
	}

}

public static class QuickSorter {
	public static void Sort<T>(IExtendedList<T> list, IComparer<T> comparer = null) {
		var sorter = new QuickSorter<T>(comparer);
		sorter.Sort(list);
	}
}
