using System.Collections.Generic;

namespace Sphere10.Framework {

	public interface IWriteOnlyExtendedList<in T> : IWriteOnlyExtendedCollection<T> {
		T this[int index] { set; }
		void Update(int index, T item);
		void UpdateRange(int index, IEnumerable<T> items);
		void Insert(int index, T item);
		void InsertRange(int index, IEnumerable<T> items);
		void RemoveAt(int index);
		void RemoveRange(int index, int count);
	}

}