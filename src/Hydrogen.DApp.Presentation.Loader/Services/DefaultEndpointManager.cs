// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Hamish Rose
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blazored.LocalStorage;
using Microsoft.Extensions.Options;
using Hydrogen.DApp.Presentation.Services;

namespace Hydrogen.DApp.Presentation.Loader.Services;

/// <summary>
/// Server / backend configuration service.
/// </summary>
public class DefaultEndpointManager : IEndpointManager {
	/// <summary>
	/// key that endpoint values are stored against in browser local streams.
	/// </summary>
	private readonly string _hydrogenEndpointKey = "hydrogen.endpoints";

	/// <summary>
	/// Raised when the current endpoint in use is changed.
	/// </summary>
	public event EventHandler<EventArgs>? EndpointChanged;

	/// <summary>
	/// Raised when a new endpoint is added.
	/// </summary>
	public event EventHandler<EventArgs>? EndpointAdded;

	/// <summary>
	/// Initializes a new instance of the <see cref="DefaultEndpointManager"/> class.
	/// </summary>
	/// <param name="configuration"></param>
	/// <param name="syncLocalStorageService"></param>
	/// <param name="localStorageService"></param>
	public DefaultEndpointManager(
		IOptions<DataSourceOptions> configuration,
		ISyncLocalStorageService syncLocalStorageService, ILocalStorageService localStorageService) {
		Configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
		SyncLocalStorageService = syncLocalStorageService ??
		                          throw new ArgumentNullException(nameof(syncLocalStorageService));
		LocalStorageService = localStorageService ?? throw new ArgumentNullException(nameof(localStorageService));

		var endpoints = Configuration.Value.Servers;

		if (SyncLocalStorageService.ContainKey(_hydrogenEndpointKey)) {
			endpoints = endpoints
				.Concat(SyncLocalStorageService.GetItem<IEnumerable<Uri>>(_hydrogenEndpointKey))
				.ToList();
		}

		Endpoint = endpoints.First();
		Endpoints = endpoints;
	}

	/// <summary>
	/// Gets the active server
	/// </summary>
	public Uri Endpoint { get; private set; }

	/// <summary>
	/// Gets the config options
	/// </summary>
	private IOptions<DataSourceOptions> Configuration { get; }

	/// <summary>
	/// Gets the sync local streams service
	/// </summary>
	private ISyncLocalStorageService SyncLocalStorageService { get; }

	/// <summary>
	/// Gets the async local streams service
	/// </summary>
	private ILocalStorageService LocalStorageService { get; }

	/// <summary>
	/// Validates the given server details
	/// </summary>
	/// <param name="uri"> server</param>
	/// <returns> whether this is a valid server</returns>
	public Task<Result<bool>> ValidateEndpointAsync(Uri uri) {
		return Task.FromResult<Result<bool>>(true);
	}

	/// <summary>
	/// Gets the available endpoints
	/// </summary>
	public IEnumerable<Uri> Endpoints { get; private set; }

	/// <summary>
	/// Sets the current in use endpoint.
	/// </summary>
	/// <param name="uri"></param>
	/// <returns></returns>
	public Task SetCurrentEndpointAsync(Uri uri) {
		Endpoint = uri;
		EndpointChanged?.Invoke(this, EventArgs.Empty);
		return Task.CompletedTask;
	}

	/// <summary>
	/// Add a new endpoint URI to the available endpoints.
	/// </summary>
	/// <param name="uri"> config</param>
	/// <returns></returns>
	public async Task AddEndpointAsync(Uri uri) {
		Result<bool> validationResult = await ValidateEndpointAsync(uri);

		if (validationResult) {
			List<Uri> existing = new();

			if (await LocalStorageService.ContainKeyAsync(_hydrogenEndpointKey)) {
				existing = (await LocalStorageService.GetItemAsync<IEnumerable<Uri>>(_hydrogenEndpointKey))
					.ToList();
			}

			if (existing.All(x => x != uri)) {
				existing.Add(uri);

				await LocalStorageService.SetItemAsync(_hydrogenEndpointKey, existing);

				Endpoints = Endpoints.Append(uri);
				EndpointAdded?.Invoke(this, EventArgs.Empty);
			}
		} else {
			throw new ArgumentException(
				$"Invalid endpoint configuration, {validationResult.ErrorMessages.Aggregate((x, y) => $"{x}, {y}")}",
				nameof(uri));
		}
	}
}
