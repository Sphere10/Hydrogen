using System.Threading.Tasks;
using VelocityNET.Presentation.Hydrogen.Services;
using VelocityNET.Presentation.Hydrogen.WidgetGallery;

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
           await _modalService.ShowAsync<WidgetModal>();
        }
    }
}