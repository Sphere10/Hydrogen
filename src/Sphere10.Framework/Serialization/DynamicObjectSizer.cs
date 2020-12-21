using System.Collections.Generic;

namespace Sphere10.Framework {

	public abstract class DynamicObjectSizer<TItem> : IObjectSizer<TItem> {

		public bool IsFixedSize => false;

		public int FixedSize => -1;

		public abstract int CalculateTotalSize(IEnumerable<TItem> items, bool calculateIndividualItems, out int[] itemSizes);

		public abstract int CalculateSize(TItem item);

	}

}