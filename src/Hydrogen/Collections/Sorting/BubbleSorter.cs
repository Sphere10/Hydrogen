// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT NON-AI software license, see the accompanying file
// LICENSE or visit https://sphere10.com/legal/NON-AI-MIT.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System.Collections.Generic;

namespace Hydrogen;

public sealed class BubbleSorter<T> : SortAlgorithm<T> {

	public BubbleSorter(IComparer<T> comparer = null) : base(comparer) {
	}

	public override void Sort(IExtendedList<T> list) {
		bool swapped;
		do {
			swapped = false;
			for (var i = 0; i < list.Count - 1; i++) {
				if (Comparer.Compare(list[i], list[i + 1]) > 0) {
					Swap(list, i, i + 1);
					swapped = true;
				}
			}
		} while (swapped);
	}

}

public static class BubbleSorter { 
	public static void Sort<T>(IExtendedList<T> list, IComparer<T> comparer = null) {
		var sorter = new BubbleSorter<T>(comparer);
		sorter.Sort(list);
	}
}
