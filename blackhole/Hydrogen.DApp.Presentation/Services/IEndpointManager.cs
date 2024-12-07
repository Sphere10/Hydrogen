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

namespace Hydrogen.DApp.Presentation.Services;

/// <summary>
/// Endpoint manager
/// </summary>
public interface IEndpointManager {
	/// <summary>
	/// Raised when endpoint is changed
	/// </summary>
	event EventHandler<EventArgs>? EndpointChanged;

	/// <summary>
	/// Raised when a new server is added
	/// </summary>
	event EventHandler<EventArgs>? EndpointAdded;

	/// <summary>
	/// Gets the currently selected endpoint Uri.
	/// </summary>
	Uri Endpoint { get; }

	/// <summary>
	/// Gets the available endpoints
	/// </summary>
	public IEnumerable<Uri> Endpoints { get; }

	/// <summary>
	/// Sets the current in use endpoint.
	/// </summary>
	/// <param name="uri"></param>
	/// <returns></returns>
	Task SetCurrentEndpointAsync(Uri uri);

	/// <summary>
	/// Add a new endpoint URI to the available endpoints.
	/// </summary>
	/// <param name="uri"> config</param>
	/// <returns></returns>
	Task AddEndpointAsync(Uri uri);

	/// <summary>
	/// Validates the given server details
	/// </summary>
	/// <param name="uri"> server</param>
	/// <returns> whether this is a valid server</returns>
	Task<Result<bool>> ValidateEndpointAsync(Uri uri);
}
