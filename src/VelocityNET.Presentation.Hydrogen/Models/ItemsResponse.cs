using System.Collections.Generic;

namespace VelocityNET.Presentation.Hydrogen.Models {
    public record ItemsResponse<TItem>(IEnumerable<TItem> Items, int TotalItems);
}