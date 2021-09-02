using System;
using Microsoft.AspNetCore.Components.Rendering;

namespace Sphere10.Hydrogen.Presentation2.UI.Controls.BlazorGrid.Classes {
	public interface IDataEnd 
	{
		public string Name { get; set; }
		public Func<object, object> DataExtractor { get; set; }
		public void Render(object item, RenderTreeBuilder builder);
	}
}