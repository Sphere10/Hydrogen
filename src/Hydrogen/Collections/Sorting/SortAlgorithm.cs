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
	public abstract void Execute(IExtendedList<T> list, IComparer<T> comparer);

	protected virtual void Swap(IExtendedList<T> list, long leftIdx, long rightIdx) {
		T temp = list[leftIdx];
		list[leftIdx] = list[rightIdx];
		list[rightIdx] = temp;
	}
}


public static class SortAlgorithmExtensions {
	public static void Execute<T>(this SortAlgorithm<T> algorithm, IExtendedList<T> list) {
		algorithm.Execute(list, Comparer<T>.Default);
	}
}
