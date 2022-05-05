using System.Collections.Generic;

namespace Hydrogen.DApp.Presentation.Models {
    public record ItemsResponse<TItem>(IEnumerable<TItem> Items, int TotalItems);
}