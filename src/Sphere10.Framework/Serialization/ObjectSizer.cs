using System.Collections.Generic;
using System.Linq;

namespace Sphere10.Framework {

    public abstract class ObjectSizer<TItem> : IObjectSizer<TItem> {

		public bool IsFixedSize => false;

		public int FixedSize => -1;

		public virtual int CalculateTotalSize(IEnumerable<TItem> items, bool calculateIndividualItems, out int[] itemSizes) {
            var sizes = items.Select(CalculateSize).ToArray();
            itemSizes = calculateIndividualItems ? sizes.ToArray() : null;
            return sizes.Sum();
        }

		public abstract int CalculateSize(TItem item);

	}

}