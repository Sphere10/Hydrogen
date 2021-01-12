using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using VelocityNET.Presentation.Hydrogen.Components.Modal;
using VelocityNET.Presentation.Hydrogen.Loader.Components;
using VelocityNET.Presentation.Hydrogen.Models;
using VelocityNET.Presentation.Hydrogen.Services;
using VelocityNET.Presentation.Hydrogen.ViewModels;

namespace VelocityNET.Presentation.Hydrogen.Loader.ViewModels
{

    /// <summary>
    /// View model for servers page.
    /// </summary>
    public class ServersViewModel : ComponentViewModelBase
    {
        /// <summary>
        /// Gets the available servers
        /// </summary>
        public IEnumerable<Uri> Servers => ConfigService.AvailableServers;

        /// <summary>
        /// Getse the server config service.
        /// </summary>
        private IServerConfigService ConfigService { get; }

        /// <summary>
        /// Gets the modal service
        /// </summary>
        private IModalService ModalService { get; }

        /// <summary>
        /// Gets the active server
        /// </summary>
        public Uri ActiveServer => ConfigService.ActiveServer;

        /// <summary>
        /// Gets or sets the new server model
        /// </summary>
        public NewServer NewServer { get; set; } = new();

        /// <summary>
        /// Initializes a new instance of the <see cref="ServersViewModel"/> class.
        /// </summary>
        /// <param name="configService"></param>
        /// <param name="modalService"></param>
        public ServersViewModel(IServerConfigService configService, IModalService modalService)
        {
            ConfigService = configService ?? throw new ArgumentNullException(nameof(configService));
            ModalService = modalService;
            configService.ActiveServerChanged += ConfigServiceOnActiveServerChanged;
        }

        /// <summary>
        /// Handles active server change
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ConfigServiceOnActiveServerChanged(object? sender, EventArgs e)
        {
            StateHasChangedDelegate?.Invoke();
        }

        /// <summary>
        /// Handles select new active server.
        /// </summary>
        /// <param name="server"></param>
        /// <returns></returns>
        public async Task OnSelectActiveServer(Uri server)
        {
            await ConfigService.SetActiveServerAsync(server);
            StateHasChangedDelegate?.Invoke();
        }

        /// <summary>
        /// Handles form submit for new server
        /// </summary>
        /// <returns></returns>
        public async Task OnAddNewServerAsync()
        {
            if (NewServer.Uri is not null)
            {
                Uri address = new(NewServer.Uri);
                await ConfigService.AddServerAsync(address);
                StateHasChangedDelegate?.Invoke();
            }
        }
    }

    public class NewServer
    {
        [Required(AllowEmptyStrings = false)]
        public string? Uri { get; set; }
    }

}