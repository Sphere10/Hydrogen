using System.Collections.Generic;
using System.Linq;

namespace Hydrogen {

	public abstract class ItemSizer<TItem> : IItemSizer<TItem> {

		public bool IsStaticSize => false;

		public int StaticSize => -1;

		public virtual int CalculateTotalSize(IEnumerable<TItem> items, bool calculateIndividualItems, out int[] itemSizes) {
			var sizes = items.Select(CalculateSize).ToArray();
			itemSizes = calculateIndividualItems ? sizes.ToArray() : null;
			return sizes.Sum();
		}

		public abstract int CalculateSize(TItem item);

	}

}