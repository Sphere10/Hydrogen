using System.Collections.Generic;

namespace Hydrogen {

	public interface IItemSizer<T> {

		bool IsStaticSize { get; }

		int StaticSize { get; }

		int CalculateTotalSize(IEnumerable<T> items, bool calculateIndividualItems, out int[] itemSizes);

		int CalculateSize(T item);
	}

}