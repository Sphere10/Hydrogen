// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT NON-AI software license, see the accompanying file
// LICENSE or visit https://sphere10.com/legal/NON-AI-MIT.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

namespace Hydrogen;

public enum PreAllocationPolicy {
	/// <summary>
	/// The initial block of pre-allocated items is used, never grown or reduced.
	/// </summary>
	Fixed,

	/// <summary>
	/// The Capacity is grown in fixed-sized blocks as needed and never reduced.
	/// </summary>
	ByBlock,

	/// <summary>
	/// The Capacity is grown (and reduced) to meet the item Count.
	/// </summary>
	MinimumRequired,

}
