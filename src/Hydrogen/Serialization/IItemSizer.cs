using System.Collections.Generic;

namespace Sphere10.Framework {

	public interface IItemSizer<T> {

		bool IsStaticSize { get; }

		int StaticSize { get; }

		int CalculateTotalSize(IEnumerable<T> items, bool calculateIndividualItems, out int[] itemSizes);

		int CalculateSize(T item);
	}

}