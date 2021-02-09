using System.Threading.Tasks;
using Sphere10.Framework;
using Sphere10.Hydrogen.Presentation.Components.Wizard;
using Sphere10.Hydrogen.Presentation.WidgetGallery.Widgets.Models;

namespace Sphere10.Hydrogen.Presentation.WidgetGallery.Widgets.ViewModels {

    public class WidgetDimensionsStepViewModel : WizardStepViewModelBase<NewWidgetModel> {
        public override Task<Result> OnNextAsync() {
            return Task.FromResult(Result.Valid);
        }

        public override Task<Result> OnPreviousAsync() {
            return Task.FromResult(Result.Valid);
        }
    }
}