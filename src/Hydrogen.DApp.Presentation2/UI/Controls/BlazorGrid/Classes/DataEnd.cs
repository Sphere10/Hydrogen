using System;
using Microsoft.AspNetCore.Components.Rendering;

namespace Hydrogen.DApp.Presentation2.UI.Controls.BlazorGrid.Classes {
	public class DataEnd<TInItem, TOutItem>
	{
		public string Name { get; set; }
		public Func<object, object> DataExtractor { get; set; }

		public DataEnd() { }

		public DataEnd(string name, Func<object, object> dataExtractor)
		{
			Name = name;
			DataExtractor = dataExtractor;
		}

		public void Render(object item, RenderTreeBuilder builder)
		{
			var extractedData = (IColumnDefinition)DataExtractor(item);
			var componentType = extractedData.GetComponentType();
			dynamic component = Activator.CreateInstance(componentType);
			component.Render((TOutItem)extractedData, builder);
		}
	}
}