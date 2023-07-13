// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System.Collections.Generic;

namespace Hydrogen;

public sealed class BubbleSort<T> : SortAlgorithm<T> {

	public override void Execute(IExtendedList<T> list, IComparer<T> comparer) {
		bool swap = false;

		do {
			swap = false;
			for (int i = 0; i < list.Count - 1; i++) {
				if (comparer.Compare(list[i], list[i + 1]) > 0) {
					this.Swap(list, i, (i + 1));
					swap = true;
				}
			}
		} while (swap == true);
	}

}
