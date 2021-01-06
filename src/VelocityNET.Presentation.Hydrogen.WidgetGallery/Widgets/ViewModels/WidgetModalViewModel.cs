using System;
using System.Threading.Tasks;
using VelocityNET.Presentation.Hydrogen.Components;
using VelocityNET.Presentation.Hydrogen.ViewModels;

namespace VelocityNET.Presentation.Hydrogen.WidgetGallery.Widgets.ViewModels
{

    public class WidgetModalViewModel : IModalViewModel
    {
        public Task<ModalResult> ShowAsync()
        {
            return Task.FromResult(new ModalResult());
        }
    }

}