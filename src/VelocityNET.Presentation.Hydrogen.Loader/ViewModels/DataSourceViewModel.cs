using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VelocityNET.Presentation.Hydrogen.Models;
using VelocityNET.Presentation.Hydrogen.Services;
using VelocityNET.Presentation.Hydrogen.ViewModels;

namespace VelocityNET.Presentation.Hydrogen.Loader.ViewModels
{

    /// <summary>
    /// Data source view modal base
    /// </summary>
    public class DataSourceViewModel : ComponentViewModelBase, IDisposable
    {
        private IServerConfigService ServerConfigService { get; }

        public Server ActiveServer => ServerConfigService.ActiveServer;

        public IEnumerable<Server> AvailableServers => ServerConfigService.AvailableServers;

        public DataSourceViewModel(IServerConfigService serverConfigService)
        {
            ServerConfigService = serverConfigService ?? throw new ArgumentNullException(nameof(serverConfigService));
            ServerConfigService.ActiveServerChanged += ServerConfigServiceOnActiveServerChanged;
        }

        private void ServerConfigServiceOnActiveServerChanged(object? sender, EventArgs e)
        {
            StateHasChangedDelegate?.Invoke();
        }

        public async Task OnSelectServerAsync(Server server) => await ServerConfigService.SetActiveServerAsync(server);

        public void Dispose()
        {
            ServerConfigService.ActiveServerChanged -= ServerConfigServiceOnActiveServerChanged;
        }
    }

}