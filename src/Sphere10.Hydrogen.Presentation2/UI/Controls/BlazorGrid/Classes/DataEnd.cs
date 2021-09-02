using System;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace Sphere10.Hydrogen.Presentation2.UI.Controls.BlazorGrid.Classes {
	public class DataEnd<TInItem, TOutItem> : IDataEnd
	{
		public string Name { get; set; }
		public Func<object, object> DataExtractor { get; set; }

		public DataEnd() { }

		public DataEnd(string name, Func<object, object> dataExtractor)
		{
			Name = name;
			DataExtractor = dataExtractor;
		}

//		public RenderFragment Render(RenderTreeBuilder builder)
		public void Render(object item, RenderTreeBuilder builder)
		{
//			var component = Activator.CreateInstance(GridComponent);

//			dynamic data = DataExtractor(item);

//			component.Render(Convert.ChangeType(data, GridComponent), builder);

			dynamic component = (IGridComponent<TOutItem>)Activator.CreateInstance(typeof(TOutItem));

			var data = (TOutItem)DataExtractor(item);

			//var genericType = GridComponent.MakeGenericType(data.GetType());

			component.Render(data, builder);
		}
	}
}