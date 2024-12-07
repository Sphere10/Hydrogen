using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Hydrogen;

namespace Hydrogen.DApp.Presentation2.UI.MainFrame {

	/// <summary>
	/// Search input component
	/// </summary>
	public partial class SearchInput {

		/// <summary>
		/// Search provider delegate - 
		/// </summary>
		/// <param name="searchTerm"></param>
		public delegate Task<IEnumerable<string>> SearchProviderDelegate(string searchTerm);


		/// <summary>
		/// Gets or sets the search provider delegate that will facilitate searching
		/// </summary>
		[Parameter]
		public SearchProviderDelegate? SearchProvider { get; set; }

		/// <summary>
		/// Gets or sets the limit in MS of search frequency - optional, default value of 10ms.
		/// </summary>
		[Parameter]
		public int SearchFreqLimitMs { get; set; } = 100;

		/// <summary>
		/// Gets or sets the number of items to show in the results.
		/// </summary>
		[Parameter]
		public int ResultsCount { get; set; } = 10;

		/// <summary>
		/// Gets or sets the current search term result set.
		/// </summary>
		public IEnumerable<string> Results { get; set; } = new List<string>();

		/// <summary>
		/// Gets or sets the throttle object use
		/// </summary>
		private Throttle? Throttle { get; set; }

		/// <summary>
		/// Gets the semaphore used to throttle requests. 
		/// </summary>
		private SemaphoreSlim Semaphore { get; } = new(1);

		/// <summary>
		/// handles the user's request to search, probably on key press.
		/// </summary>
		/// <param name="term"> search term</param>
		/// <returns></returns>
		public async Task OnSearchAsync(string term) {
			if (!string.IsNullOrWhiteSpace(term)) {
				if (SearchProvider is not null) {
					if (Throttle is not null) {
						try {
							string latest = term;

							if (Semaphore.CurrentCount != 1) {
								await Throttle.WaitAsync();

								var results = await SearchProvider(latest);
								Results = results;
							}
						} finally {
							Semaphore.Release();
						}
					}
				}
			}
		}

		/// <inheritdoc />
		protected override void OnParametersSet() {
			base.OnParametersSet();
			Throttle = new Throttle(TimeSpan.FromMilliseconds(SearchFreqLimitMs));
		}
	}
}
