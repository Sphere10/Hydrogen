using System.Collections.Generic;

namespace Sphere10.Framework {

	public interface IExtendedCollection<T> : ICollection<T>, IReadOnlyExtendedCollection<T>, IWriteOnlyExtendedCollection<T> {
		new int Count { get; }
		new void Add(T item);
		new void Clear();
		new bool Contains(T item);
		new void CopyTo(T[] array, int arrayIndex);
		new bool Remove(T item);
	}

}