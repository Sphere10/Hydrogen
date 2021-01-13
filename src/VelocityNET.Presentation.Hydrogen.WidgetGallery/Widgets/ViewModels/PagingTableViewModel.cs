using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using VelocityNET.Presentation.Hydrogen.Components;
using VelocityNET.Presentation.Hydrogen.Models;
using VelocityNET.Presentation.Hydrogen.Services;
using VelocityNET.Presentation.Hydrogen.ViewModels;

namespace VelocityNET.Presentation.Hydrogen.WidgetGallery.Widgets.ViewModels
{
    public class PagingTableViewModel : ComponentViewModelBase
    {
        [Inject]
        public INodeService NodeService { get; set; }
        
        [Inject]
        public IModalService ModalService { get; set; }
        
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