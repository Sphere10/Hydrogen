using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VelocityNET.Presentation.Hydrogen.Models;
using VelocityNET.Presentation.Hydrogen.ViewModels;

namespace VelocityNET.Presentation.Hydrogen.Components
{
/// <summary>
/// Virtual paged table view model
/// </summary>
/// <typeparam name="TItem"> item type</typeparam>
    public class VirtualPagedTableViewModel<TItem> : ComponentViewModelBase
    {
        /// <summary>
        /// Gets or sets the item provider delegate
        /// </summary>
        public VirtualPagedTableTemplate<TItem>.ItemsProviderDelegate ItemsProvider { get; set; } = null!;

        /// <summary>
        /// Gets the current page of items being displayed
        /// </summary>
        public IEnumerable<TItem> Page { get; private set; } = new List<TItem>();
        
        /// <summary>
        /// Gets or sets the size of the page to show.
        /// </summary>
        public int PageSize { get; set; } = 10;

        /// <summary>
        /// Gets the total items
        /// </summary>
        public int TotalItems { get; private set; }

        /// <summary>
        /// Gets the current page
        /// </summary>
        public int CurrentPage { get; private set; } = 1;

        /// <summary>
        /// Gets the total number of pages based on total items and page size.
        /// </summary>
        public int TotalPages => (int) Math.Ceiling((double) TotalItems / PageSize);

        /// <summary>
        /// Gets a value indicating whether there is a next page.
        /// </summary>
        public bool HasNextPage => CurrentPage < TotalPages;

        /// <summary>
        /// Gets a value indicating whether there is a previous page
        /// </summary>
        public bool HasPrevPage => CurrentPage > 1 && CurrentPage <= TotalPages;

        /// <summary>
        /// Move to next page, retrieving it from the data provider
        /// </summary>
        /// <exception cref="InvalidOperationException"> thrown if on the last page</exception>
        public async Task NextPageAsync()
        {
            if (!HasNextPage)
            {
                throw new InvalidOperationException("On last page, no next page");
            }

            CurrentPage++;

            (IEnumerable<TItem>? items, int totalItems) = await ItemsProvider(new ItemRequest(
                (CurrentPage - 1) * PageSize - 1,
                PageSize,
                string.Empty,
                string.Empty));

            TotalItems = totalItems;
            Page = items;
        }

        /// <summary>
        /// Move to previous page, retrieving it from the data provider
        /// </summary>
        /// <exception cref="InvalidOperationException"> thrown if on the first page</exception>
        public async Task PrevPageAsync()
        {
            if (!HasPrevPage)
            {
                throw new InvalidOperationException("On first page, no previous page");
            }

            CurrentPage--;

            (IEnumerable<TItem>? items, int totalItems) = await ItemsProvider(new ItemRequest(
                (CurrentPage - 1) * PageSize - 1,
                PageSize,
                string.Empty,
                string.Empty));

            TotalItems = totalItems;
            Page = items;
        }

        /// <summary>
        /// Move to last page, retrieving it from the data provider
        /// </summary>
        public async Task LastPageAsync()
        {
            CurrentPage = TotalPages;

            (IEnumerable<TItem>? items, int totalItems) = await ItemsProvider(new ItemRequest(
                (CurrentPage - 1) * PageSize - 1,
                PageSize,
                string.Empty,
                string.Empty));

            TotalItems = totalItems;
            Page = items;
        }

        public async Task SetPageSizeAsync(int pageSize)
        {
            int index = (CurrentPage - 1) * PageSize + Page.Count();
            PageSize = pageSize;
            CurrentPage = (int) Math.Ceiling((double) index / PageSize);

            (IEnumerable<TItem>? items, int totalItems) = await ItemsProvider(new ItemRequest(
                (CurrentPage - 1) * PageSize - 1,
                PageSize,
                string.Empty,
                string.Empty));

            TotalItems = totalItems;
            Page = items;
        }

        public override async Task InitAsync()
        {
            (IEnumerable<TItem>? items, int totalItems) = await ItemsProvider.Invoke(new ItemRequest(0,
                PageSize,
                string.Empty,
                string.Empty));

            Page = items;
            TotalItems = totalItems;

            await base.InitAsync();
        }
    }

}