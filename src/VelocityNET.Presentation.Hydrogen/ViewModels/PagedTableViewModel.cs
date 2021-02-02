using System;
using System.Collections.Generic;
using System.Linq;

namespace VelocityNET.Presentation.Hydrogen.ViewModels
{

    /// <summary>
    /// View model for paged table component.
    /// </summary>
    /// <typeparam name="TItem"> type of item being displayed</typeparam>
    public class PagedTableViewModel<TItem> : ComponentViewModelBase
    {
        /// <summary>
        /// Gets or sets the items collection
        /// </summary>
        public IEnumerable<TItem> Items { get; set; } = null!;

        /// <summary>
        /// Gets the current page of items being displayed
        /// </summary>
        public IEnumerable<TItem> Page => Items!.Skip((CurrentPage - 1) * PageSize).Take(PageSize)
            .ToList();

        /// <summary>
        /// backing field for page size
        /// </summary>
        private int _pageSize = 10;

        /// <summary>
        /// Gets or sets the size of the page to show. When set, updates the current page
        /// accordingly based on current position.
        /// </summary>
        public int PageSize
        {
            get => _pageSize;
            set
            {
                if (Items is not null)
                {
                    int index = (CurrentPage - 1) * PageSize + Page.Count();
                    _pageSize = value;
                    CurrentPage = (int) Math.Ceiling((double) index / _pageSize);
                }
                else
                {
                    _pageSize = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the current page
        /// </summary>
        public int CurrentPage { get; private set; } = 1;

        /// <summary>
        /// Gets the total number of pages based on total items and page size.
        /// </summary>
        public int TotalPages => (int) Math.Ceiling((double) Items!.Count() / PageSize);

        /// <summary>
        /// Gets a value indicating whether there is a next page.
        /// </summary>
        public bool HasNextPage => CurrentPage < TotalPages;

        /// <summary>
        /// Gets a value indicating whether there is a previous page
        /// </summary>
        public bool HasPrevPage => CurrentPage > 1 && CurrentPage <= TotalPages;

        /// <summary>
        /// Move to next page
        /// </summary>
        /// <exception cref="InvalidOperationException"> thrown if on the last page</exception>
        public void NextPage()
        {
            if (!HasNextPage)
            {
                throw new InvalidOperationException("On last page, no next page");
            }

            CurrentPage++;
        }

        /// <summary>
        /// Move to previous page
        /// </summary>
        /// <exception cref="InvalidOperationException"> thrown if on the first page</exception>
        public void PrevPage()
        {
            if (!HasPrevPage)
            {
                throw new InvalidOperationException("On first page, no previous page");
            }

            CurrentPage--;
        }

        /// <summary>
        /// Move to last page. 
        /// </summary>
        public void LastPage()
        {
            CurrentPage = TotalPages;
        }
    }

}