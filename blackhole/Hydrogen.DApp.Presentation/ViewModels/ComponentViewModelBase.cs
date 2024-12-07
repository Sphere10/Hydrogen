// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Hamish Rose
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Threading.Tasks;

namespace Hydrogen.DApp.Presentation.ViewModels;

/// <summary>
/// Base class for component view models.
/// </summary>
public abstract class ComponentViewModelBase {
	/// <summary>
	/// Gets or sets the state change delegate
	/// </summary>
	public Action? StateHasChangedDelegate { get; set; }

	/// <summary>
	/// Gets or sets a value indicating whether the view model is initialized.
	/// Set to true by calling <see cref="InitAsync"/>
	/// </summary>
	public bool IsInitialized { get; protected set; }

	/// <summary>
	/// Initialize the view model.
	/// </summary>
	/// <returns> a task.</returns>
	public async Task InitAsync() {
		IsInitialized = false;
		await InitCoreAsync();
		IsInitialized = true;
	}

	/// <summary>
	/// Called when view is initialized, override to provide custom initialization logic. 
	/// </summary>
	/// <returns></returns>
	protected virtual Task InitCoreAsync() {
		return Task.CompletedTask;
	}
}
