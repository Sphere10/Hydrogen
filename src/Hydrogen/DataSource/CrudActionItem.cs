namespace Hydrogen {
	public class CrudActionItem<TItem> {
		public CrudAction CrudAction { get; init; }
		public TItem Item { get; set; }

		public CrudActionItem(CrudAction crudAction, TItem item) {	
			CrudAction = crudAction;
			Item = item;
		}
	}
}