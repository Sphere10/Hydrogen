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
	Null = 1 << 0,

	/// <summary>
	/// Represents a stream which is deleted but whose index can be resurrected for a value.
	/// </summary>
	Tomb = 1 << 1,

	/// <summary>
	/// Default tran for a clustered stream
	/// </summary>
	Default = None,
}
