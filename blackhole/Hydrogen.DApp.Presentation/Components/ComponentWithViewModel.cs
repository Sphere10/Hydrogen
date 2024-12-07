// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Hamish Rose
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Hydrogen.DApp.Presentation.ViewModels;

namespace Hydrogen.DApp.Presentation.Components;

/// <summary>
/// Extends <see cref="ComponentBase"/> adding common functionality and view model support.
/// </summary>
/// <typeparam name="TViewModel"></typeparam>
public abstract class ComponentWithViewModel<TViewModel> : ComponentBase where TViewModel : ComponentViewModelBase {
	/// <summary>
	/// Gets the view model for this component
	/// </summary>
	[Inject]
	// ReSharper disable once AutoPropertyCanBeMadeGetOnly.Global
	public TViewModel? ViewModel { get; set; } = null!;

	/// <summary>
	/// Method invoked when the component is ready to start, having received its
	/// initial parameters from its parent in the render tree.
	/// Override this method if you will perform an asynchronous operation and
	/// want the component to refresh when that operation is completed.
	/// </summary>
	/// <returns>A <see cref="T:System.Threading.Tasks.Task" /> representing any asynchronous operation.</returns>
	protected override async Task OnInitializedAsync() {
		if (ViewModel is null) {
			throw new InvalidOperationException("View model has not been injected successfully and is null");
		} else {
			await ViewModel.InitAsync();
			ViewModel.StateHasChangedDelegate = StateHasChanged;
			await base.OnInitializedAsync();
		}
	}
}
