// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;

namespace Hydrogen;

[Flags]
public enum ClusteredStreamTraits : byte {
	/// <summary>
	/// Stream is standard
	/// </summary>
	None = 0,

	/// <summary>
	/// Stream should be interpreted as null (not empty)
	/// </summary>
	IsNull = 1 << 0,

	/// <summary>
	/// In Dictionary usage, this bit indicates whether Stream is a part of the Dictionary. When 0, it is available to be used as a slot for the dictionary item.
	/// </summary>
	/// TODO: rename to IsTombstone and invert usage
	IsUsed = 1 << 1,


	Default = None,
}
