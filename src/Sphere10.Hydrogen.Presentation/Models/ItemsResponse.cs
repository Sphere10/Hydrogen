using System.Collections.Generic;

namespace Sphere10.Hydrogen.Presentation.Models {
    public record ItemsResponse<TItem>(IEnumerable<TItem> Items, int TotalItems);
}