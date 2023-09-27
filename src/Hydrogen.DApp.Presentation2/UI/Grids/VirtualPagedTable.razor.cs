using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;

namespace Hydrogen.DApp.Presentation2.UI.Grids {

	public partial class VirtualPagedTable<TItem> {
		/// <summary>
		/// Items provider delegate - used to page of items.
		/// </summary>
		/// <param name="request"> request for items</param>
		public delegate Task<ItemsResponse<TItem>> ItemsProviderDelegate(ItemRequest request);


		/// <summary>
		/// Gets or sets the items provider delegate
		/// </summary>
		[Parameter]
		public ItemsProviderDelegate ItemsProvider { get; set; }

		/// <summary>
		/// Gets or sets the size of the pages
		/// </summary>
		[Parameter]
		public int PageSize { get; set; } = 10;

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
		/// Gets the css class applied to the table element.
		/// </summary>
		[Parameter]
		public string Class { get; set; }

		/// <inheritdoc />
		protected override void OnParametersSet() {
			if (ItemTemplate is null) {
				throw new InvalidOperationException("Item template parameter is required.");
			}

			if (HeaderTemplate is null) {
				throw new InvalidOperationException("Header template parameter is required.");
			}
		}

		/// <summary>
		/// Gets the current page of items being displayed
		/// </summary>
		private IEnumerable<TItem> Page { get; set; } = new List<TItem>();

		/// <summary>
		/// Gets the total items
		/// </summary>
		private int TotalItems { get; set; }

		/// <summary>
		/// Gets or sets the current page
		/// </summary>
		private int CurrentPage { get; set; } = 1;

		/// <summary>
		/// Gets the total number of pages based on total items and page size.
		/// </summary>
		private int TotalPages => (int)Math.Ceiling((double)TotalItems / PageSize);

		/// <summary>
		/// Gets a value indicating whether there is a next page.
		/// </summary>
		private bool HasNextPage => CurrentPage < TotalPages;

		/// <summary>
		/// Gets a value indicating whether there is a previous page
		/// </summary>
		private bool HasPrevPage => CurrentPage > 1 && CurrentPage <= TotalPages;

		/// <summary>
		/// Move to next page, retrieving it from the data provider
		/// </summary>
		/// <exception cref="InvalidOperationException"> thrown if on the last page</exception>
		private async Task NextPageAsync() {
			if (!HasNextPage) {
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
		private async Task PrevPageAsync() {
			if (!HasPrevPage) {
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
		public async Task LastPageAsync() {
			CurrentPage = TotalPages;

			(IEnumerable<TItem>? items, int totalItems) = await ItemsProvider(new ItemRequest(
				(CurrentPage - 1) * PageSize - 1,
				PageSize,
				string.Empty,
				string.Empty));

			TotalItems = totalItems;
			Page = items;
		}

		/// <summary>
		/// Set the page size shown in the table.
		/// </summary>
		/// <param name="pageSize"> new page size</param>
		/// <returns> task</returns>
		public async Task SetPageSizeAsync(int pageSize) {
			int index = (CurrentPage - 1) * PageSize + Page.Count();
			PageSize = pageSize;
			CurrentPage = (int)Math.Ceiling((double)index / PageSize);

			(IEnumerable<TItem>? items, int totalItems) = await ItemsProvider(new ItemRequest(
				(CurrentPage - 1) * PageSize - 1,
				PageSize,
				string.Empty,
				string.Empty));

			TotalItems = totalItems;
			Page = items;
		}

		/// <inheritdoc />
		protected override async Task OnParametersSetAsync() {
			(IEnumerable<TItem>? items, int totalItems) = await ItemsProvider.Invoke(new ItemRequest(0,
				PageSize,
				string.Empty,
				string.Empty));

			Page = items;
			TotalItems = totalItems;

			await base.OnParametersSetAsync();
		}
	}

}
