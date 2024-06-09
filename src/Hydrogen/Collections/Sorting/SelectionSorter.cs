// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT NON-AI software license, see the accompanying file
// LICENSE or visit https://sphere10.com/legal/NON-AI-MIT.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System.Collections.Generic;

namespace Hydrogen;

public sealed class SelectionSorter<T> : SortAlgorithm<T>  {

	public SelectionSorter(IComparer<T> comparer = null) : base(comparer) {
	}

	public override void Sort(IExtendedList<T> list) {
		for (var i = 0; i < list.Count - 1; i++) {
			for (var j = i + 1; j < list.Count; j++) {
				if (Comparer.Compare(list[i], list[j]) > 0) {
					Swap(list, i, j);
				}
			}
		}
	}
}

public static class SelectionSorter {
	public static void Sort<T>(IExtendedList<T> list, IComparer<T> comparer = null) {
		var sorter = new SelectionSorter<T>(comparer);
		sorter.Sort(list);
	}
}
