// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT NON-AI software license, see the accompanying file
// LICENSE or visit https://sphere10.com/legal/NON-AI-MIT.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System.Collections.Generic;

namespace Hydrogen;

public sealed class InsertionSorter<T> : SortAlgorithm<T> {

	public InsertionSorter(IComparer<T> comparer = null) : base(comparer) {
	}

	public override void Sort(IExtendedList<T> list) {
		var done = false;
		for (var i = 1; i < list.Count; i++) {
			var value = list[i];
			var j = i - 1;
			do {
				if (Comparer.Compare(list[j], value) > 0) {
					list[j + 1] = list[j];
					j--;

					if (j < 0) {
						done = true;
					}
				} else {
					done = true;
				}
			} while (done == false);

			list[j + 1] = value;
		}
	}

}

public static class InsertionSorter {
	public static void Sort<T>(IExtendedList<T> list, IComparer<T> comparer = null) {
		var sorter = new InsertionSorter<T>(comparer);
		sorter.Sort(list);
	}
}