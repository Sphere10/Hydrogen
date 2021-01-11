using System.Collections.Generic;
using System.Threading;
using VelocityNET.Presentation.Hydrogen.Models;

namespace VelocityNET.Presentation.Hydrogen.Services
{
    /// <summary>
    /// Node service -- example data service
    /// </summary>
    public interface INodeService
    {
        /// <summary>
        /// Begin receiving new blocks, async awaiting until the next block is available. The provided
        /// enumerable does not have an end and should be handled accordingly.
        /// </summary>
        /// <returns> stream of new blocks.</returns>
        IAsyncEnumerable<Block> GetBlocksAsync(CancellationToken ct = default);
    }
}