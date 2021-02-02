using System;
using Microsoft.AspNetCore.Components;
using VelocityNET.Presentation.Hydrogen.Components.Wizard;

namespace VelocityNET.Presentation.Hydrogen.Components.Modal
{

    /// <summary>
    /// Wizard modal - show a wizard component inside a modal dialog.
    /// </summary>
    public partial class WizardModal
    {
        private WizardHost? _host;

        /// <summary>
        /// Gets or sets the wizard render fragment
        /// </summary>
        [Parameter]
        public IWizard Wizard
        {
            get => ViewModel!.Wizard;
            set => ViewModel!.Wizard = value;
        }
        
        /// <summary>
        /// Gets or sets the call back the wizard will use when finished to signal the completion of the
        /// modal interaction.
        /// </summary>
        private EventCallback OnFinished { get; set; }

        /// <summary>
        /// Gets or sets the on cancelled call back the wizard will use when cancellation is requested.
        /// </summary>
        private EventCallback OnCancelled { get; set; }
        
        /// <summary>
        /// Gets or sets the event callback passed to child components to notify the wizard modal
        /// of step change.
        /// </summary>
        private EventCallback OnStepChange { get; set; }
        

        /// <inheritdoc />
        protected override void OnParametersSet()
        {
            base.OnParametersSet();

            if (Wizard is null)
            {
                throw new InvalidOperationException("Wizard parameter is required");
            }

            OnFinished = EventCallback.Factory.Create(ViewModel!, ViewModel!.Ok);
            OnCancelled = EventCallback.Factory.Create(ViewModel!, ViewModel!.Cancel);
            OnStepChange = EventCallback.Factory.Create(this, StateHasChanged);
        }
    }

}