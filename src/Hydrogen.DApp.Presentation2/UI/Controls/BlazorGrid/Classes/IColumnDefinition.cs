using System;

namespace Hydrogen.DApp.Presentation2.UI.Controls.BlazorGrid.Classes {
	public interface IColumnDefinition 
	{
		public static readonly int DefaultWidth = 100;

		public Type GetComponentType();
		public int Width { get; init; }
	}
}