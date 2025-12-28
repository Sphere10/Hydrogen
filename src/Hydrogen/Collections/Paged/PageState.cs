// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT NON-AI software license, see the accompanying file
// LICENSE or visit https://sphere10.com/legal/NON-AI-MIT.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

namespace Hydrogen;

/// <summary>
/// Represents the lifecycle of a page as it is loaded, unloaded, and removed.
/// </summary>
public enum PageState {
	/// <summary>
	/// The page is being hydrated from storage.
	/// </summary>
	Loading,
	/// <summary>
	/// The page is resident and ready for access.
	/// </summary>
	Loaded,
	/// <summary>
	/// The page is being evicted from active memory.
	/// </summary>
	Unloading,
	/// <summary>
	/// The page has been unloaded but still exists logically.
	/// </summary>
	Unloaded,
	/// <summary>
	/// The page is currently being removed.
	/// </summary>
	Deleting,
	/// <summary>
	/// The page has been removed and should no longer be used.
	/// </summary>
	Deleted
}
