// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Hamish Rose
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Hydrogen.DApp.Presentation.Services;
using Hydrogen.DApp.Presentation.ViewModels;

namespace Hydrogen.DApp.Presentation.Loader.ViewModels;

public class SidebarBrandViewModel : ComponentViewModelBase, IDisposable {
	private IEndpointManager EndpointManager { get; }

	public Uri Endpoint => EndpointManager.Endpoint;

	public IEnumerable<Uri> Endpoints => EndpointManager.Endpoints;

	public SidebarBrandViewModel(IEndpointManager endpointManager) {
		EndpointManager = endpointManager ?? throw new ArgumentNullException(nameof(endpointManager));
		EndpointManager.EndpointChanged += EndpointManagerOnEvent;
		EndpointManager.EndpointAdded += EndpointManagerOnEvent;
	}

	private void EndpointManagerOnEvent(object? sender, EventArgs e) {
		StateHasChangedDelegate?.Invoke();
	}

	public async Task OnSelectEndpointAsync(Uri server) => await EndpointManager.SetCurrentEndpointAsync(server);

	public void Dispose() {
		EndpointManager.EndpointChanged -= EndpointManagerOnEvent;
		EndpointManager.EndpointAdded -= EndpointManagerOnEvent;
	}
}
