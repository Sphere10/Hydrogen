using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VelocityNET.Presentation.Hydrogen.Components;
using VelocityNET.Presentation.Hydrogen.Components.Modal;
using VelocityNET.Presentation.Hydrogen.Services;

namespace VelocityNET.Presentation.Hydrogen.Loader.ViewModels
{
    public class HomeViewModel
    {
        private readonly IModalService _modalService;

        public HomeViewModel(IModalService modalService)
        {
            _modalService = modalService;
        }

        public async Task ShowModalAsync()
        {
           ModalResult result = await _modalService.ShowAsync<ConfirmDialog>(new Dictionary<string,object>
           {
               {nameof(ConfirmDialog.Title), "A confirmation dialog"},
               {nameof(ConfirmDialog.Message), "Its working"},
               {nameof(ConfirmDialog.ConfirmMessageText), "Ok"},
               {nameof(ConfirmDialog.CancelMessageText), "Cancel"},
           });

           var data = result as ModalResult<string>;
           Console.WriteLine(data);
        }
    }
}