using System;
using System.Threading.Tasks;
using FluentValidation;
using FluentValidation.Results;
using Sphere10.Framework;
using VelocityNET.Presentation.Hydrogen.ViewModels;
using VelocityNET.Presentation.Hydrogen.WidgetGallery.Widgets.Models;

namespace VelocityNET.Presentation.Hydrogen.WidgetGallery.Widgets.ViewModels
{

    public class NewWidgetWizardStepViewModel : WizardStepComponentViewModelBase
    {
        private IValidator<NewWidgetModel> Validator { get; }

        public NewWidgetWizardStepViewModel(IValidator<NewWidgetModel> validator)
        {
            Validator = validator ?? throw new ArgumentNullException(nameof(validator));
        }

        /// <inheritdoc />
        public override Task<bool> OnNextAsync()
        {
            return Task.FromResult(true);
        }

        /// <inheritdoc />
        public override Task<bool> OnPreviousAsync()
        {
            return Task.FromResult(true);
        }

        /// <summary>
        /// Validate the model at this step of the wizard.
        /// </summary>
        /// <returns> validation result.</returns>
        public override Result Validate()
        {
            ValidationResult validationResult = Validator.Validate(Model as NewWidgetModel);

            if (validationResult.IsValid)
            {
                return Result.Valid;
            }
            else
            {
                var result = new Result();
                foreach (var error in validationResult.Errors)
                {
                    result.AddError(error.ErrorMessage);
                }
                
                return result;
            }
        }
    }
}