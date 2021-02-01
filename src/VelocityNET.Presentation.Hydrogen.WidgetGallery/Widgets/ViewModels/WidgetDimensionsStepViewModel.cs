using System.Threading.Tasks;
using Sphere10.Framework;
using VelocityNET.Presentation.Hydrogen.Components.Wizard;
using VelocityNET.Presentation.Hydrogen.WidgetGallery.Widgets.Models;

namespace VelocityNET.Presentation.Hydrogen.WidgetGallery.Widgets.ViewModels
{

    public class WidgetDimensionsStepViewModel : WizardStepViewModelBase<NewWidgetModel>
    {
        public override Task<Result> OnNextAsync()
        {
            return Task.FromResult(Result.Valid);
        }

        public override Task<Result> OnPreviousAsync()
        {
            return Task.FromResult(Result.Valid);
        }
    }
}