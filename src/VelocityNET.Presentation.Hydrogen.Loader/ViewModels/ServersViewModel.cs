using System;
using System.Collections.Generic;
using VelocityNET.Presentation.Hydrogen.Models;
using VelocityNET.Presentation.Hydrogen.Services;
using VelocityNET.Presentation.Hydrogen.ViewModels;

namespace VelocityNET.Presentation.Hydrogen.Loader.ViewModels
{
    public class ServersViewModel : ComponentViewModelBase
    {
        public IEnumerable<Server> Servers => ConfigService.AvailableServers;
        private IServerConfigService ConfigService { get; }

        public ServersViewModel(IServerConfigService configService)
        {
            ConfigService = configService ?? throw new ArgumentNullException(nameof(configService));
        }
    }
}