// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Hamish Rose
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Collections.Generic;
using System.Threading;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace Hydrogen.DApp.Presentation.Components.Tables;

/// <summary>
/// Rapid table control / component. async enumerates an async enumerable e.g. a stream
/// or channel until cancelled or disposed. Generates a table with thead, and tbody from templates
/// </summary>
public class RapidTable<TItem> : ComponentWithViewModel<RapidTableViewModel<TItem>> {
	/// <summary>
	/// Gets or sets the async item source that will be enumerated.
	/// </summary>
	[Parameter]
	public IAsyncEnumerable<TItem>? Source {
		get => ViewModel!.Source;
		set => ViewModel!.Source = value!;
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
	/// Gets or sets the number of items to be displayed in the table.
	/// </summary>
	[Parameter]
	public int ItemLimit {
		get => ViewModel!.ItemLimit;
		set => ViewModel!.ItemLimit = value;
	}

	/// <summary>
	/// Gets or sets the cancellation token used to cancel the async enumeration of the source property.
	/// </summary>
	[Parameter]
	public CancellationToken CancellationToken {
		get => ViewModel!.CancellationToken;
		set => ViewModel!.CancellationToken = value;
	}

	/// <summary>
	/// Gets or sets the callback to call when row is clicked
	/// </summary>
	[Parameter]
	public EventCallback<TItem> OnRowSelect { get; set; } = EventCallback<TItem>.Empty;

	/// <summary>
	/// Gets or sets the css class applied to the table element
	/// </summary>
	[Parameter]
	public string? Class { get; set; }

	/// <inheritdoc />
	protected override void BuildRenderTree(RenderTreeBuilder builder) {
		builder.OpenElement(0, "table");
		builder.AddAttribute(2, "class", Class);

		builder.OpenElement(3, "thead");
		HeaderTemplate(builder);
		builder.CloseElement();

		builder.OpenElement(4, "tbody");

		foreach (TItem item in ViewModel!.Items) {
			builder.OpenElement(5, "span");
			builder.AddAttribute(6, "style", "display: contents");
			builder.AddAttribute(7, "onclick", EventCallback.Factory.Create(this, () => OnRowSelect.InvokeAsync(item)));

			ItemTemplate(item)(builder);

			builder.CloseElement();
		}

		builder.CloseElement();
		builder.CloseElement();
	}

	/// <inheritdoc />
	protected override void OnParametersSet() {
		if (ItemTemplate is null) {
			throw new InvalidOperationException("Item template parameter is required.");
		}

		if (HeaderTemplate is null) {
			throw new InvalidOperationException("Header template parameter is required.");
		}

		if (Source is null) {
			throw new InvalidOperationException("Source parameter is required.");
		}

		base.OnParametersSet();
	}
}
