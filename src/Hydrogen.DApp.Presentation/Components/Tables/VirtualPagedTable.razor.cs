// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Hamish Rose
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Hydrogen.DApp.Presentation.Models;

namespace Hydrogen.DApp.Presentation.Components.Tables;

public class VirtualPagedTable<TItem> : ComponentWithViewModel<VirtualPagedTableViewModel<TItem>> {
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
	public ItemsProviderDelegate ItemsProvider {
		get => ViewModel!.ItemsProvider;
		set => ViewModel!.ItemsProvider = value;
	}

	/// <summary>
	/// Gets or sets the size of the pages
	/// </summary>
	[Parameter]
	public int PageSize {
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
	[Parameter]
	public EventCallback<TItem> OnRowSelect { get; set; } = EventCallback<TItem>.Empty;

	/// <summary>
	/// Gets the css class applied to the table element.
	/// </summary>
	[Parameter]
	public string? Class { get; set; }

	/// <inheritdoc />
	protected override void OnParametersSet() {
		if (ItemTemplate is null) {
			throw new InvalidOperationException("Item template parameter is required.");
		}

		if (HeaderTemplate is null) {
			throw new InvalidOperationException("Header template parameter is required.");
		}
	}

	protected override void BuildRenderTree(RenderTreeBuilder builder) {
		builder.OpenElement(0, "table");
		builder.AddAttribute(2, "class", Class);

		builder.OpenElement(3, "thead");
		HeaderTemplate(builder);
		builder.CloseElement();

		builder.OpenElement(4, "tbody");

		foreach (TItem item in ViewModel!.Page) {
			builder.OpenElement(5, "span");
			builder.AddAttribute(6, "style", "display: contents");
			builder.AddAttribute(7,
				"onclick",
				EventCallback.Factory.Create(this, () => OnRowSelect.InvokeAsync(item)));

			ItemTemplate(item)(builder);

			builder.CloseElement();
		}

		builder.CloseElement();

		builder.OpenComponent<Pagination>(21);
		builder.AddAttribute(21, nameof(Pagination.Model), ViewModel);
		builder.CloseComponent();

		builder.CloseElement();

		builder.OpenComponent<PageSizeSelector>(23);
		builder.AddAttribute(23, nameof(PageSizeSelector.Model), ViewModel);
		builder.AddAttribute(23, nameof(PageSizeSelector.Value), ViewModel!.PageSize);
		builder.AddAttribute(23,
			nameof(PageSizeSelector.ValueExpression),
			(Expression<Func<int>>)(() => ViewModel!.PageSize));
		builder.AddAttribute(23,
			nameof(PageSizeSelector.ValueChanged),
			EventCallback.Factory.Create<int>(this, x => ViewModel!.SetPageSizeAsync(x)));
		builder.CloseComponent();

	}
}
