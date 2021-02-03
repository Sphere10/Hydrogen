using System.Threading.Tasks;

namespace VelocityNET.Presentation.Hydrogen.Components.Tables
{

    public interface IPagedCollectionViewModel
    {
        /// <summary>
        /// Gets or sets the size of the page to show.
        /// </summary>
        int PageSize { get; }

        /// <summary>
        /// Gets a value indicating whether there is a next page.
        /// </summary>
        bool HasNextPage { get; }
        
        /// <summary>
        /// Gets a value indicating whether there is a previous page
        /// </summary>
        public bool HasPrevPage { get; }
        
        /// <summary>
        /// Gets the total number of pages based on total items and page size.
        /// </summary>
        int TotalPages { get; }
        
        /// <summary>
        /// Gets the current page
        /// </summary>
        int CurrentPage { get; }

        /// <summary>
        /// Next page
        /// </summary>
        /// <returns></returns>
        Task NextPageAsync();

        /// <summary>
        /// Previous page
        /// </summary>
        /// <returns></returns>
        Task PrevPageAsync();
    }

}