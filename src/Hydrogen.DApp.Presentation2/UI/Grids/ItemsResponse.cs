using System.Collections.Generic;

namespace Sphere10.Hydrogen.Presentation2.UI.Grids {
    public record ItemsResponse<TItem>(IEnumerable<TItem> Items, int TotalItems);
}