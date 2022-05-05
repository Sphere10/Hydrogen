using System.Collections.Generic;

namespace Sphere10.Framework {
	public class DataSourceMutatedItems<TItem> {
		public IList<CrudActionItem<TItem>> UpdatedItems { get; set; } = new List<CrudActionItem<TItem>>();
		public int CurrentPage { get; set; }
		public int TotalItems { get; set; }
	}
}