using System;
using System.Threading.Tasks;
using FluentValidation;
using FluentValidation.Results;
using Sphere10.Framework;
using Sphere10.Hydrogen.Presentation.Components.Wizard;
using Sphere10.Hydrogen.Presentation.WidgetGallery.Extensions;
using Sphere10.Hydrogen.Presentation.WidgetGallery.Widgets.Components;
using Sphere10.Hydrogen.Presentation.WidgetGallery.Widgets.Models;

namespace Sphere10.Hydrogen.Presentation.WidgetGallery.Widgets.ViewModels {

    public class NewWidgetWizardStepViewModel : WizardStepViewModelBase<NewWidgetModel> {
        private IValidator<NewWidgetModel> Validator { get; }

        public NewWidgetWizardStepViewModel(IValidator<NewWidgetModel> validator) {
            Validator = validator ?? throw new ArgumentNullException(nameof(validator));
        }

        /// <inheritdoc />
        public override async Task<Result> OnNextAsync() {
            ValidationResult result = await Validator.ValidateAsync(Model);

            if (result.IsValid) {
                if (Model.AreDimensionsKnown) {
                    Wizard.UpdateSteps(StepUpdateType.Inject, new[] { typeof(WidgetDimensionsStep) });
                }
            }

            return result.ToResult();
        }

        /// <inheritdoc />
        public override Task<Result> OnPreviousAsync() {
            return Task.FromResult(Result.Valid);
        }
    }

}