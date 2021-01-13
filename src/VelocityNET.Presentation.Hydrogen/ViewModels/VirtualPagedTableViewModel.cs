using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VelocityNET.Presentation.Hydrogen.Components;
using VelocityNET.Presentation.Hydrogen.Models;

namespace VelocityNET.Presentation.Hydrogen.ViewModels
{
    public class VirtualPagedTableViewModel<TItem> : ComponentViewModelBase
    {
        public VirtualPagedTableTemplate<TItem>.ItemsProviderDelegate ItemsProvider { get; set; } = null!;

        public IEnumerable<TItem> Page { get; private set; } = new List<TItem>();

        public int PageSize { get; set; } = 25;

        public int TotalItems { get; private set; }

        public int CurrentPage { get; set; } = 1;

        public int TotalPages => (int) Math.Ceiling((double) TotalItems / PageSize);

        public bool HasNextPage => CurrentPage < TotalPages;

        public bool HasPrevPage => CurrentPage > 1 && CurrentPage <= TotalPages;

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

        public override async Task InitAsync()
        {
            (IEnumerable<TItem>? items, int totalItems) = await ItemsProvider(new ItemRequest(0,
                PageSize,
                string.Empty,
                string.Empty));

            Page = items;
            TotalItems = totalItems;

            await base.InitAsync();
        }
    }
}