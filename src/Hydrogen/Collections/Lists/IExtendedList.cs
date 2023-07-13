// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Collections.Generic;

namespace Hydrogen;

public interface IExtendedList<T> : IExtendedCollection<T>, IList<T>, IReadOnlyExtendedList<T>, IWriteOnlyExtendedList<T> {
	new T this[int index] { get; set; }

	new int IndexOf(T item);

	new void Insert(int index, T item);

	new void RemoveAt(int index);
}


public static class IExtendedListExtensions {

	public static void RemoveRange<T>(this IExtendedList<T> list, Range range) {
		var (offset, length) = range.GetOffsetAndLength(list.Count);
		list.RemoveRange(offset, length);
	}

	public static IExtendedList<TTo> CastListAs<TFrom, TTo>(this IExtendedList<TFrom> list) where TFrom : class where TTo : class {
		if (list is CastedExtendedList<TTo, TFrom> casted) // if casting back, use original
			return casted._from;
		return new CastedExtendedList<TFrom, TTo>(list);
	}
}
