// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Hamish Rose
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using Hydrogen.DApp.Presentation.Services;

namespace Hydrogen.DApp.Presentation.ViewModels;

/// <summary>
/// Enhanced component view model base
/// </summary>
public abstract class ExtendedComponentViewModel : ComponentViewModelBase {
	/// <summary>
	/// Gets the endpoint manager
	/// </summary>
	public IEndpointManager EndpointManager { get; }

	/// <summary>
	/// Initializes a new instance of the <see cref="ExtendedComponentViewModel"/> class.
	/// </summary>
	/// <param name="endpointManager"> endpoint manager</param>
	/// <param name="messenger"></param>
	public ExtendedComponentViewModel(IEndpointManager endpointManager) {
		EndpointManager = endpointManager ?? throw new ArgumentNullException(nameof(endpointManager));
		EndpointManager.EndpointChanged += OnEndpointChanged;
	}

	/// <summary>
	/// Handles the endpoint changed event - reinitialize the view model.
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	private async void OnEndpointChanged(object? sender, EventArgs e) {
		try {
			await InitCoreAsync();
		} catch (Exception exception) {
			// Do something, show error notification / message. 
			Console.WriteLine(exception);
		}
	}
}
