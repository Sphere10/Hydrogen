using System.Threading.Tasks;
using Sphere10.Framework;
using VelocityNET.Presentation.Hydrogen.Components.Wizard;

namespace VelocityNET.Presentation.Hydrogen.ViewModels
{

    public abstract class WizardStepViewModelBase<TModel> : ComponentViewModelBase
    {
        /// <summary>
        /// Gets or sets the wizard instance
        /// </summary>
        public IWizard<TModel> Wizard { get; set; }

        /// <summary>
        /// Gets the model.
        /// </summary>
        public TModel Model => Wizard.Model;
        
        /// <summary>
        /// Implement logic when the user requests the next step in the wizard. Returning
        /// true will signal the step is ready to advance. false will prevent the wizard moving to next step.
        /// </summary>
        /// <returns> whether or not to progress</returns>
        public abstract Task<Result> OnNextAsync();

        /// <summary>
        /// Implements logic for this step when a user has requested the previous step in the wizard.
        /// true will signal the step is ready to advance. false will prevent the wizard moving to next step.
        /// </summary>
        /// <returns> whether or not to progress</returns>
        public abstract Task<Result> OnPreviousAsync();

        /// <summary>
        /// Validate the model at this step of the wizard.
        /// </summary>
        /// <returns> validation result.</returns>
        public abstract Task<Result> ValidateAsync();
    }
}