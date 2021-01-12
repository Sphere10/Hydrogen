using System;
using System.Collections.Generic;
using System.Linq;

namespace VelocityNET.Presentation.Hydrogen.ViewModels
{

    public class PagedTableViewModel<TItem> : ComponentViewModelBase
    {
        public IEnumerable<TItem> Items { get; set; }

        public IEnumerable<TItem> Page => Items.Skip((CurrentPage - 1) * PageSize).Take(PageSize)
            .ToList();

        public int PageSize { get; set; } = 25;

        public int CurrentPage { get; set; } = 1;

        // ReSharper disable once PossibleLossOfFraction -- intentional
        public int TotalPages => (int) Math.Ceiling((decimal) Items.Count() / PageSize);

        public bool HasNextPage => CurrentPage < TotalPages;

        public bool HasPrevPage => CurrentPage > 1 && CurrentPage <= TotalPages;

        public void NextPage()
        {
            if (!HasNextPage)
            {
                throw new InvalidOperationException("On last page, no next page");
            }

            CurrentPage++;
        }

        public void PrevPage()
        {
            if (!HasPrevPage)
            {
                throw new InvalidOperationException("On first page, no previous page");
            }

            CurrentPage--;
        }
    }

}