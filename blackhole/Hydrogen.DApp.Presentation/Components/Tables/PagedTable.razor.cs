// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Hamish Rose
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Hydrogen.DApp.Presentation.ViewModels;

namespace Hydrogen.DApp.Presentation.Components.Tables;

/// <summary>
/// Paging table - simple table with pagination 
/// </summary>
/// <typeparam name="TItem"> type of item being displayed</typeparam>
public class PagedTable<TItem> : ComponentWithViewModel<PagedTableViewModel<TItem>> {
	/// <summary>
	/// Gets or sets the items being displayed in the table
	/// </summary>
	[Parameter]
	public IEnumerable<TItem> Items {
		get => ViewModel!.Items;
		set => ViewModel!.Items = value;
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
	/// Gets or sets the CSS class string applied to the table element.
	/// </summary>
	[Parameter]
	public string? Class { get; set; }

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

	protected override void BuildRenderTree(RenderTreeBuilder builder) {
		builder.OpenElement(0, "table");
		builder.AddAttribute(2, "class", Class);

		builder.OpenElement(3, "thead");
		HeaderTemplate(builder);
		builder.CloseElement();

		builder.OpenElement(4, "tbody");

		foreach (TItem item in ViewModel!.Page) {
			builder.OpenElement(5, "span");
			builder.AddAttribute(5, "style", "display: contents");
			builder.AddAttribute(5,
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
			EventCallback.Factory.Create<int>(this, x => ViewModel!.PageSize = x));
		builder.CloseComponent();
	}
}
