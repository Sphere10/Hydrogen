using System;
using System.Threading.Tasks;
using FluentValidation;
using FluentValidation.Results;
using Sphere10.Framework;
using VelocityNET.Presentation.Hydrogen.Components.Wizard;
using VelocityNET.Presentation.Hydrogen.WidgetGallery.Extensions;
using VelocityNET.Presentation.Hydrogen.WidgetGallery.Widgets.Components;
using VelocityNET.Presentation.Hydrogen.WidgetGallery.Widgets.Models;

namespace VelocityNET.Presentation.Hydrogen.WidgetGallery.Widgets.ViewModels
{

    public class NewWidgetWizardStepViewModel : WizardStepViewModelBase<NewWidgetModel>
    {
        private IValidator<NewWidgetModel> Validator { get; }

        public NewWidgetWizardStepViewModel(IValidator<NewWidgetModel> validator)
        {
            Validator = validator ?? throw new ArgumentNullException(nameof(validator));
        }

        /// <inheritdoc />
        public override async Task<Result> OnNextAsync()
        {
            ValidationResult result = await Validator.ValidateAsync(Model);

            if (result.IsValid)
            {
                if (Model.AreDimensionsKnown)
                {
                    Wizard.UpdateSteps(StepUpdateType.Inject, new[] {typeof(WidgetDimensionsStep)});
                }
            }

            return result.ToResult();
        }

        /// <inheritdoc />
        public override Task<Result> OnPreviousAsync()
        {
            return Task.FromResult(Result.Valid);
        }
    }

}