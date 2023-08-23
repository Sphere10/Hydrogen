// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System.Collections.Generic;

namespace Hydrogen;

public sealed class QuickSorter<T> : SortAlgorithm<T> {

	public QuickSorter(IComparer<T> comparer = null) : base(comparer) {
	}

	public override void Sort(IExtendedList<T> list) {
		ExecuteRecursive(list, 0, list.Count - 1);
	}

	private void ExecuteRecursive(IExtendedList<T> list, long left, long right) {
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
			ExecuteRecursive(list, left, segmentRight);
		}

		if (segmentLeft < right) {
			ExecuteRecursive(list, segmentLeft, right);
		}
	}

}

public static class QuickSorter {
	public static void Sort<T>(IExtendedList<T> list, IComparer<T> comparer = null) {
		var sorter = new QuickSorter<T>(comparer);
		sorter.Sort(list);
	}
}
