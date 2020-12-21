using System.Collections.Generic;

namespace Sphere10.Framework {

	public interface IExtendedList<T> : IExtendedCollection<T>, IList<T>, IReadOnlyExtendedList<T>, IWriteOnlyExtendedList<T> {
		new T this[int index] { get; set;  }
		new int IndexOf(T item);
		new void Insert(int index, T item);
		new void RemoveAt(int index);
	}

}