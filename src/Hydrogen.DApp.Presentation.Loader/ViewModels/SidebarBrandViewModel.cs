using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Hydrogen.DApp.Presentation.Services;
using Hydrogen.DApp.Presentation.ViewModels;

namespace Hydrogen.DApp.Presentation.Loader.ViewModels
{

    public class SidebarBrandViewModel : ComponentViewModelBase, IDisposable
    {
        private IEndpointManager EndpointManager { get; }

        public Uri Endpoint => EndpointManager.Endpoint;

        public IEnumerable<Uri> Endpoints => EndpointManager.Endpoints;

        public SidebarBrandViewModel(IEndpointManager endpointManager)
        {
            EndpointManager = endpointManager ?? throw new ArgumentNullException(nameof(endpointManager));
            EndpointManager.EndpointChanged += EndpointManagerOnEvent;
            EndpointManager.EndpointAdded += EndpointManagerOnEvent;
        }

        private void EndpointManagerOnEvent(object? sender, EventArgs e)
        {
            StateHasChangedDelegate?.Invoke();
        }

        public async Task OnSelectEndpointAsync(Uri server) => await EndpointManager.SetCurrentEndpointAsync(server);

        public void Dispose()
        {
            EndpointManager.EndpointChanged -= EndpointManagerOnEvent;
            EndpointManager.EndpointAdded -= EndpointManagerOnEvent;
        }
    }

}