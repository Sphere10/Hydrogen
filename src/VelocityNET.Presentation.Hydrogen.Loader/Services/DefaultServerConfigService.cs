using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blazored.LocalStorage;
using Microsoft.Extensions.Options;
using VelocityNET.Presentation.Hydrogen.Services;

namespace VelocityNET.Presentation.Hydrogen.Loader.Services
{

    /// <summary>
    /// Server / backend configuration service.
    /// </summary>
    public class DefaultServerConfigService : IServerConfigService
    {
        private readonly string _hydrogenServerKey = "hydrogen.servers";

        /// <summary>
        /// Raised when active server has been changed.
        /// </summary>
        public event EventHandler<EventArgs>? ActiveServerChanged;

        /// <summary>
        /// Raised when a new server is added
        /// </summary>
        public event EventHandler<EventArgs>? NewServerAdded;

        public DefaultServerConfigService(
            IOptions<DataSourceOptions> configuration,
            ISyncLocalStorageService syncLocalStorageService, ILocalStorageService localStorageService)
        {
            Configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            SyncLocalStorageService = syncLocalStorageService ??
                throw new ArgumentNullException(nameof(syncLocalStorageService));
            LocalStorageService = localStorageService ?? throw new ArgumentNullException(nameof(localStorageService));

            var configuredServers = Configuration.Value.Servers;

            if (SyncLocalStorageService.ContainKey(_hydrogenServerKey))
            {
                configuredServers = configuredServers
                    .Concat(SyncLocalStorageService.GetItem<IEnumerable<Uri>>(_hydrogenServerKey))
                    .ToList();
            }

            ActiveServer = configuredServers.First();
            AvailableServers = configuredServers;
        }

        /// <summary>
        /// Gets the active server
        /// </summary>
        public Uri ActiveServer { get; private set; }

        /// <summary>
        /// Gets the available servers
        /// </summary>
        public IEnumerable<Uri> AvailableServers { get; private set; }

        /// <summary>
        /// Gets the config options
        /// </summary>
        private IOptions<DataSourceOptions> Configuration { get; }

        /// <summary>
        /// Gets the sync local storage service
        /// </summary>
        private ISyncLocalStorageService SyncLocalStorageService { get; }

        /// <summary>
        /// Gets the async local storage service
        /// </summary>
        private ILocalStorageService LocalStorageService { get; }

        /// <summary>
        /// Sets the active server
        /// </summary>
        /// <param name="server"></param>
        /// <returns></returns>
        public Task SetActiveServerAsync(Uri server)
        {
            ActiveServer = server;
            ActiveServerChanged?.Invoke(this, EventArgs.Empty);
            return Task.CompletedTask;
        }

        /// <summary>
        /// Adds another server config. Should be persisted.
        /// </summary>
        /// <param name="server"> config</param>
        /// <returns></returns>
        public async Task AddServerAsync(Uri server)
        {
            if (await ValidateServerAsync(server))
            {
                List<Uri> existing = new();

                if (await LocalStorageService.ContainKeyAsync(_hydrogenServerKey))
                {
                    existing = (await LocalStorageService.GetItemAsync<IEnumerable<Uri>>(_hydrogenServerKey))
                        .ToList();
                }

                if (existing.All(x => x != server))
                {
                    existing.Add(server);

                    await LocalStorageService.SetItemAsync(_hydrogenServerKey, existing);

                    AvailableServers = AvailableServers.Append(server);
                    NewServerAdded?.Invoke(this, EventArgs.Empty);
                }
            }
            else
            {
                throw new ArgumentException("Invalid server configuration", nameof(server));
            }
        }

        /// <summary>
        /// Validates the given server details
        /// </summary>
        /// <param name="server"> server</param>
        /// <returns> whether this is a valid server</returns>
        public Task<bool> ValidateServerAsync(Uri server)
        {
            return Task.FromResult(true);
        }
    }

}