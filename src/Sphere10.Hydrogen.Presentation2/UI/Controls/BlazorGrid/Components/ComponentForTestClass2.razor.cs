using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Sphere10.Hydrogen.Presentation2.UI.Controls.BlazorGrid.Classes;

namespace Sphere10.Hydrogen.Presentation2.UI.Controls.BlazorGrid.Components {
	public partial class ComponentForTestClass2 : IGridComponent<TestClass2>
	{
		public ComponentForTestClass2() { }
		[Parameter] public TestClass2 Data { get; set; }

		public void Render(TestClass2 item, RenderTreeBuilder builder) 
		{
			builder.OpenComponent<ComponentForTestClass2>(0);
			builder.AddAttribute(1, "Data", item);
			builder.CloseComponent();
		}
	}
}