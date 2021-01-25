using System.Threading.Tasks;
using Sphere10.Framework;
using VelocityNET.Presentation.Hydrogen.ViewModels;

namespace VelocityNET.Presentation.Hydrogen.WidgetGallery.Widgets.ViewModels
{

    public class WidgetDimensionsStepViewModel : WizardStepComponentViewModelBase
    {
        public override Task<bool> OnNextAsync()
        {
            return Task.FromResult(true);
        }

        public override Task<bool> OnPreviousAsync()
        {
            return Task.FromResult(true);
        }

        public override Result Validate()
        {
            return Result.Valid;
        }
    }

}