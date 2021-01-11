using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VelocityNET.Presentation.Hydrogen.Models;
using VelocityNET.Presentation.Hydrogen.Services;
using VelocityNET.Presentation.Hydrogen.ViewModels;

namespace VelocityNET.Presentation.Hydrogen.Loader.ViewModels
{
    public class ServersViewModel : ComponentViewModelBase
    {
        public IEnumerable<Server> Servers => ConfigService.AvailableServers;
        private IServerConfigService ConfigService { get; }

        public Server ActiveServer => ConfigService.ActiveServer;

        public ServersViewModel(IServerConfigService configService)
        {
            ConfigService = configService ?? throw new ArgumentNullException(nameof(configService));
            configService.ActiveServerChanged += ConfigServiceOnActiveServerChanged;
        }

        private void ConfigServiceOnActiveServerChanged(object? sender, EventArgs e)
        {
            StateHasChangedDelegate?.Invoke();
        }

        public async Task SelectActiveServer(Server server)
        {
            await ConfigService.SetActiveServerAsync(server);
            StateHasChangedDelegate?.Invoke();
        }
    }
}