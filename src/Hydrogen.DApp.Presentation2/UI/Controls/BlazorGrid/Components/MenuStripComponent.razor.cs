using Hydrogen.DApp.Presentation2.UI.Controls.BlazorGrid.Classes;

namespace Hydrogen.DApp.Presentation2.UI.Controls.BlazorGrid.Components {
	partial class MenuStripComponent {
		MenuStrip Menu { get; set; } = new MenuStrip();
		public int Top { get; set; }
		public int Left { get; set; }

		public void SetMenuStrip(MenuStrip menu) {
			Menu = menu;
			StateHasChanged();
		}

		public MenuStripComponent() { }

		void ContextMenuClick(MenuStripItem item) {
			if (item.IsOnClickSet()) {
				item.CallOnClick();
				CloseMenu();
			}
		}

		string GetContextMenuContentStyle() {
			return $"width:200px; height:300px; left:{Left}px;top:{Top}px;";
		}

		void CloseMenu() {
			Left = 0;
			Top = 0;
			Menu.Items.Clear();
			StateHasChanged();
		}
	}
}
