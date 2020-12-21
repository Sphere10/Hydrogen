//-----------------------------------------------------------------------
// <copyright file="SortStrategy.cs" company="Sphere 10 Software">
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

	public abstract class SortAlgorithm<T> {
		public abstract void Execute(IExtendedList<T> list, IComparer<T> comparer);
		protected virtual void Swap(IExtendedList<T> list, int leftIdx, int rightIdx) {
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
}
