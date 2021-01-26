using System.Threading.Tasks;
using Sphere10.Framework;
using VelocityNET.Presentation.Hydrogen.Components.Wizard;

namespace VelocityNET.Presentation.Hydrogen.ViewModels
{
    /// <summary>
    /// Wizard step component view model base. Wizard step view models
    /// should extend this class.
    /// </summary>
    // HS: WizardStep should be merged with this
    public abstract class WizardStepViewModelBase
    {
        //  HS: the model object is only used internally here, never by user. Generic sub-class exposes it, if needed.
        internal object Model { get; set; } = default!;

        /// <summary>
        /// Implement logic when the user requests the next step in the wizard. Returning
        /// true will signal the step is ready to advance. false will prevent the wizard moving to next step.
        /// </summary>
        /// <returns> whether or not to progress</returns>
        public abstract Task<bool> OnNextAsync();

        /// <summary>
        /// Implements logic for this step when a user has requested the previous step in the wizard.
        /// true will signal the step is ready to advance. false will prevent the wizard moving to next step.
        /// </summary>
        /// <returns> whether or not to progress</returns>
        public abstract Task<bool> OnPreviousAsync();

        /// <summary>
        /// Validate the model at this step of the wizard.
        /// </summary>
        /// <returns> validation result.</returns>
        public abstract Task<Result> Validate();
    }

    public abstract class WizardStepViewModelBase<TModel> : WizardStepViewModelBase {

        /// <summary>
        /// Gets or sets the model
        /// </summary>
        public new TModel Model {
            get => (TModel)base.Model;
            set => base.Model = value!;
        } 
     
    }
}