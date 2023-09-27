// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
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
