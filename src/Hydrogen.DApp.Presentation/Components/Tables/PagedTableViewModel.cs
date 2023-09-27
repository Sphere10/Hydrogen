// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Hamish Rose
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hydrogen.DApp.Presentation.ViewModels;

namespace Hydrogen.DApp.Presentation.Components.Tables;

/// <summary>
/// View model for paged table component.
/// </summary>
/// <typeparam name="TItem"> type of item being displayed</typeparam>
public class PagedTableViewModel<TItem> : ComponentViewModelBase, IPagedCollectionViewModel {
	/// <summary>
	/// Gets or sets the items collection
	/// </summary>
	public IEnumerable<TItem> Items { get; set; } = new List<TItem>();

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
	/// Current page
	/// </summary>
	private int _currentPage = 1;

	/// <summary>
	/// Gets or sets the current page
	/// </summary>
	public int CurrentPage {
		get => _currentPage;
		set {
			_currentPage = value;
			StateHasChangedDelegate?.Invoke();
		}
	}

	/// <summary>
	/// Gets the total number of pages based on total items and page size.
	/// </summary>
	public int TotalPages => (int)Math.Ceiling((double)Items!.Count() / PageSize);

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
	public Task NextPageAsync() {
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
	public Task PrevPageAsync() {
		if (!HasPrevPage) {
			throw new InvalidOperationException("On first page, no previous page");
		}

		CurrentPage--;
		return Task.CompletedTask;
	}

	/// <summary>
	/// Move to last page. 
	/// </summary>
	public void LastPage() {
		CurrentPage = TotalPages;
	}
}
