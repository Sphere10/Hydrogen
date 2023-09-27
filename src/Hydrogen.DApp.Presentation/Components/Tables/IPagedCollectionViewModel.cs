// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Hamish Rose
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System.Threading.Tasks;

namespace Hydrogen.DApp.Presentation.Components.Tables;

public interface IPagedCollectionViewModel {
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
