// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Hamish Rose
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Hydrogen.DApp.Presentation.Models;
using Hydrogen.DApp.Presentation.Services;

namespace Hydrogen.DApp.Presentation.Loader.Services;

/// <summary>
/// Node service -- example data service not for real use with node.
/// </summary>
public sealed class MockNodeService : INodeService, IDisposable {
	private List<Block> Blocks { get; } = Enumerable.Range(0, 1000)
		.Select(x => new Block { Address = Guid.NewGuid().ToString(), Number = x }).ToList();

	/// <summary>
	/// Initializes a new instance of the <see cref="MockNodeService"/> class.
	/// </summary>
	/// <param name="endpointManager"></param>
	public MockNodeService(IEndpointManager endpointManager) {
		EndpointManager = endpointManager ?? throw new ArgumentNullException(nameof(endpointManager));

		Endpoint = endpointManager.Endpoint;
		EndpointManager.EndpointChanged += EndpointManagerOnEndpointChanged;
	}

	/// <summary>
	/// Gets the last block number.
	/// </summary>
	private int LastBlockNumber { get; set; } = 1000;

	/// <summary>
	/// Gets the server that is used as data source.
	/// </summary>
	private Uri Endpoint { get; }

	/// <summary>
	/// Gets the config service.
	/// </summary>
	private IEndpointManager EndpointManager { get; }

	/// <summary>
	/// Begin receiving new blocks, async awaiting until the next block is available. The provided
	/// enumerable does not have an end and should be handled accordingly.
	/// </summary>
	/// <returns> stream of new blocks.</returns>
	public async IAsyncEnumerable<Block> GetBlocksAsync([EnumeratorCancellation] CancellationToken ct = default) {
		Random random = new();

		while (!ct.IsCancellationRequested) {
			await Task.Delay(random.Next(1, 25), ct);

			yield return new Block { Number = LastBlockNumber++ };
		}
	}

	/// <summary>
	/// Gets n blocks in a page given starting from index
	/// </summary>
	/// <returns> response</returns>
	public Task<ItemsResponse<Block>> GetBlocksAsync(ItemRequest request) {
		return Task.FromResult(new ItemsResponse<Block>(Blocks.Skip(request.Index).Take(request.Count),
			Blocks.Count));
	}

	public void Dispose() {
		EndpointManager.EndpointChanged -= EndpointManagerOnEndpointChanged;
	}

	private void EndpointManagerOnEndpointChanged(object? sender, EventArgs e) {
	}
}
