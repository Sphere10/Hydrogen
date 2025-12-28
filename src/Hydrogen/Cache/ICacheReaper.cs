// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT NON-AI software license, see the accompanying file
// LICENSE or visit https://sphere10.com/legal/NON-AI-MIT.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

namespace Hydrogen;

/// <summary>
/// Coordinates freeing capacity across caches.
/// </summary>
public interface ICacheReaper {

	/// <summary>
	/// Registers a cache so the reaper can manage its capacity.
	/// </summary>
	void Register(ICache cache);

	/// <summary>
	/// Deregisters a previously registered cache.
	/// </summary>
	void Deregister(ICache cache);

	/// <summary>
	/// Indicates how much space is currently free.
	/// </summary>
	long AvailableSpace();

	/// <summary>
	/// Attempts to free the requested number of bytes for the specified cache.
	/// </summary>
	long MakeSpace(ICache requestingCache, long requestedBytes);
}
