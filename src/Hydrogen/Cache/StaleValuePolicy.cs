// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT NON-AI software license, see the accompanying file
// LICENSE or visit https://sphere10.com/legal/NON-AI-MIT.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

namespace Hydrogen;

/// <summary>
/// Indicates how cached items are checked for staleness beyond expiration rules.
/// </summary>
public enum StaleValuePolicy {
	/// <summary>
	/// Items are assumed to remain valid until expiration.
	/// </summary>
	AssumeNeverStale,
	/// <summary>
	/// Staleness is evaluated on-demand using the cache-provided predicate.
	/// </summary>
	CheckStaleOnDemand,
	/// <summary>
	/// Placeholder for background staleness monitoring (not currently implemented).
	/// </summary>
	BackgroundCheck,
}
