// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT NON-AI software license, see the accompanying file
// LICENSE or visit https://sphere10.com/legal/NON-AI-MIT.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;

namespace Hydrogen;

/// <summary>
/// Flags describing the lifecycle state of a cached entry.
/// </summary>
[Flags]
public enum CachedItemTraits {
	/// <summary>
	/// Item is no longer valid and should be refreshed.
	/// </summary>
	Invalidated = 1 << 0,
	/// <summary>
	/// Item may be removed by reaping.
	/// </summary>
	CanPurge = 1 << 1,
	/// <summary>
	/// Item has been purged from the cache.
	/// </summary>
	Purged = 1 << 2,
	/// <summary>
	/// Default trait set for newly added items.
	/// </summary>
	Default = CanPurge,
}
