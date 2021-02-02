using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using VelocityNET.Presentation.Hydrogen.Models;

namespace VelocityNET.Presentation.Hydrogen.Components
{
    public partial class VirtualPagedTableTemplate<TItem>
    {
        /// <summary>
        /// Items provider delegate - used to page of items.
        /// </summary>
        /// <param name="request"> request for items</param>
        public delegate Task<ItemsResponse<TItem>> ItemsProviderDelegate(ItemRequest request);
        
        /// <summary>
        /// Gets or sets the items provider delegate
        /// </summary>
        [Parameter]
        public ItemsProviderDelegate ItemsProvider
        {
            get => ViewModel!.ItemsProvider;
            set => ViewModel!.ItemsProvider = value;
        }
        
        /// <summary>
        /// Gets or sets the size of the pages
        /// </summary>
        [Parameter]
        public int PageSize
        {
            get => ViewModel!.PageSize;
            set => ViewModel!.PageSize = value;
        }

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
        [Parameter] public EventCallback<TItem> OnRowSelect { get; set; } = EventCallback<TItem>.Empty;
        
        /// <summary>
        /// Gets the css class applied to the table element.
        /// </summary>
        [Parameter]
        public string? Class { get; set; }
        
        /// <inheritdoc />
        protected override void OnParametersSet()
        {
            if (ItemTemplate is null)
            {
                throw new InvalidOperationException("Item template parameter is required.");
            }
        
            if (HeaderTemplate is null)
            {
                throw new InvalidOperationException("Header template parameter is required.");
            }
        }
    }
}