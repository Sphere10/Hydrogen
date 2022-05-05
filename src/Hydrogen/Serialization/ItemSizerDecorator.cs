using System.Collections.Generic;

namespace Hydrogen {

	public class ItemSizerDecorator<TItem, TObjectSizer> : IItemSizer<TItem> where TObjectSizer : IItemSizer<TItem> {
		protected readonly TObjectSizer Internal;

		public ItemSizerDecorator(TObjectSizer internalSizer) {
			Internal = internalSizer;
		}

		public virtual bool IsStaticSize => Internal.IsStaticSize;

		public virtual int StaticSize => Internal.StaticSize;

		public virtual int CalculateTotalSize(IEnumerable<TItem> items, bool calculateIndividualItems, out int[] itemSizes) 
			=> Internal.CalculateTotalSize(items, calculateIndividualItems, out itemSizes);

		public virtual int CalculateSize(TItem item) => Internal.CalculateSize(item);
	}

}