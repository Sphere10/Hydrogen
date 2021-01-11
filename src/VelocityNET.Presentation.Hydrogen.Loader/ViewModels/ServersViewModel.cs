using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VelocityNET.Presentation.Hydrogen.Components.Modal;
using VelocityNET.Presentation.Hydrogen.Loader.Components;
using VelocityNET.Presentation.Hydrogen.Models;
using VelocityNET.Presentation.Hydrogen.Services;
using VelocityNET.Presentation.Hydrogen.ViewModels;

namespace VelocityNET.Presentation.Hydrogen.Loader.ViewModels
{
    public class ServersViewModel : ComponentViewModelBase
    {
        public IEnumerable<Server> Servers => ConfigService.AvailableServers;
        
        private IServerConfigService ConfigService { get; }
        
        private IModalService ModalService { get; }

        public Server ActiveServer => ConfigService.ActiveServer;

        public ServersViewModel(IServerConfigService configService, IModalService modalService)
        {
            ConfigService = configService ?? throw new ArgumentNullException(nameof(configService));
            ModalService = modalService;
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

        public async Task AddNewServerAsync()
        {
            ModalResult result = await ModalService.ShowAsync<NewServerModal>();

            if (result.ResultType == ModalResultType.Ok)
            {
                Server newServer = result.GetData<Server>();
                
                if (await ConfigService.ValidateServerAsync(newServer))
                {
                    await ConfigService.AddServerAsync(newServer);
                }
            }
        }
    }
}