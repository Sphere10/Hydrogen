using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using VelocityNET.Presentation.Hydrogen.Models;

namespace VelocityNET.Presentation.Hydrogen.Components.Tables
{
    public class VirtualPagedTable<TItem> : ComponentWithViewModel<VirtualPagedTableViewModel<TItem>>
    {
        /// <summary>
        /// Items provider delegate - used to page of items.
        /// </summary>
        /// <param name="request"> request for items</param>
        public delegate Task<ItemsResponse<TItem>> ItemsProviderDelegate(ItemRequest request);
        
        private string PreviousItemClass => ViewModel!.HasPrevPage ? string.Empty : "disabled";

        private string NextItemClass => ViewModel!.HasNextPage ? string.Empty : "disabled";
        
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

        protected override void BuildRenderTree(RenderTreeBuilder builder)
        {
            builder.OpenElement(0, "table");
            builder.AddAttribute(2, "class", Class);

            builder.OpenElement(3, "thead");
            HeaderTemplate(builder);
            builder.CloseElement();

            builder.OpenElement(4, "tbody");

            foreach (TItem item in ViewModel!.Page)
            {
                builder.OpenElement(5, "span");
                builder.AddAttribute(6, "style", "display: contents");
                builder.AddAttribute(7, "onclick", EventCallback.Factory.Create(this, () => OnRowSelect.InvokeAsync(item)));
                
                ItemTemplate(item)(builder);
                
                builder.CloseElement();
            }
            
            builder.CloseElement();
            
            builder.OpenElement(8, "nav");
            builder.OpenElement(9, "ul");
            builder.AddAttribute(10, "class", "pagination");
            
            builder.OpenElement(12, "li");
            builder.AddAttribute(12, "class", $"page-item {PreviousItemClass}");
            builder.OpenElement(13, "a");
            builder.AddAttribute(13, "class", "page-link");
            builder.AddAttribute(13, "onclick", EventCallback.Factory.Create(this, () => ViewModel!.PrevPageAsync()));
            builder.AddContent(13, "Previous");
            builder.CloseElement();
            builder.CloseElement();

            if (ViewModel!.HasPrevPage)
            {
                builder.OpenElement(14, "li");
                builder.AddAttribute(14, "class", $"page-item");
                builder.OpenElement(14, "a");
                builder.AddAttribute(14, "class", "page-link");
                builder.AddAttribute(14, "onclick", EventCallback.Factory.Create(this, () => ViewModel!.PrevPageAsync()));
                builder.AddContent(15, ViewModel!.CurrentPage - 1);
                builder.CloseElement();
                builder.CloseElement();
            }
            
            builder.OpenElement(16, "li");
            builder.AddAttribute(16, "class", $"page-item active");
            builder.OpenElement(16, "a");
            builder.AddAttribute(16, "class", "page-link");
            builder.AddContent(16, ViewModel!.CurrentPage);
            builder.CloseElement();
            builder.CloseElement();
            
            if (ViewModel!.HasNextPage)
            {
                builder.OpenElement(18, "li");
                builder.AddAttribute(18, "class", $"page-item");
                builder.OpenElement(18, "a");
                builder.AddAttribute(18, "class", "page-link");
                builder.AddAttribute(18, "onclick", EventCallback.Factory.Create(this, () => ViewModel!.NextPageAsync()));
                builder.AddContent(18, ViewModel!.CurrentPage + 1);
                builder.CloseElement();
                builder.CloseElement();
            }
            
            builder.OpenElement(20, "li");
            builder.AddAttribute(21, "class", $"page-item {NextItemClass}");
            builder.OpenElement(22, "a");
            builder.AddAttribute(22, "class", "page-link");
            builder.AddAttribute(22, "onclick", EventCallback.Factory.Create(this, () => ViewModel!.NextPageAsync()));
            builder.AddContent(22, "Next");
            builder.CloseElement();
            builder.CloseElement();
            builder.CloseElement();

            builder.OpenComponent<PageSelector>(23);
            builder.AddAttribute(23, "Model", ViewModel);
            builder.AddAttribute(23, "Value", ViewModel!.PageSize);
            builder.AddAttribute(23, "ValueExpression", (Expression<Func<int>>)(() => ViewModel!.PageSize));
            builder.AddAttribute(23, "ValueChanged",
                EventCallback.Factory.Create<int>(this, x => ViewModel!.SetPageSizeAsync(x)));
            
            builder.CloseElement();
            builder.CloseElement();
                
            builder.CloseElement();
        }
    }
}