using System;
using Microsoft.AspNetCore.Components;
namespace VelocityNET.Presentation.Hydrogen.Components.Wizard
{
    /// <summary>
    /// Wizard component.
    /// </summary>
    // HS: almost all of this should be merged into WizardViewModel<TModel>
    public partial class WizardHost
    {
        /// <summary>
        /// Call back, invoked when wizard is finished. cascaded from a parent component is used to signal
        /// the completion of the wizard.
        /// </summary>
        [CascadingParameter(Name = "OnFinished")]
        public EventCallback OnFinished
        {
            get => ViewModel!.OnFinished;
            set => ViewModel!.OnFinished = value;
        }

        /// <summary>
        /// Call back, invoked when wizard is cancelled. cascaded from a parent component is used to signal
        /// the cancellation of the wizard.
        /// </summary>
        [CascadingParameter(Name = "OnCancelled")]
        public EventCallback OnCancelled
        {
            get => ViewModel!.OnCancelled;
            set => ViewModel!.OnCancelled = value;
        }
        
        /// <summary>
        /// Gets or sets the wizard model instance.
        /// </summary>
        [CascadingParameter]
        public IWizard Wizard
        {
            get => ViewModel!.Wizard;
            set => ViewModel!.Wizard = value;
        }

        /// <inheritdoc />
        protected override void OnParametersSet()
        {
            base.OnParametersSet();

            if (Wizard is null)
            {
                throw new InvalidOperationException("Wizard parameter is required.");
            }
        }
    }
}