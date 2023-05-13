// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System.Collections.Generic;

namespace Hydrogen {
	public sealed class InsertionSort<T> : SortAlgorithm<T>{
		
		public override void Execute(IExtendedList<T> list, IComparer<T> comparer) {
			for (int i = 1; i < list.Count; i++) {
				T value = list[i];

				int j = i - 1;

				bool done = false;

				do {
					if (comparer.Compare(list[j], value) > 0) {
						list[j + 1] = list[j];
						j--;

						if (j < 0) {
							done = true;
						}
					} else {
						done = true;
					}
				}
				while (done == false);

				list[j + 1] = value;
			}
		}
	}
}
