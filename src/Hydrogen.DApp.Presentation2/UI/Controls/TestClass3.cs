using System;
using Sphere10.Hydrogen.Presentation2.UI.Controls.BlazorGrid.Classes;
using Sphere10.Hydrogen.Presentation2.UI.Controls.BlazorGrid.Components;

namespace Sphere10.Hydrogen.Presentation2.UI.Controls {
	public class TestClass3 : IColumnDefinition 
	{
		public int HashCode { get; set; }

		public TestClass3() { }

		public TestClass3(string text) 
		{
			HashCode = text.GetHashCode();
		}
		public Type GetComponentType() 
		{
			return typeof(ComponentForTestClass3);
		}
	}
}