using System;
using System.Threading.Tasks;
using FluentValidation;
using Sphere10.Framework;
using VelocityNET.Presentation.Hydrogen.Components.Wizard;
using VelocityNET.Presentation.Hydrogen.ViewModels;
using VelocityNET.Presentation.Hydrogen.WidgetGallery.Extensions;
using VelocityNET.Presentation.Hydrogen.WidgetGallery.Widgets.Components;
using VelocityNET.Presentation.Hydrogen.WidgetGallery.Widgets.Models;

namespace VelocityNET.Presentation.Hydrogen.WidgetGallery.Widgets.ViewModels
{

    public class NewWidgetWizardStepViewModel : WizardStepViewModelBase<NewWidgetModel>
    {
        private IValidator<NewWidgetModel> Validator { get; }
        
        public IWizard Wizard { get; set; }

        public NewWidgetWizardStepViewModel(IValidator<NewWidgetModel> validator)
        {
            Validator = validator ?? throw new ArgumentNullException(nameof(validator));
        }

        /// <inheritdoc />
        public override async Task<Result> OnNextAsync()
        {
            Result result = await ValidateAsync();
            
            if (result.Success)
            {
                if (Model.AreDimensionsKnown)
                {
                    Wizard.UpdateSteps(StepUpdateType.Inject, typeof(WidgetDimensionsStep));
                }
            }
            
            return result;
        }

        /// <inheritdoc />
        public override Task<Result> OnPreviousAsync()
        {
            return Task.FromResult(Result.Valid);
        }

        /// <summary>
        /// Validate the model at this step of the wizard.
        /// </summary>
        /// <returns> validation result.</returns>
        public override Task<Result> ValidateAsync() => Task.FromResult(Validator.Validate(Model).ToResult());
    }
}