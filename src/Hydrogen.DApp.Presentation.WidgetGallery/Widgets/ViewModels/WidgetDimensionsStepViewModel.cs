using System.Threading.Tasks;
using Hydrogen;
using Hydrogen.DApp.Presentation.Components.Wizard;
using Hydrogen.DApp.Presentation.WidgetGallery.Widgets.Models;

namespace Hydrogen.DApp.Presentation.WidgetGallery.Widgets.ViewModels {

    public class WidgetDimensionsStepViewModel : WizardStepViewModelBase<NewWidgetModel> {
        public override Task<Result> OnNextAsync() {
            return Task.FromResult(Result.Valid);
        }

        public override Task<Result> OnPreviousAsync() {
            return Task.FromResult(Result.Valid);
        }
    }
}