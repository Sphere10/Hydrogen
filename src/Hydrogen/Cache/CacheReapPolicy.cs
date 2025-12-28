// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT NON-AI software license, see the accompanying file
// LICENSE or visit https://sphere10.com/legal/NON-AI-MIT.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

namespace Hydrogen;

/// <summary>
/// Determines which items are evicted when a cache needs to free space.
/// </summary>
public enum CacheReapPolicy {
	/// <summary>
	/// Prefer removing items that have been accessed the least.
	/// </summary>
	LeastUsed,
	/// <summary>
	/// Prefer removing the largest items first.
	/// </summary>
	Largest,
	/// <summary>
	/// Prefer removing the smallest items first.
	/// </summary>
	Smallest,
	/// <summary>
	/// Prefer removing items idle for the longest duration.
	/// </summary>
	LongestIdle,
	/// <summary>
	/// Prefer removing the oldest fetched items.
	/// </summary>
	Oldest,
	/// <summary>
	/// Aggressively remove expired items, then any others until the request is satisfied.
	/// </summary>
	ASAP,
	/// <summary>
	/// Never reap automatically; only expired items are eligible.
	/// </summary>
	None
}
