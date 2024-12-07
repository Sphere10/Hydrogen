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
using System.Threading.Tasks;
using Hydrogen.DApp.Presentation.ViewModels;

namespace Hydrogen.DApp.Presentation.Components.Tables;

/// <summary>
/// Rapid table control view model
/// </summary>
/// <typeparam name="TItem"> item type</typeparam>
public class RapidTableViewModel<TItem> : ComponentViewModelBase, IDisposable {
	/// <summary>
	/// Gets or sets the async enumerable item source
	/// </summary>
	public IAsyncEnumerable<TItem> Source { get; set; } = null!;

	/// <summary>
	/// Gets or sets the control items source
	/// </summary>
	public List<TItem> Items { get; } = new();

	/// <summary>
	/// Gets or sets the cancellation token used to cancel enumeration of the data source.
	/// </summary>
	public CancellationToken CancellationToken { get; set; }

	/// <summary>
	/// Gets or sets the task that performs the background work of enumerating the data source
	/// and updating the bound items collection
	/// </summary>
	private Task EnumeratorTask { get; set; } = null!;

	/// <summary>
	/// Gets the item limit
	/// </summary>
	public int ItemLimit { get; set; } = 25;

	/// <summary>
	/// Called when view is initialized, override to provide custom initialization logic. 
	/// </summary>
	/// <returns></returns>
	protected override Task InitCoreAsync() {
		EnumeratorTask = Task.Run(async () => {
				await foreach (var item in Source.WithCancellation(CancellationToken)) {
					if (Items.Count >= ItemLimit) {
						Items.RemoveAt(0);
					}

					Items.Add(item);
					StateHasChangedDelegate?.Invoke();
				}

			},
			CancellationToken);

		return Task.CompletedTask;
	}

	/// <summary>
	/// Dispose.
	/// </summary>
	public void Dispose() {
		EnumeratorTask.Dispose();
	}
}
