using System.Collections.Generic;
using System.Linq;

namespace Sphere10.Framework {

	// ReSharper disable PossibleInvalidOperationException

	public class FixedSizeObjectSizer<TItem> : IObjectSizer<TItem> {

		public FixedSizeObjectSizer(int fixedSize) {
			FixedSize = fixedSize;
		}

		public bool IsFixedSize => true;

		public int FixedSize { get; }

		public int CalculateTotalSize(IEnumerable<TItem> items, bool calculateIndividualItems, out int[] itemSizes) {
			return CalculateTotalSize(items.Count(), calculateIndividualItems, out itemSizes);
		}

		public int CalculateTotalSize(int itemsCount, bool calculateIndividualItems, out int[] itemSizes) {
			var val = FixedSize;
			var size = itemsCount * val;
			itemSizes = calculateIndividualItems ? Tools.Array.Gen(itemsCount, val) : null;
			return size;
		}

		public int CalculateSize(TItem item) => FixedSize;
	}

}