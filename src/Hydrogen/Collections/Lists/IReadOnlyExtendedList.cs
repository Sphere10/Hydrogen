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

public interface IReadOnlyExtendedList<T> : IReadOnlyExtendedCollection<T>, IReadOnlyList<T> {
	long IndexOfL(T item);

	IEnumerable<long> IndexOfRange(IEnumerable<T> items);

	T Read(long index);

	IEnumerable<T> ReadRange(long index, long count);

	T this[long index] { get; }
}


public static class IReadOnlyExtendedListExtensions {
	public static IEnumerable<T> ReadRange<T>(this IReadOnlyExtendedList<T> list, Range range) {
		var (offset, length) = range.GetOffsetAndLength(list.Count);
		return list.ReadRange(offset, length);
	}
}
