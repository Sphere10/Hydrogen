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

        public Uri ActiveServer => ServerConfigService.ActiveServer;

        public IEnumerable<Uri> AvailableServers => ServerConfigService.AvailableServers;

        public DataSourceViewModel(IServerConfigService serverConfigService)
        {
            ServerConfigService = serverConfigService ?? throw new ArgumentNullException(nameof(serverConfigService));
            ServerConfigService.ActiveServerChanged += ServerConfigOnEvent;
            ServerConfigService.NewServerAdded += ServerConfigOnEvent;
        }

        private void ServerConfigOnEvent(object? sender, EventArgs e)
        {
            StateHasChangedDelegate?.Invoke();
        }

        public async Task OnSelectServerAsync(Uri server) => await ServerConfigService.SetActiveServerAsync(server);

        public void Dispose()
        {
            ServerConfigService.ActiveServerChanged -= ServerConfigOnEvent;
            ServerConfigService.NewServerAdded -= ServerConfigOnEvent;
        }
    }
}