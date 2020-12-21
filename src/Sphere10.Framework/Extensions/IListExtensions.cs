//-----------------------------------------------------------------------
// <copyright file="IListExtensions.cs" company="Sphere 10 Software">
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
using System.Linq;
using System.Text;
using System.Collections;

namespace Sphere10.Framework {

	public static class IListExtensions {

		public static void RemoveAt<T>(this IList<T> list, Index index) {
			list.RemoveAt(index.GetOffset(list.Count));
		}


		public static IExtendedList<T> ToExtendedList<T>(this IList<T> list) {
            return new ExtendedListAdapter<T>(list); 
		}

	    public static void RemoveRangeSequentially<T>(this IList<T> list, int index, int count) {
			if (list is List<T> listImpl) {
	            listImpl.RemoveRange(index, count);
	            return;
	        }
            while(count-- > 0)
                list.RemoveAt(index);
	    }

		public static void InsertRangeSequentially<T>(this IList<T> list, int index, IEnumerable<T> items) {
			if (list is List<T> listImpl) {
				listImpl.InsertRange(index, items);
				return;
			}

			foreach (var (item, offset) in items.WithIndex()) {
				list.Insert(index + offset, item);
			}
		}

		public static void UpdateRangeSequentially<T>(this IList<T> list, int index, IEnumerable<T> items) {
			foreach (var (item, offset) in items.WithIndex()) {
				list[index + offset] = item;
			}
		}


		/// <summary>
		/// Binary searches an ordered list of Ranged  
		/// </summary>
		/// <typeparam name="TRange"></typeparam>
		/// <typeparam name="TValue"></typeparam>
		/// <param name="ranges"></param>
		/// <param name="value"></param>
		/// <param name="rangeComparer"> 
		/// Returns 0 if value is in the specified range;
		/// less than 0 if value is above the range;
		/// greater than 0 if value is below the range.
		///</param>
		/// <returns></returns>
		//public static int BinarySearchRange<TRange, TValue>(
		//    this IList<TRange> ranges,
		//    TValue value,
		//    Func<TRange, TValue, int> rangeComparer) {
		//    int min = 0;
		//    int max = ranges.Count - 1;

		//    while (min <= max) {
		//        int mid = (min + max) / 2;
		//        int comparison = rangeComparer(ranges[mid], value);
		//        if (comparison == 0) {
		//            return mid;
		//        }
		//        if (comparison < 0) {
		//            min = mid + 1;
		//        } else if (comparison > 0) {
		//            max = mid - 1;
		//        }
		//    }
		//    return ~min;
		//}

		public static void RemoveClones<T, TKey>(this IList<T> source, Func<T, TKey> keySelector, Func<IEnumerable<T>, T> duplicateResolver) {
	        source.RemoveClones((x) => x, (x, i, r) => source[i] = r, keySelector, duplicateResolver);
	    }

        public static void RemoveClones<T, TMember, TKey>(this IList<T> source, Func<T, TMember> memberGetter, Action<T, int, TMember> memberSetter, Func<TMember, TKey> keySelector, Func<IEnumerable<TMember>, TMember> duplicateResolver) {
            var membersByKey = source.Select(memberGetter).ToLookup(keySelector).ToDictionary(g => g.Key, g => duplicateResolver(g));
            for (int i = 0; i < source.Count; i++)
	            memberSetter(source[i], i, membersByKey[keySelector(memberGetter(source[i]))]);
	    }

		public static void Swap<T>(this IList<T> list, int fromIndex, int toIndex) {
			var temp = list[fromIndex];
			list[fromIndex] = list[toIndex];
			list[toIndex] = temp;
		}

		public static IList<T> RotateLeft<T>(this IList<T> list, int num = 1) {
			if (list.Count == 0)
				return list;

			if (num == 0)
				return list;

			return list.RotateLeft(0, list.Count - 1, num);
		}

        public static IList<T> RotateLeft<T>(this IList<T> list, int fromIndex, int toIndex, int num = 1) {
            if (list == null)
                throw new ArgumentNullException("list");

            if (list.Count == 0)
                return list;

            if (num == 0)
                return list;

            num = num % (toIndex - fromIndex + 1);

            var temp = new Queue<T>();
            for (int i = fromIndex; i < fromIndex + num; i++)
                temp.Enqueue(list[i]);

            for (int i = fromIndex + num; i <= toIndex; i++)
                list[i - num] = list[i];

            for (int i = toIndex - num + 1; i <= toIndex; i++)
                list[i] = temp.Dequeue();



            return list;
        }

		public static IList<T> RotateRight<T>(this IList<T> list, int num = 1) {
			if (list.Count == 0)
				return list;

			if (num == 0)
				return list;

			return list.RotateRight(0, list.Count - 1, num);
		}


		public static IList<T> RotateRight<T>(this IList<T> list, int fromIndex, int toIndex, int num = 1) {
			if (list == null)
				throw new ArgumentNullException("list");


			//Validator.ForValue(fromIndex).LessThan(list.Count).LessThanEqualTo(toIndex); BUG LessthanEqualTo compared to LessThan
			//Validator.ForValue(toIndex).LessThan(list.Count);

			if (list.Count == 0)
				return list;

			if (num == 0)
				return list;



			num = num % (toIndex - fromIndex + 1);

			var temp = new Queue<T>();
			for (int i = toIndex - num + 1; i <= toIndex; i++)
				temp.Enqueue(list[i]);

			for (int i = toIndex - num; i >= fromIndex; i--)
				list[i + num] = list[i];

			for (int i = fromIndex; i < fromIndex + num; i++)
				list[i] = temp.Dequeue();



			return list;
		}

	}
}
