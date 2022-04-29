using Sphere10.Hydrogen.Presentation.ViewModels;
using Sphere10.Hydrogen.Presentation.WidgetGallery.Widgets.Services;

namespace Sphere10.Hydrogen.Presentation.WidgetGallery.Widgets.ViewModels {

    public class WidgetGalleryViewModel : ComponentViewModelBase {
        public IRandomNumberService NumberService { get; }

        public WidgetGalleryViewModel(IRandomNumberService numberService) {
            NumberService = numberService;
        }
    }
}