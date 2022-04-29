using System.Collections.Generic;

namespace Sphere10.Framework {

	public interface IReadOnlyExtendedCollection<T> : IReadOnlyCollection<T> {
		bool Contains(T item);
		IEnumerable<bool> ContainsRange(IEnumerable<T> items);
		void CopyTo(T[] array, int arrayIndex);
	}

}