// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;

namespace Hydrogen.DApp.Core.Runtime;

/// <summary>
/// Describes the characteristics of a Hydrogen node.
/// </summary>
[Flags]
public enum NodeTraits {
	/// <summary>
	/// Stores minimal data for light-client functionality.
	/// </summary>
	Light,

	/// <summary>
	/// Fully validating node, stores all data for consensus.
	/// </summary>
	Full,

	/// <summary>
	/// Archival node, stores the full history of network data.
	/// </summary>
	Archival,

	/// <summary>
	/// Mining server, can be used to mine blocks by mining clients.
	/// </summary>
	MiningServer,

	/// <summary>
	/// GUI server, permits GUI app to connect to for data.
	/// </summary>
	GUIServer,

	/// <summary>
	/// Node is hosted by a Hydrogen Host, which permits auto-upgrading.
	/// </summary>
	Hosted
}
