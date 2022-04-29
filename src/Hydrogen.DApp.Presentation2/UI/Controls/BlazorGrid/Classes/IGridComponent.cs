using Microsoft.AspNetCore.Components.Rendering;

namespace Sphere10.Hydrogen.Presentation2.UI.Controls.BlazorGrid.Classes {
	public interface IGridComponent<TItem>
	{
		public void Render(TItem item, RenderTreeBuilder builder);
	}
}