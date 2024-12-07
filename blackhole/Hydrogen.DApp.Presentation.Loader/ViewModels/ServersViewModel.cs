// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Hamish Rose
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Hydrogen.DApp.Presentation.Services;
using Hydrogen.DApp.Presentation.ViewModels;

namespace Hydrogen.DApp.Presentation.Loader.ViewModels;

/// <summary>
/// View model for servers page.
/// </summary>
public class ServersViewModel : ComponentViewModelBase {
	/// <summary>
	/// Gets the available servers
	/// </summary>
	public IEnumerable<Uri> Servers => EndpointManager.Endpoints;

	/// <summary>
	/// Getse the server config service.
	/// </summary>
	private IEndpointManager EndpointManager { get; }

	/// <summary>
	/// Gets the modal service
	/// </summary>
	private IModalService ModalService { get; }

	/// <summary>
	/// Gets the active server
	/// </summary>
	public Uri ActiveServer => EndpointManager.Endpoint;

	/// <summary>
	/// Gets or sets the new server model
	/// </summary>
	public NewServer NewServer { get; set; } = new();

	/// <summary>
	/// Initializes a new instance of the <see cref="ServersViewModel"/> class.
	/// </summary>
	/// <param name="configService"></param>
	/// <param name="modalService"></param>
	public ServersViewModel(IEndpointManager configService, IModalService modalService) {
		EndpointManager = configService ?? throw new ArgumentNullException(nameof(configService));
		ModalService = modalService;
		configService.EndpointChanged += EndpointManagerOnEndpointChanged;
	}

	/// <summary>
	/// Handles active server change
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	private void EndpointManagerOnEndpointChanged(object? sender, EventArgs e) {
		StateHasChangedDelegate?.Invoke();
	}

	/// <summary>
	/// Handles select new active server.
	/// </summary>
	/// <param name="server"></param>
	/// <returns></returns>
	public async Task OnSelectActiveServer(Uri server) {
		await EndpointManager.SetCurrentEndpointAsync(server);
		StateHasChangedDelegate?.Invoke();
	}

	/// <summary>
	/// Handles form submit for new server
	/// </summary>
	/// <returns></returns>
	public async Task OnAddNewServerAsync() {
		if (NewServer.Uri is not null) {
			Uri address = new(NewServer.Uri);
			await EndpointManager.AddEndpointAsync(address);
			StateHasChangedDelegate?.Invoke();
		}
	}
}


public class NewServer {
	[Required(AllowEmptyStrings = false)] public string? Uri { get; set; }
}
