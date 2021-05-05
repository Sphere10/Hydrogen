using System.Collections.Generic;

namespace Sphere10.Framework {

	public class ItemSizerDecorator<TItem, TObjectSizer> : IItemSizer<TItem> where TObjectSizer : IItemSizer<TItem> {
		protected readonly TObjectSizer Internal;

		public ItemSizerDecorator(TObjectSizer internalSizer) {
			Internal = internalSizer;
		}

		public virtual bool IsFixedSize => Internal.IsFixedSize;

		public virtual int FixedSize => Internal.FixedSize;

		public virtual int CalculateTotalSize(IEnumerable<TItem> items, bool calculateIndividualItems, out int[] itemSizes) 
			=> Internal.CalculateTotalSize(items, calculateIndividualItems, out itemSizes);

		public virtual int CalculateSize(TItem item) => Internal.CalculateSize(item);
	}

}