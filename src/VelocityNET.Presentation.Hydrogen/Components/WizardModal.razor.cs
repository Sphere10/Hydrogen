using Microsoft.AspNetCore.Components;

namespace VelocityNET.Presentation.Hydrogen.Components
{

    /// <summary>
    /// Wizard modal - show a wizard component inside a modal dialog.
    /// </summary>
    /// <typeparam name="TWizard"> wizard type</typeparam>
    public partial class WizardModal
    {
        /// <summary>
        /// Gets or sets the wizard render fragment
        /// </summary>
        [Parameter]
        public RenderFragment Wizard { get; set; }

        /// <summary>
        /// Gets or sets the call back the wizard will use when finished to signal the completion of the
        /// modal interaction.
        /// </summary>
        private EventCallback<object> OnFinished { get; set; }
        
        /// <summary>
        /// Gets or sets the on cancelled call back the wizard will use when cancellation is requested.
        /// </summary>
        private EventCallback OnCancelled { get; set; }

        /// <inheritdoc />
        protected override void OnAfterRender(bool firstRender)
        {
            base.OnAfterRender(firstRender);
            
            OnFinished = EventCallback.Factory.Create<object>(ViewModel, o => ViewModel!.OkData(o));
            OnCancelled = EventCallback.Factory.Create(ViewModel, () => ViewModel!.Cancel());
        }
    }
}