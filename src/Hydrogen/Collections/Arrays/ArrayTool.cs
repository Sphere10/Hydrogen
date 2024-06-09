// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT NON-AI software license, see the accompanying file
// LICENSE or visit https://sphere10.com/legal/NON-AI-MIT.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using Hydrogen;
using System;
using System.Runtime.InteropServices;

// ReSharper disable CheckNamespace
namespace Tools;

public static class Array {


	public static T[][] Transpose<T>(T[][] array, int? expectedColumns = null) {
		Guard.ArgumentNotNull(array, nameof(array));
		if (expectedColumns != null)
			Guard.Argument(expectedColumns.Value > 0, nameof(expectedColumns), "Must be null or greater than or equal to 0");

		var columnCount = array.Length > 0 ? (expectedColumns ?? array[0].Length) : 0;
		var rowCount = array.Length;

		if (columnCount == 0 && rowCount > 0 || rowCount == 0 && columnCount > 0)
			throw new ArgumentException("Expected empty but was not");


		// Ensure all rows have consistent columns
		for (var i = 0; i < rowCount; i++)
			if (array[i].Length != columnCount)
				throw new ArgumentException($"Row at index {i} had {array[i].Length} columns but expected {columnCount}", nameof(array));

		// Transpose the array
		var result = new T[columnCount][];
		for (var i = 0; i < columnCount; i++) {
			result[i] = new T[rowCount];
			for (var j = 0; j < rowCount; j++)
				result[i][j] = array[j][i];
		}
		return result;

	}

	public static T PopHead<T>(ref T[] arr) {
		if (arr.Length == 0)
			throw new ArgumentOutOfRangeException(nameof(arr), "Empty");
		var head = arr[0];
		System.Array.Copy(arr, 1, arr, 0, arr.Length - 1);
		System.Array.Resize(ref arr, arr.Length - 1);
		return head;
	}

	public static void Fill<T>(T[] arr, T val, int? amount = null) {
		var length = amount ?? arr.Length;
		for (var i = 0; i < length; i++)
			arr[i] = val;
	}

	public static T[] Gen<T>(long size, T val) {
		var arr = new T[size];
		Fill(arr, val);
		return arr;
	}

	public static void ConcatInPlace<T>(ref T[] arr1, T[] arr2) {
		System.Array.Resize(ref arr1, arr1.Length + arr2.Length);
		System.Array.Copy(arr1, arr1.Length, arr2, 0, arr2.Length);
	}

	public static T[] Concat<T>(ReadOnlySpan<T> span0, T item) {
		return Concat(span0, MemoryMarshal.CreateReadOnlySpan(ref item, 1));
	}

	public static T[] Concat<T>(ReadOnlySpan<T> span0) {
		var result = new T[span0.Length];
		span0.CopyTo(result);
		return result;
	}

	public static T[] Concat<T>(ReadOnlySpan<T> span0, ReadOnlySpan<T> span1) {
		var result = new T[span0.Length + span1.Length];
		var resultSpan = result.AsSpan();
		span0.CopyTo(result);
		var from = span0.Length;
		span1.CopyTo(resultSpan.Slice(from));
		return result;
	}

	public static T[] Concat<T>(ReadOnlySpan<T> span0, ReadOnlySpan<T> span1, ReadOnlySpan<T> span2) {
		var result = new T[span0.Length + span1.Length + span2.Length];
		var resultSpan = result.AsSpan();
		span0.CopyTo(result);
		var from = span0.Length;
		span1.CopyTo(resultSpan.Slice(from));
		from += span1.Length;
		span2.CopyTo(resultSpan.Slice(from));
		return result;
	}

	public static T[] Concat<T>(ReadOnlySpan<T> span0, ReadOnlySpan<T> span1, ReadOnlySpan<T> span2, ReadOnlySpan<T> span3) {
		var result = new T[span0.Length + span1.Length + span2.Length + span3.Length];
		var resultSpan = result.AsSpan();
		span0.CopyTo(result);
		var from = span0.Length;
		span1.CopyTo(resultSpan.Slice(from));
		from += span1.Length;
		span2.CopyTo(resultSpan.Slice(from));
		from += span2.Length;
		span3.CopyTo(resultSpan.Slice(from));
		return result;
	}

	public static T[] Concat<T>(ReadOnlySpan<T> span0, ReadOnlySpan<T> span1, ReadOnlySpan<T> span2, ReadOnlySpan<T> span3, ReadOnlySpan<T> span4) {
		var result = new T[span0.Length + span1.Length + span2.Length + span3.Length + span4.Length];
		var resultSpan = result.AsSpan();
		span0.CopyTo(result);
		var from = span0.Length;
		span1.CopyTo(resultSpan.Slice(from));
		from += span1.Length;
		span2.CopyTo(resultSpan.Slice(from));
		from += span2.Length;
		span3.CopyTo(resultSpan.Slice(from));
		from += span3.Length;
		span4.CopyTo(resultSpan.Slice(from));
		return result;
	}

	public static T[] GetReversedCopy<T>(T[] arr) {
		var result = new T[arr.Length];
		for (var i = 0; i < arr.Length; i++)
			result[i] = arr[arr.Length - i - 1];
		return result;
	}

	public static T RandomElement<T>(T[] arr) {
		if (arr.Length == 0)
			throw new ArgumentException("array is empty", nameof(arr));

		return arr[Tools.Maths.RNG.Next(0, arr.Length - 1)];
	}

	public static T[] Clone<T>(T[] source) {
		return Clone(source, x => x);
	}

	public static T[] Clone<T>(T[] source, Func<T, T> itemCloner) {
		if (source == null)
			return null;

		var clonedArray = new T[source.Length];
		for (var i = 0; i < source.Length; i++)
			clonedArray[i] = itemCloner(source[i]);
		return clonedArray;
	}
}
