using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using VelocityNET.Presentation.Hydrogen.Components;
using VelocityNET.Presentation.Hydrogen.Services;
using VelocityNET.Presentation.Hydrogen.ViewModels;

namespace VelocityNET.Presentation.Hydrogen.Loader.ViewModels
{
    /// <summary>
    /// Data source view modal base
    /// </summary>
    public class DataSourceViewModel : ComponentViewModelBase
    {
        public DataSourceViewModel(IModalService modalService)
        {
            ModalService = modalService ?? throw new ArgumentNullException(nameof(modalService));
        }

        // ReSharper disable once AutoPropertyCanBeMadeGetOnly.Local
        private IModalService ModalService { get; set; }

        public async Task ShowDataSourceModalAsync()
        {
            await ModalService.ShowAsync<InfoDialog>();
        }
    }
}