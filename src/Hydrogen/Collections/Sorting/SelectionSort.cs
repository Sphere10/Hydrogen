//-----------------------------------------------------------------------
// <copyright file="SelectionSort.cs" company="Sphere 10 Software">
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

using System;
using System.Collections.Generic;

namespace Sphere10.Framework {
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
