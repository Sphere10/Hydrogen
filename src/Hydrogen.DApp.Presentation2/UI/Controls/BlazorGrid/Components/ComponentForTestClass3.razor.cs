using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Sphere10.Hydrogen.Presentation2.UI.Controls.BlazorGrid.Classes;

namespace Sphere10.Hydrogen.Presentation2.UI.Controls.BlazorGrid.Components {
	public partial class ComponentForTestClass3 : IGridComponent<TestClass3> 
	{
		public ComponentForTestClass3() { }
		[Parameter] public TestClass3 Data { get; set; }

		public void Render(TestClass3 item, RenderTreeBuilder builder) 
		{
			builder.OpenComponent<ComponentForTestClass3>(0);
			builder.AddAttribute(1, "Data", item);
			builder.CloseComponent();
		}
	}
}