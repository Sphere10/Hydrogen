// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT NON-AI software license, see the accompanying file
// LICENSE or visit https://sphere10.com/legal/NON-AI-MIT.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

namespace Hydrogen;

/// <summary>
/// Defines how expiration windows are measured for cached items.
/// </summary>
public enum ExpirationPolicy {
	/// <summary>
	/// Expiration is measured from the time the value was fetched.
	/// </summary>
	SinceFetchedTime,
	/// <summary>
	/// Expiration is measured from the last access time.
	/// </summary>
	SinceLastAccessedTime,
	/// <summary>
	/// Items do not expire automatically.
	/// </summary>
	None
}
