using System.Collections.Generic;

namespace Hydrogen.DApp.Presentation2.UI.Grids {
    public record ItemsResponse<TItem>(IEnumerable<TItem> Items, int TotalItems);
}