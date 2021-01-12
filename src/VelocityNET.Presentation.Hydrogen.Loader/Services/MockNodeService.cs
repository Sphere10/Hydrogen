using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using VelocityNET.Presentation.Hydrogen.Models;
using VelocityNET.Presentation.Hydrogen.Services;

namespace VelocityNET.Presentation.Hydrogen.Loader.Services
{
    /// <summary>
    /// Node service -- example data service not for real use with node.
    /// </summary>
    public sealed class MockNodeService : INodeService, IDisposable
    {

        public MockNodeService(IServerConfigService configService)
        {
            ConfigService = configService ?? throw new ArgumentNullException(nameof(configService));

            Server = ConfigService.ActiveServer;
            ConfigService.ActiveServerChanged += ConfigServiceOnActiveServerChanged;
        }

        /// <summary>
        /// Gets the last block number.
        /// </summary>
        private int LastBlockNumber { get; set; } = 1000;

        /// <summary>
        /// Gets the server that is used as data source.
        /// </summary>
        private Uri Server { get; }
        
        /// <summary>
        /// Gets the config service.
        /// </summary>
        private IServerConfigService ConfigService { get; }
        
        /// <summary>
        /// Begin receiving new blocks, async awaiting until the next block is available. The provided
        /// enumerable does not have an end and should be handled accordingly.
        /// </summary>
        /// <returns> stream of new blocks.</returns>
        public async IAsyncEnumerable<Block> GetBlocksAsync([EnumeratorCancellation] CancellationToken ct = default)
        {
            Random random = new();

            while (!ct.IsCancellationRequested)
            {
                await Task.Delay(random.Next(5000, 10000), ct);

                yield return new Block {Number = LastBlockNumber++};
            }
        }

        public void Dispose()
        {
            ConfigService.ActiveServerChanged -= ConfigServiceOnActiveServerChanged;
        }
        
        private void ConfigServiceOnActiveServerChanged(object? sender, EventArgs e)
        {
            // server / node has changed, cancel stuff and reinitialize
        }
    }
}