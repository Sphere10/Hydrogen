namespace Hydrogen.DApp.Presentation2.UI.Controls.BlazorGrid.Classes {
	public class MenuStripItem {

		public event EventHandlerEx<object> OnClick;
		public string Text { get; set; }
		public object Item { get; set; }

		public MenuStripItem(object item, string text) {
			Item = item;
			Text = text;
		}

		public bool IsOnClickSet() {
			return OnClick != null;
		}

		public void CallOnClick() {

			if (!IsOnClickSet()) return;

			OnClick.Invoke(Item);
		}
	}
}