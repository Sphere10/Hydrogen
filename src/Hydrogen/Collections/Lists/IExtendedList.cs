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
using System.Linq;

namespace Hydrogen;

public interface IExtendedList<T> : IExtendedCollection<T>, IReadOnlyExtendedList<T>, IWriteOnlyExtendedList<T>, IList<T> {
	// Below interface methods resolve ambiguities between IReadOnlyExtendedList<T> and IList<T>
	new T this[int index] { get; set; }
	new T this[long index] { get; set; }

	new int IndexOf(T item);

	new void Insert(long index, T item);

	new void RemoveAt(long index);
}


public static class IExtendedListExtensions {

	public static void RemoveRange<T>(this IExtendedList<T> list, Range range) {
		var (offset, length) = range.GetOffsetAndLength(((IList<T>)list).Count);
		list.RemoveRange(offset, length);
	}

	/// <summary>
	/// Performs a binary search on the specified collection.
	/// </summary>
	/// <typeparam name="TItem">The type of the item.</typeparam>
	/// <param name="list">The list to be searched.</param>
	/// <param name="value">The value to search for.</param>
	/// <param name="comparer">The comparer that is used to compare the value with the list items.</param>
	/// <returns></returns>
	public static long BinarySearch<TItem>(this IExtendedList<TItem> list, TItem value, IComparer<TItem> comparer = null) {
		Guard.ArgumentNotNull(list, nameof(list));
		comparer ??= Comparer<TItem>.Default;
		return Tools.Collection.BinarySearch(list, value, 0, list.Count - 1, comparer.Compare);
	}
}