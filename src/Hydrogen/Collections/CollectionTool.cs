// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT NON-AI software license, see the accompanying file
// LICENSE or visit https://sphere10.com/legal/NON-AI-MIT.
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

/// <summary>
/// Provides small, allocation-friendly helpers for constructing and manipulating collections.
/// </summary>
public static class Collection {

	/// <summary>
	/// Wraps the specified item in an <see cref="IEnumerable{T}"/> so it can be used in LINQ pipelines.
	/// </summary>
	/// <typeparam name="T">The item type.</typeparam>
	/// <param name="item">The item to expose as a single-value sequence.</param>
	/// <returns>An enumerable that yields <paramref name="item"/> once.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static IEnumerable<T> AsEnumerable<T>(T item) {
		yield return item;
	}

	/// <summary>
	/// Yields the item if it is non-null; otherwise returns an empty sequence.
	/// Useful for optional items in fluent pipelines.
	/// </summary>
	/// <typeparam name="T">The item type.</typeparam>
	/// <param name="item">The candidate item.</param>
	/// <returns>An enumerable that yields <paramref name="item"/> when it is not <c>null</c>.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static IEnumerable<T> AsEnumerableWhenNotNull<T>(T item) where T : class {
		if (item != null)
			yield return item;
	}

	/// <summary>
	/// Returns an endless enumerable that can be used to drive loops without allocating new objects.
	/// </summary>
	public static IEnumerable<Void> Infinity {
		get {
			while (true)
				yield return Void.Value;
		}
	}

	/// <summary>
	/// Filters out <c>null</c> entries from the provided values.
	/// </summary>
	/// <typeparam name="T">The item type.</typeparam>
	/// <param name="values">Items to filter.</param>
	/// <returns>All non-null items from <paramref name="values"/>.</returns>
	public static IEnumerable<T> IgnoreNulls<T>(params T[] values) {
		return values.Where(v => v != null);
	}

	/// <summary>
	/// Materializes an array using the supplied generator for each index.
	/// </summary>
	/// <typeparam name="T">The element type.</typeparam>
	/// <param name="num">The number of elements to produce.</param>
	/// <param name="generator">Function invoked with the current index.</param>
	/// <returns>An array populated by <paramref name="generator"/>.</returns>
	public static T[] GenerateArray<T>(int num, Func<int, T> generator) {
		var arr = new T[num];
		for (int i = 0; i < num; i++)
			arr[i] = generator(i);
		return arr;
	}

	/// <summary>
	/// Generates an infinite sequence by repeatedly invoking the generator.
	/// </summary>
	/// <typeparam name="T">The generated type.</typeparam>
	/// <param name="generator">Function that produces the next value.</param>
	/// <returns>An enumerable that yields generator results forever.</returns>
	public static IEnumerable<T> Generate<T>(Func<T> generator) {
		while (true)
			yield return generator();
	}

	/// <summary>
	/// Invokes an action a fixed number of times using a 32-bit counter.
	/// </summary>
	/// <param name="action">The action to execute.</param>
	/// <param name="count">The number of invocations.</param>
	public static void Repeat(Action action, int count) {
		for (var i = 0; i < count; i++)
			action();
	}

	/// <summary>
	/// Invokes an action a fixed number of times using a 64-bit counter.
	/// </summary>
	/// <param name="action">The action to execute.</param>
	/// <param name="count">The number of invocations.</param>
	public static void Repeat(Action action, long count) {
		for (var i = 0L; i < count; i++)
			action();
	}

	/// <summary>
	/// Produces a sequence that repeats the provided value a given number of times.
	/// </summary>
	/// <typeparam name="T">The value type.</typeparam>
	/// <param name="value">Value to repeat.</param>
	/// <param name="count">Number of repetitions.</param>
	/// <returns>An enumerable yielding <paramref name="value"/> <paramref name="count"/> times.</returns>
	public static IEnumerable<T> RepeatValue<T>(T value, long count) {
		for (var i = 0L; i < count; i++)
			yield return value;
	}


	/// <summary>
	/// Determines whether the supplied index falls within the bounds of the collection.
	/// </summary>
	/// <typeparam name="T">Collection element type.</typeparam>
	/// <param name="collection">The collection to evaluate.</param>
	/// <param name="index">Zero-based index.</param>
	/// <returns><c>true</c> if the index is valid; otherwise <c>false</c>.</returns>
	public static bool ValidIndex<T>(IEnumerable<T> collection, int index) {
		if (index < 0)
			return false;
		return index < collection.Count();
	}

	/// <summary>
	/// Breaks a number into chunk sizes, yielding the final remainder as the last element.
	/// </summary>
	/// <param name="number">The total amount to partition.</param>
	/// <param name="chunk">The preferred chunk size.</param>
	/// <returns>A sequence of chunk sizes that sum to <paramref name="number"/>.</returns>
	public static IEnumerable<int> Partition(int number, int chunk) {
		while (number > 0) {
			yield return chunk < number ? chunk : number;
			number -= chunk;
		}
	}

	/// <summary>
	/// Breaks a 64-bit number into chunk sizes, yielding the final remainder as the last element.
	/// </summary>
	/// <param name="number">The total amount to partition.</param>
	/// <param name="chunk">The preferred chunk size.</param>
	/// <returns>A sequence of chunk sizes that sum to <paramref name="number"/>.</returns>
	public static IEnumerable<long> Partition(long number, long chunk) {
		while (number > 0) {
			yield return chunk < number ? chunk : number;
			number -= chunk;
		}
	}

	/// <summary>
	/// Performs a binary search over an <see cref="IExtendedList{T}"/> using a custom comparer.
	/// </summary>
	/// <typeparam name="TItem">The item type contained in the list.</typeparam>
	/// <typeparam name="TSearch">The search value type.</typeparam>
	/// <param name="list">The sorted list to search.</param>
	/// <param name="value">The value to locate.</param>
	/// <param name="lower">Lower bound index, inclusive.</param>
	/// <param name="upper">Upper bound index, inclusive.</param>
	/// <param name="comparer">Comparer returning negative/zero/positive like <see cref="Comparer{T}.Compare(T,T)"/>.</param>
	/// <returns>The index of the matching element or bitwise complement of the insertion point.</returns>
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

	/// <summary>
	/// Performs a binary search over a non-generic <see cref="IList"/> using a custom comparer.
	/// </summary>
	/// <typeparam name="TItem">Type of items stored in the list.</typeparam>
	/// <typeparam name="TSearch">Type of the search value.</typeparam>
	/// <param name="list">The sorted list to search.</param>
	/// <param name="value">Value to locate.</param>
	/// <param name="lower">Lower bound index, inclusive.</param>
	/// <param name="upper">Upper bound index, inclusive.</param>
	/// <param name="comparer">Comparer returning negative/zero/positive like <see cref="Comparer{T}.Compare(T,T)"/>.</param>
	/// <returns>The index of the matching element or bitwise complement of the insertion point.</returns>
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

	/// <summary>
	/// Generates a sequence of contiguous 64-bit integers starting at <paramref name="start"/>.
	/// </summary>
	/// <param name="start">First value in the sequence.</param>
	/// <param name="count">Number of values to return.</param>
	/// <returns>A sequence of <paramref name="count"/> values.</returns>
	public static IEnumerable<long> RangeL(long start, long count) {
		for (var i = 0L; i < count; i++)
			yield return start + i;
	}

	/// <summary>
	/// Validates a 64-bit length value can fit within 32-bit addressing used by Hydrogen collections.
	/// </summary>
	/// <param name="value">The length to validate.</param>
	/// <returns>The input value cast to <see cref="int"/> if within range.</returns>
	/// <exception cref="NotImplementedException">Thrown when 64-bit addressing is required.</exception>
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


	/// <summary>
	/// Validates a 64-bit index value can fit within 32-bit addressing used by Hydrogen collections.
	/// </summary>
	/// <param name="value">The index to validate.</param>
	/// <returns>The input value cast to <see cref="int"/> if within range.</returns>
	/// <exception cref="NotImplementedException">Thrown when 64-bit addressing is required.</exception>
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


	/// <summary>
	/// Resizes an array while enforcing Hydrogen's large-array limitations.
	/// </summary>
	/// <typeparam name="T">The element type.</typeparam>
	/// <param name="internalArray">The array reference to resize.</param>
	/// <param name="length">The desired length.</param>
	/// <exception cref="NotSupportedException">Thrown when the requested length exceeds supported bounds.</exception>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void ResizeArray<T>(ref T[] internalArray, long length) {
		if (length <= int.MaxValue) {
			System.Array.Resize(ref internalArray, checked((int)length));
		} else if (length != internalArray.LongLength) {	
			throw new NotSupportedException("Huge-arrays are not currently supported in Hydrogen");
		}
	}
}
