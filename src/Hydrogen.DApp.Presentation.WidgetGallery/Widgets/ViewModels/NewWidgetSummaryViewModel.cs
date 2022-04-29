using System.Threading.Tasks;
using Hydrogen;
using Hydrogen.DApp.Presentation.Components.Wizard;
using Hydrogen.DApp.Presentation.WidgetGallery.Widgets.Models;

namespace Hydrogen.DApp.Presentation.WidgetGallery.Widgets.ViewModels {

    public class NewWidgetSummaryViewModel : WizardStepViewModelBase<NewWidgetModel> {
        /// <inheritdoc />
        public override Task<Result> OnNextAsync() {
            return Task.FromResult(Result.Valid);
        }

        /// <inheritdoc />
        public override Task<Result> OnPreviousAsync() {
            return Task.FromResult(Result.Valid);
        }
    }

}