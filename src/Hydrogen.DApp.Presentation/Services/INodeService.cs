// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Hamish Rose
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Hydrogen.DApp.Presentation.Models;

namespace Hydrogen.DApp.Presentation.Services;

/// <summary>
/// Node service -- example data service
/// </summary>
public interface INodeService {
	/// <summary>
	/// Begin receiving new blocks, async awaiting until the next block is available. The provided
	/// enumerable does not have an end and should be handled accordingly.
	/// </summary>
	/// <returns> stream of new blocks.</returns>
	IAsyncEnumerable<Block> GetBlocksAsync(CancellationToken ct = default);

	/// <summary>
	/// Gets n blocks in a page given starting from index
	/// </summary>
	/// <returns> response</returns>
	Task<ItemsResponse<Block>> GetBlocksAsync(ItemRequest itemRequest);
}
