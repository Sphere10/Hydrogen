using System.Threading.Tasks;
using Sphere10.Framework;
using VelocityNET.Presentation.Hydrogen.ViewModels;
using VelocityNET.Presentation.Hydrogen.WidgetGallery.Widgets.Models;

namespace VelocityNET.Presentation.Hydrogen.WidgetGallery.Widgets.ViewModels
{

    public class WidgetDimensionsStepViewModel : WizardStepViewModelBase<NewWidgetModel> {
        public override Task<bool> OnNextAsync()
        {
            return Task.FromResult(true);
        }

        public override Task<bool> OnPreviousAsync()
        {
            return Task.FromResult(true);
        }

        public override async Task<Result> Validate()
        {
            return Result.Valid;
        }
    }

}