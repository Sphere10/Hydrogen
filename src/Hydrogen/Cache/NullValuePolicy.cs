// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT NON-AI software license, see the accompanying file
// LICENSE or visit https://sphere10.com/legal/NON-AI-MIT.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

namespace Hydrogen;

/// <summary>
/// Specifies how caches should treat fetched <c>null</c> values.
/// </summary>
public enum NullValuePolicy {
	/// <summary>
	/// Cache <c>null</c> like any other value.
	/// </summary>
	CacheNormally,
	/// <summary>
	/// Return <c>null</c> but do not keep it in the cache.
	/// </summary>
	ReturnButDontCache,
	/// <summary>
	/// Treat a <c>null</c> fetch as an error.
	/// </summary>
	Throw
}
