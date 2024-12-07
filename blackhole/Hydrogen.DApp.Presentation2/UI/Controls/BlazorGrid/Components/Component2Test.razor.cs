using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Hydrogen.DApp.Presentation2.UI.Controls.BlazorGrid.Classes;

namespace Hydrogen.DApp.Presentation2.UI.Controls.BlazorGrid.Components {
	partial class Component2Test : IGridComponent<bool> {
		[Parameter] public bool Checked { get; set; }

		public Component2Test() {
		}

		public Component2Test(bool checkedValue) {
			Checked = checkedValue;
		}

		public void Render(bool item, RenderTreeBuilder builder) {
			builder.OpenComponent<Component2Test>(0);
			builder.AddAttribute(1, "Checked", Checked);
			builder.CloseComponent();
		}
	}
}
