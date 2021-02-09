using System;
using System.Threading.Tasks;
using Sphere10.Hydrogen.Presentation.Services;
using Sphere10.Hydrogen.Presentation.ViewModels;

namespace Sphere10.Hydrogen.Presentation.WidgetGallery.Widgets.ViewModels {

    public class TablesViewModel : ExtendedComponentViewModel {
        public INodeService NodeService { get; }

        public TablesViewModel(INodeService nodeService, IEndpointManager endpointManager) : base(endpointManager) {
            NodeService = nodeService ?? throw new ArgumentNullException(nameof(nodeService));
        }

        protected override async Task InitCoreAsync() {
            await Task.Delay(3000);
        }
    }
}