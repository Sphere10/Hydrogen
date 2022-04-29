using System;
using Hydrogen.DApp.Presentation2.UI.Controls.BlazorGrid.Classes;
using Hydrogen.DApp.Presentation2.UI.Controls.BlazorGrid.Components;

namespace Hydrogen.DApp.Presentation2.UI.Controls {
	public class TestClass2 : IColumnDefinition
	{
		public string Text { get; set; }
		public decimal Value { get; set; }
		public bool Locked { get; set; }

		public TestClass2() { }

		public TestClass2(string text, decimal value, bool locked) 
		{
			Text = text;
			Value = value;
			Locked = locked;
		}

		public Type GetComponentType() 
		{
			return typeof(ComponentForTestClass2);
		}
	}
}