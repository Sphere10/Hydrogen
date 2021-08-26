using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Sphere10.Hydrogen.Presentation2.UI.Controls.BlazorGrid.Classes;

namespace Sphere10.Hydrogen.Presentation2.UI.Controls.BlazorGrid.Components {
	partial class Component2Test : IGridComponent 
	{
		[Parameter] public bool Checked { get; set; }

		public Component2Test() {}

		public Component2Test(bool checkedValue) 
		{
			Checked = checkedValue;
		}

		public void Render(RenderTreeBuilder builder) 
		{
			builder.OpenComponent<Component2Test>(0);
			builder.AddAttribute(1, "Checked", Checked);
			builder.CloseComponent();
		}
	}
}