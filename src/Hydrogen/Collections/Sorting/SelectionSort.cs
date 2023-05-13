// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Collections.Generic;

namespace Hydrogen {
	internal class SelectionSort<T> : SortAlgorithm<T> where T : IComparable<T> {
		public override void Execute(IExtendedList<T> list, IComparer<T> comparer) {
			for (int i = 0; i < list.Count - 1; i++) {
				for (int j = i + 1; j < list.Count; j++) {
					if (comparer.Compare( list[i], list[j]) > 0) {
						this.Swap(list, i, j);
					}
				}
			}
		}
	}
}
