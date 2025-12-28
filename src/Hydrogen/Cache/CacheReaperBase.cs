// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT NON-AI software license, see the accompanying file
// LICENSE or visit https://sphere10.com/legal/NON-AI-MIT.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

namespace Hydrogen;

/// <summary>
/// Base implementation for cache reapers that enforces registration semantics.
/// </summary>
public abstract class CacheReaperBase : ICacheReaper {

	/// <inheritdoc />
	public abstract void Register(ICache cache);

	/// <inheritdoc />
	public abstract void Deregister(ICache cache);

	/// <inheritdoc />
	public abstract long AvailableSpace();

	/// <inheritdoc />
	public abstract long MakeSpace(ICache requestingCache, long requestedBytes);
}
