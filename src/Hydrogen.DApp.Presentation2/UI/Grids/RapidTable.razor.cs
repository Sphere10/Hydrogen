using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;

namespace Hydrogen.DApp.Presentation2.UI.Grids {

	/// <summary>
	/// Rapid table control / component. async enumerates an async enumerable e.g. a stream
	/// or channel until cancelled or disposed. Generates a table with thead, and tbody from templates
	/// </summary>
	public partial class RapidTable<TItem> : IDisposable {
		/// <summary>
		/// Gets or sets the async item source that will be enumerated.
		/// </summary>
		[Parameter]
		public IAsyncEnumerable<TItem> Source { get; set; }

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
		public int ItemLimit { get; set; } = 25;

		/// <summary>
		/// Gets or sets the cancellation token used to cancel the async enumeration of the source property.
		/// </summary>
		[Parameter]
		public CancellationToken CancellationToken { get; set; }

		/// <summary>
		/// Gets or sets the callback to call when row is clicked
		/// </summary>
		[Parameter]
		public EventCallback<TItem> OnRowSelect { get; set; } = EventCallback<TItem>.Empty;

		/// <summary>
		/// Gets or sets the css class applied to the table element
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

			if (Source is null) {
				throw new InvalidOperationException("Source parameter is required.");
			}

			base.OnParametersSet();
		}

		protected override void OnAfterRender(bool firstRender) {
			if (firstRender) {
				EnumeratorTask = Task.Run(async () => {
						await foreach (var item in Source.WithCancellation(CancellationToken)) {
							if (Items.Count >= ItemLimit) {
								Items.RemoveAt(0);
							}

							Items.Add(item);
							StateHasChanged();
						}
					},
					CancellationToken);
			}
		}

		/// <summary>
		/// Gets or sets the control items source
		/// </summary>
		private List<TItem> Items { get; } = new();

		/// <summary>
		/// Gets or sets the task that performs the background work of enumerating the data source
		/// and updating the bound items collection
		/// </summary>
		private Task EnumeratorTask { get; set; } = null!;

		/// <summary>
		/// Dispose.
		/// </summary>
		public void Dispose() {
			EnumeratorTask.Dispose();
		}
	}

}
