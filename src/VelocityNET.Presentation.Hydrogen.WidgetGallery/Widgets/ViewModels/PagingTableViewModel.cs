using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using VelocityNET.Presentation.Hydrogen.Components;
using VelocityNET.Presentation.Hydrogen.Models;
using VelocityNET.Presentation.Hydrogen.Services;
using VelocityNET.Presentation.Hydrogen.ViewModels;

namespace VelocityNET.Presentation.Hydrogen.WidgetGallery.Widgets.ViewModels
{

    public class PagingTableViewModel : ComponentViewModelBase, IDisposable
    {
        public INodeService NodeService { get; }

        public IModalService ModalService { get; }

        public Queue<Block> Blocks { get; } = new(10);

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

        private CancellationTokenSource TaskCancellationSource { get; } = new();

        /// <summary>
        /// Called when view is initialized, override to provide custom initialization logic. 
        /// </summary>
        /// <returns></returns>
        protected override Task InitCoreAsync()
        {
            return Task.CompletedTask;
        }

        /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
        public void Dispose()
        {
            TaskCancellationSource.Cancel();
        }
    }
}