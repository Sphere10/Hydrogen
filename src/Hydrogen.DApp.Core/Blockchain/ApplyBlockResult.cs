// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

namespace Hydrogen.DApp.Core.Blockchain;

public enum ApplyBlockResult {
	/// <summary>
	/// Block was appended to the blockchain successfully
	/// </summary>
	Success,

	/// <summary>
	/// Block was not appended to the blockchain due to invalid data
	/// </summary>
	InvalidBlock,

	/// <summary>
	/// Block was not appended because it did not satisfy the leader consensus rule (i.e. not enough work in PoW, or wrong staker in PoS, etc)
	/// </summary>
	InvalidLeader
}
