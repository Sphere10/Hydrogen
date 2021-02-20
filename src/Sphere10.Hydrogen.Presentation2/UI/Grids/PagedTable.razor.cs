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
        /// backing field for page size
        /// </summary>
        private int _pageSize = 10;

        /// <summary>
        /// Gets or sets the size of the page to show. When set, updates the current page
        /// accordingly based on current position.
        /// </summary>
        [Parameter]
        public int PageSize {
            get => _pageSize;
            set {
                if (Items.Any()) {
                    int index = (CurrentPage - 1) * PageSize + Page.Count();
                    _pageSize = value;
                    CurrentPage = (int)Math.Ceiling((double)index / _pageSize);
                } else {
                    _pageSize = value;
                }
            }
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
        
        /// <summary>
        /// Gets the current page of items being displayed
        /// </summary>
        private IEnumerable<TItem> Page => Items!.Skip((CurrentPage - 1) * PageSize).Take(PageSize)
            .ToList();

        /// <summary>
        /// Gets or sets the current page
        /// </summary>
        private int CurrentPage { get; set; } = 1;

        /// <summary>
        /// Gets the total number of pages based on total items and page size.
        /// </summary>
        private int TotalPages => (int)Math.Ceiling((double)Items!.Count() / PageSize);

        /// <summary>
        /// Gets a value indicating whether there is a next page.
        /// </summary>
        private bool HasNextPage => CurrentPage < TotalPages;

        /// <summary>
        /// Gets a value indicating whether there is a previous page
        /// </summary>
        private bool HasPrevPage => CurrentPage > 1 && CurrentPage <= TotalPages;

        /// <summary>
        /// Move to next page
        /// </summary>
        /// <exception cref="InvalidOperationException"> thrown if on the last page</exception>
        private Task NextPageAsync() {
            if (!HasNextPage) {
                throw new InvalidOperationException("On last page, no next page");
            }

            CurrentPage++;
            return Task.CompletedTask;
        }

        /// <summary>
        /// Move to previous page
        /// </summary>
        /// <exception cref="InvalidOperationException"> thrown if on the first page</exception>
        private Task PrevPageAsync() {
            if (!HasPrevPage) {
                throw new InvalidOperationException("On first page, no previous page");
            }

            CurrentPage--;
            return Task.CompletedTask;
        }

        /// <summary>
        /// Move to last page. 
        /// </summary>
        private void LastPage() {
            CurrentPage = TotalPages;
        }
    }
}