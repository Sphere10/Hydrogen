using System;
using System.Collections.Generic;

namespace Sphere10.Framework {

	public interface IReadOnlyExtendedList<T> : IReadOnlyExtendedCollection<T>, IReadOnlyList<T> {
		int IndexOf(T item);
		IEnumerable<int> IndexOfRange(IEnumerable<T> items);
		T Read(int index);
		IEnumerable<T> ReadRange(int index, int count);
	}


	public static class IReadOnlyExtendedListExtensions {
		public static IEnumerable<T> ReadRange<T>(this IReadOnlyExtendedList<T> list, Range range) {
			var (offset, length) = range.GetOffsetAndLength(list.Count);
			return list.ReadRange(offset, length);
		}
	}


}