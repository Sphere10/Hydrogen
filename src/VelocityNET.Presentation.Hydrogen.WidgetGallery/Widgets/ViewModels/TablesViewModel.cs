using System;
using System.Threading.Tasks;
using VelocityNET.Presentation.Hydrogen.Services;
using VelocityNET.Presentation.Hydrogen.ViewModels;

namespace VelocityNET.Presentation.Hydrogen.WidgetGallery.Widgets.ViewModels
{

    public class TablesViewModel : ExtendedComponentViewModel
    {
        public INodeService NodeService { get; }

        public TablesViewModel(INodeService nodeService, IEndpointManager endpointManager) : base(endpointManager)
        {
            NodeService = nodeService ?? throw new ArgumentNullException(nameof(nodeService));
        }

        protected override async Task InitCoreAsync()
        {
            await Task.Delay(3000);
        }
    }
}