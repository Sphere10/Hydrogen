using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;

namespace Sphere10.Hydrogen.Presentation2.UI.Grids {
    /// <summary>
    /// Paging table - simple table with pagination 
    /// </summary>
    /// <typeparam name="TItem"> type of item being displayed</typeparam>
    public partial class PagedTable<TItem>  {
       
        /// <summary>
        /// Gets or sets the items being displayed in the table
        /// </summary>
        [Parameter]
        public IEnumerable<TItem> Items { get; set; }

        /// <summary>
        /// Gets or sets the item template
        /// </summary>
        [Parameter]
        public RenderFragment<TItem> ItemTemplate { get; set; } = null!;

        /// <summary>
        /// Gets or sets the header template
        /// </summary>
        [Parameter]
        public RenderFragment HeaderTemplate { get; set; } = null!;

        /// <summary>
        /// Gets or sets the callback to call when row is clicked
        /// </summary>
        [Parameter] 
        public EventCallback<TItem> OnRowSelect { get; set; } = EventCallback<TItem>.Empty;

        /// <summary>
        /// Gets or sets the CSS class string applied to the table element.
        /// </summary>
        [Parameter]
        public string Class { get; set; }

        /// <inheritdoc />
        protected override void OnParametersSet() {
            if (Items is null) {
                throw new InvalidOperationException("Items parameter is required.");
            }

            if (HeaderTemplate is null) {
                throw new InvalidOperationException("Header template parameter is required.");
            }

            if (ItemTemplate is null) {
                throw new InvalidOperationException("Item template parameter is required.");
            }
        }
        
    }
}