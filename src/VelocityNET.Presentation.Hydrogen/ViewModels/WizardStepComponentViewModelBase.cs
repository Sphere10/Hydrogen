using System.Threading.Tasks;

namespace VelocityNET.Presentation.Hydrogen.ViewModels
{
    /// <summary>
    /// Wizard step component view model base. Wizard step view models
    /// should extend this class.
    /// </summary>
    public abstract class WizardStepComponentViewModelBase
    {
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
    }
}