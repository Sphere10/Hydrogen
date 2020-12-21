using System;
using System.Collections.Generic;
using System.Linq;

namespace Sphere10.Framework {

	internal class ExtendedListHelper {
		public static int IndexOfSequenceImpl<T>(IExtendedList<T> extendedList, IEnumerable<T> items) {
			var indices = extendedList.IndexOfRange(items).ToArray();
			if (indices.Length == 0)
				return -1;
			return Enumerable.Range(indices[0], indices.Length).SequenceEqual(indices) ? indices[0] : -1;
		}

		public static int RemoveRangeImpl<T>(IExtendedList<T> extendedList, IEnumerable<T> items) {
			return
				extendedList
				.IndexOfRange(items)
				.Where(index => index >= 0)
				.Apply(extendedList.RemoveAt)
				.Count();
		}
	}

}