// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Hydrogen;
using Void = Hydrogen.Void;

// ReSharper disable CheckNamespace
namespace Tools;

public static class Collection {

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static IEnumerable<T> AsEnumerable<T>(T item) {
		yield return item;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static IEnumerable<T> AsEnumerableWhenNotNull<T>(T item) where T : class {
		if (item != null)
			yield return item;
	}

	public static IEnumerable<Void> Infinity {
		get {
			while (true)
				yield return Void.Value;
		}
	}

	public static IEnumerable<T> IgnoreNulls<T>(params T[] values) {
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

	public static void Repeat(Action action, long count) {
		for (var i = 0L; i < count; i++)
			action();
	}

	public static IEnumerable<T> RepeatValue<T>(T value, long count) {
		for (var i = 0L; i < count; i++)
			yield return value;
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

	public static IEnumerable<long> Partition(long number, long chunk) {
		while (number > 0) {
			yield return chunk < number ? chunk : number;
			number -= chunk;
		}
	}

	public static long BinarySearch<TItem, TSearch>(IExtendedList<TItem> list, TSearch value, long lower, long upper, Func<TSearch, TItem, int> comparer) {
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

	public static int BinarySearch<TItem, TSearch>(IList list, TSearch value, int lower, int upper, Func<TSearch, TItem, int> comparer) {
		Debug.Assert(list != null);
		Guard.ArgumentNotNull(comparer, nameof(comparer));
		while (lower <= upper) {
			var middle = lower + (upper - lower) / 2;
			var comparisonResult = comparer(value, (TItem)list[middle]);
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

	public static IEnumerable<long> RangeL(long start, long count) {
		for (var i = 0L; i < count; i++)
			yield return start + i;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int CheckNotImplemented64bitAddressingLength(long value) {
		if (value < int.MinValue)
			throw new NotImplementedException("64-bit addressing is not currently implemented in Hydrogen");

		if (value > int.MaxValue)
			throw new NotImplementedException("64-bit addressing is not currently implemented in Hydrogen");

		unchecked {
			return (int)value;
		}
	}


	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int CheckNotImplemented64bitAddressingIndex(long value) {
		if (value < int.MinValue)
			throw new NotImplementedException("64-bit addressing is not currently implemented in Hydrogen");

		if (value > int.MaxValue)
			throw new NotImplementedException("64-bit addressing is not currently implemented in Hydrogen");

		unchecked {
			return (int)value;
		}
	}


	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void ResizeArray<T>(ref T[] internalArray, long length) {
		if (length <= int.MaxValue) {
			System.Array.Resize(ref internalArray, checked((int)length));
		} else if (length != internalArray.LongLength) {	
			throw new NotSupportedException("Huge-arrays are not currently supported in Hydrogen");
		}
	}
}
