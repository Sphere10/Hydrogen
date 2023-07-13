// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System.Collections.Generic;

namespace Hydrogen;

public interface IWriteableExtendedList<T> {
	void AddRange(IEnumerable<T> items);

	void UpdateRange(int index, IEnumerable<T> items);

	void InsertRange(int index, IEnumerable<T> items);

	void RemoveRange(int index, int count);
}


public static class IWriteableExtendedListExtensions {

	public static void AddRange<TItem>(this IExtendedList<TItem> extendedList, params TItem[] items) {
		extendedList.AddRange(items);
	}

	public static void Add<TItem>(this IExtendedList<TItem> extendedList, TItem item) {
		extendedList.AddRange(new[] { item });
	}

	public static void Insert<TItem>(this IExtendedList<TItem> extendedList, int index, params TItem[] items) {
		extendedList.InsertRange(index, items);
	}

	public static void Insert<TItem>(this IExtendedList<TItem> extendedList, int index, TItem item) {
		extendedList.InsertRange(index, new[] { item });
	}

	public static void Update<T>(this IWriteableExtendedList<T> list, int index, params T[] items) {
		list.UpdateRange(index, items);
	}

	public static void Update<T>(this IWriteableExtendedList<T> list, int index, T item) {
		list.UpdateRange(index, new[] { item });
	}

	public static void RemoveAt<TItem>(this IWriteableExtendedList<TItem> extendedList, int index) {
		extendedList.RemoveRange(index, 1);
	}

}
