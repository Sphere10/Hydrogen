using System.Collections.Generic;

namespace Sphere10.Framework {

	public interface IWriteOnlyExtendedCollection<in T> {
		void Add(T item);
		void AddRange(IEnumerable<T> items);
		bool Remove(T item);
		IEnumerable<bool> RemoveRange(IEnumerable<T> items);
		void Clear();
	}

	public static class IWriteOnlyExtendedCollectionExtensions {

		public static void AddRange<T>(this IWriteOnlyExtendedCollection<T> collection, params T[] items) {
			collection.AddRange((IEnumerable<T>)items);
		}
	}
}