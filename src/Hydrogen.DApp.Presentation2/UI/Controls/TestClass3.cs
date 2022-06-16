using System;
using Hydrogen.DApp.Presentation2.UI.Controls.BlazorGrid.Classes;
using Hydrogen.DApp.Presentation2.UI.Controls.BlazorGrid.Components;

namespace Hydrogen.DApp.Presentation2.UI.Controls {
	public class TestClass3 : IColumnDefinition 
	{
		public int HashCode { get; set; }
		public int Width { get; init; }

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