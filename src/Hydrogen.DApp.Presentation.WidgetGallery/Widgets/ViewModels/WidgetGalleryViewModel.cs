using Hydrogen.DApp.Presentation.ViewModels;
using Hydrogen.DApp.Presentation.WidgetGallery.Widgets.Services;

namespace Hydrogen.DApp.Presentation.WidgetGallery.Widgets.ViewModels {

    public class WidgetGalleryViewModel : ComponentViewModelBase {
        public IRandomNumberService NumberService { get; }

        public WidgetGalleryViewModel(IRandomNumberService numberService) {
            NumberService = numberService;
        }
    }
}