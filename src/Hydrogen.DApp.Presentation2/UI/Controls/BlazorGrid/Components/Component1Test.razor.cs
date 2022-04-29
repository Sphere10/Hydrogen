using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Sphere10.Hydrogen.Presentation2.UI.Controls.BlazorGrid.Classes;

namespace Sphere10.Hydrogen.Presentation2.UI.Controls.BlazorGrid.Components {
	partial class Component1Test : IGridComponent<string>
	{
		[Parameter] public string ListId { get; set; }

		public Component1Test() {}

		public Component1Test(string listId) 
		{
			ListId = listId;
		}
		
		public void Render(string item, RenderTreeBuilder builder) 
		{
			builder.OpenComponent<Component1Test>(0);
			builder.AddAttribute(1, "ListId", ListId);
			builder.CloseComponent();
		}
	}
}