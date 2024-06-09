// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT NON-AI software license, see the accompanying file
// LICENSE or visit https://sphere10.com/legal/NON-AI-MIT.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Hydrogen;

public static class ArrayExtensions {

	public static T[] ReverseArray<T>(this T[] array) {
		Array.Reverse(array);
		return array;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static T[] Copy<T>(this T[] array)
		=> SubArray(array, 0);

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static T[] SubArray<T>(this T[] buffer, int offset)
		=> SubArray(buffer, offset, buffer.Length - offset);

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static T[] SubArray<T>(this T[] buffer, int offset, int length) {
		T[] middle = new T[length];
		Array.Copy(buffer, offset, middle, 0, length);
		return middle;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static T[] Left<T>(this T[] buffer, int length)
		=> buffer.SubArray(0, length);

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static T[] Right<T>(this T[] buffer, int length)
		=> buffer.SubArray(buffer.Length - length, length);

	public static void InsertionSort<T>(this T[] array, int index, int count, IComparer<T> comparer) {
		int limit = index + count;
		for (int i = index + 1; i < limit; i++) {
			var item = array[i];

			int j = i - 1;
			while (comparer.Compare(array[j], item) > 0) {
				array[j + 1] = array[j];
				j--;
				if (j < index)
					break;
			}

			array[j + 1] = item;
		}
	}

	public static void InsertionSort<T>(this T[] array, IComparer<T> comparer) {
		InsertionSort<T>(array, 0, array.Length, comparer);
	}


	public static string ToString<T>(this T[] array, string separator) {
		return "{" + String.Join<T>(separator, array) + "}";
	}


}
