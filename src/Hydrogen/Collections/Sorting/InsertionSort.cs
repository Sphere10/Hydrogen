//-----------------------------------------------------------------------
// <copyright file="InsertionSort.cs" company="Sphere 10 Software">
//
// Copyright (c) Sphere 10 Software. All rights reserved. (http://www.sphere10.com)
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// <author>Herman Schoenfeld</author>
// <date>2018</date>
// </copyright>
//-----------------------------------------------------------------------

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
