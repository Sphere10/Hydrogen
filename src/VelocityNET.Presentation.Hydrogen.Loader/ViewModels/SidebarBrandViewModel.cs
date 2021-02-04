using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VelocityNET.Presentation.Hydrogen.Services;
using VelocityNET.Presentation.Hydrogen.ViewModels;

namespace VelocityNET.Presentation.Hydrogen.Loader.ViewModels
{

    public class SidebarBrandViewModel : ComponentViewModelBase, IDisposable
    {
        private IServerConfigService ServerConfigService { get; }

        public Uri ActiveServer => ServerConfigService.ActiveServer;

        public IEnumerable<Uri> AvailableServers => ServerConfigService.AvailableServers;

        public SidebarBrandViewModel(IServerConfigService serverConfigService)
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