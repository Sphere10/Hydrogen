using VelocityNET.Presentation.Hydrogen.ViewModels;
using VelocityNET.Presentation.Hydrogen.WidgetGallery.Widgets.Services;

namespace VelocityNET.Presentation.Hydrogen.WidgetGallery.Widgets.ViewModels
{
    
    public class WidgetGalleryViewModel : ComponentViewModelBase
    {
        public IRandomNumberService NumberService { get; }

        public WidgetGalleryViewModel(IRandomNumberService numberService)
        {
            NumberService = numberService;
        }
    }
}