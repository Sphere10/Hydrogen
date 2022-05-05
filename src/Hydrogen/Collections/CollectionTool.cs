//-----------------------------------------------------------------------
// <copyright file="CollectionTool.cs" company="Sphere 10 Software">
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
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Hydrogen;

// ReSharper disable CheckNamespace
namespace Tools {

	public static class Collection {

			public static IEnumerable<object> Infinity {
				get {
					while (true)
						yield return null;
				}
			}

			public static IEnumerable<T> IgnoreNulls<T>(params T[] values ) {
				return values.Where(v => v != null);
			}

			public static T[] GenerateArray<T>(int num, Func<int, T> generator) {
				var arr = new T[num];
				for (int i = 0; i < num; i++)
					arr[i] = generator(i);
				return arr;
			}

		    public static IEnumerable<T> Generate<T>(Func<T> generator) {
		        while (true)
		            yield return generator();
		    }

			public static void Repeat(Action action, int count) {
				for (var i = 0; i < count; i++)
					action();
			}


			public static bool ValidIndex<T>(IEnumerable<T> collection, int index) {
                if (index < 0)
                    return false;
                return index < collection.Count();
            }

			public static IEnumerable<int> Partition(int number, int chunk) {
				while (number > 0) {
					yield return chunk < number ? chunk : number;
					number -= chunk;
				}
			}

			public static int BinarySearch<TItem, TSearch>(IList<TItem> list, TSearch value, int lower, int upper, Func<TSearch, TItem, int> comparer) {
				Debug.Assert(list != null);
				Guard.ArgumentNotNull(comparer, nameof(comparer));
				while (lower <= upper) {
					var middle = lower + (upper - lower) / 2;
					var comparisonResult = comparer(value, list[middle]);
					if (comparisonResult < 0) {
						upper = middle - 1;
					} else if (comparisonResult > 0) {
						lower = middle + 1;
					} else {
						return middle;
					}
				}
				return ~lower;
			}

	}

	
}
