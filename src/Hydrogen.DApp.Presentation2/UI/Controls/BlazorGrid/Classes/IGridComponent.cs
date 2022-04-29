using Microsoft.AspNetCore.Components.Rendering;

namespace Hydrogen.DApp.Presentation2.UI.Controls.BlazorGrid.Classes {
	public interface IGridComponent<TItem>
	{
		public void Render(TItem item, RenderTreeBuilder builder);
	}
}