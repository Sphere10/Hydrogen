using System.Threading.Tasks;
using Sphere10.Framework;
using VelocityNET.Presentation.Hydrogen.ViewModels;
using VelocityNET.Presentation.Hydrogen.WidgetGallery.Widgets.Models;

namespace VelocityNET.Presentation.Hydrogen.WidgetGallery.Widgets.ViewModels
{

    public class NewWidgetSummaryViewModel : WizardStepViewModelBase<NewWidgetModel>
    {
        /// <inheritdoc />
        public override Task<Result> OnNextAsync()
        {
            return Task.FromResult(Result.Valid);
        }

        /// <inheritdoc />
        public override Task<Result> OnPreviousAsync()
        {
            return Task.FromResult(Result.Valid);
        }

        public override Task<Result> ValidateAsync()
        {
            return Task.FromResult(Result.Valid);
        }
    }

}