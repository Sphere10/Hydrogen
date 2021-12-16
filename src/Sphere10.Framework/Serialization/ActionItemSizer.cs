using System;
using System.Collections.Generic;
using System.Linq;

namespace Sphere10.Framework {
	public class ActionItemSizer<T> : IItemSizer<T> {
		private readonly Func<T, int> _sizer;

		public ActionItemSizer(Func<T, int> sizer) {
			Guard.ArgumentNotNull(sizer, nameof(sizer));
			_sizer = sizer;
		}

		public bool IsStaticSize => false;

		public int StaticSize => -1;

		public int CalculateTotalSize(IEnumerable<T> items, bool calculateIndividualItems, out int[] itemSizes) {
			var sizes = items.Select(CalculateSize).ToArray();
			itemSizes = calculateIndividualItems ? sizes : null;
			return sizes.Sum();
		}

		public int CalculateSize(T item) => _sizer(item);
	}

}