//-----------------------------------------------------------------------
// <copyright file="QuickSort.cs" company="Sphere 10 Software">
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

namespace Sphere10.Framework {
	public sealed class QuickSort<T> : SortAlgorithm<T>  {
		
		public override void Execute(IExtendedList<T> list, IComparer<T> comparer) {
			this.ExecuteRecursive(list, 0, list.Count - 1, comparer);
		}

		private void ExecuteRecursive(IExtendedList<T> list, int left, int right, IComparer<T> comparer) {
			int segmentLeft = left;
			int segmentRight = right;

			var pivot = list[(left + right) / 2];

			do {
				while (comparer.Compare(list[segmentLeft],pivot) < 0 && segmentLeft < right) {
					segmentLeft++;
				}

				while (comparer.Compare(pivot, list[segmentRight]) < 0 && segmentRight > left) {
					segmentRight--;
				}

				if (segmentLeft <= segmentRight) {
					this.Swap(list, segmentLeft, segmentRight);
					segmentLeft++; segmentRight--;
				}

			}
			while (segmentLeft <= segmentRight);

			if (left < segmentRight) {
				this.ExecuteRecursive(list, left, segmentRight, comparer);
			}

			if (segmentLeft < right) {
				this.ExecuteRecursive(list, segmentLeft, right, comparer);
			}
		}


		public static void Run(IExtendedList<T> list, IComparer<T> comparer) {
			var sorter = new QuickSort<T>();
			sorter.Execute(list, comparer);
		}
	}
}
