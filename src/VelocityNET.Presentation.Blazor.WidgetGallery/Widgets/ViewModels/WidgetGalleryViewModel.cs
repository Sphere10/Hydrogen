using VelocityNET.Presentation.Blazor.Shared;
using VelocityNET.Presentation.Blazor.WidgetGallery.Widgets.Services;

namespace VelocityNET.Presentation.Blazor.WidgetGallery.Widgets.ViewModels
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