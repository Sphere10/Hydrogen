using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VelocityNET.Presentation.Hydrogen.Components.Modal;
using VelocityNET.Presentation.Hydrogen.Models;
using VelocityNET.Presentation.Hydrogen.Services;
using VelocityNET.Presentation.Hydrogen.ViewModels;

namespace VelocityNET.Presentation.Hydrogen.WidgetGallery.Widgets.ViewModels
{

    public class PagingTableViewModel : ComponentViewModelBase
    {
        public INodeService NodeService { get; }

        private IModalService ModalService { get; }

        public PagingTableViewModel(IModalService modalService, INodeService nodeService)
        {
            ModalService = modalService ?? throw new ArgumentNullException(nameof(modalService));
            NodeService = nodeService ?? throw new ArgumentNullException(nameof(nodeService));
        }

        public async Task OnClickRowAsync(Block block)
        {
            await ModalService.ShowAsync<InfoDialog>(new Dictionary<string, object>
            {
                {nameof(InfoDialog.Title), $"Block {block.Number}"},
                {nameof(InfoDialog.Message), $"Viewing {block.Number}, Address {block.Address}"},
            });
        }
    }
}