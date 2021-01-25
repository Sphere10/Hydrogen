using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Sphere10.Framework;
using VelocityNET.Presentation.Hydrogen.ViewModels;

namespace VelocityNET.Presentation.Hydrogen.Components.Wizard
{

    /// <summary>
    /// Wizard step component base. 
    /// </summary>
    /// <typeparam name="TModel"> model type</typeparam>
    /// <typeparam name="TViewModel"> view model type</typeparam>
    public abstract partial class WizardStep<TModel, TViewModel>
        where TViewModel : WizardStepComponentViewModelBase
    {
        /// <summary>
        /// Gets or sets the step view model
        /// </summary>
        [Inject]
        // ReSharper disable once AutoPropertyCanBeMadeGetOnly.Global
        public TViewModel ViewModel { get; set; } = null!;

        /// <summary>
        /// Gets or sets the model
        /// </summary>
        [Parameter]
        public TModel? Model { get; set; }
        
        /// <summary>
        /// Gets or sets the wizard model edit context
        /// </summary>
        [CascadingParameter]
        public EditContext? EditContext { get; set; }
        
        /// <inheritdoc />
        public override Task<bool> OnNextAsync() => ViewModel!.OnNextAsync();
        
        /// <inheritdoc />
        public override Task<bool> OnPreviousAsync() => ViewModel!.OnPreviousAsync();
        
        /// <inheritdoc />
        public override Result Validate() => ViewModel!.Validate();

        /// <inheritdoc />
        protected override void OnParametersSet()
        {
            ViewModel!.Model = Model;
            base.OnParametersSet();
        }
    }

    /// <summary>
    /// Non generic wizard step component base.
    /// </summary>
    public abstract class WizardStepBase : ComponentBase
    {
        /// <summary>
        /// Validate this the model at this step.  
        /// </summary>
        /// <returns> validation results.</returns>
        public abstract Result Validate();
        
        /// <summary>
        /// Called when the wizard requests the next step. Returning true will allow
        /// the wizard to progress.
        /// </summary>
        /// <returns> whether or not the step is finished and to move next</returns>
        public abstract Task<bool> OnNextAsync();

        /// <summary>
        /// Called when the wizard requests the prev step. Returning true will allow
        /// the wizard to progress.
        /// </summary>
        /// <returns> whether or not the step is finished and to move prev</returns>
        public abstract Task<bool> OnPreviousAsync();
        
        /// <summary>
        /// Gets or sets the title of the wizard step.
        /// </summary>
        public abstract string Title { get; }

        /// <summary>
        /// Gets the next button text for this step.
        /// </summary>
        public virtual string NextButtonText { get; } = "Next";

        /// <summary>
        /// Gets the back button text for this step.
        /// </summary>
        public virtual string BackButtonText { get; } = "Back";

        /// <summary>
        /// Gets the cancel button text for this step.
        /// </summary>
        public virtual string CancelButtonText { get; } = "Cancel";

        /// <summary>
        /// Gets the finish button text.
        /// </summary>
        public virtual string FinishButtonText { get; } = "Finish";

        /// <summary>
        /// Gets a value indicating whether the wizard / step may be cancelled.
        /// </summary>
        public virtual bool IsCancellable { get; } = true;
        
        /// <summary>
        /// Gets or sets the next step, settable by this step. This is used to branch
        /// the wizard and insert a step after this one.
        /// </summary>
        public virtual Type? NextStep { get; set; }

        /// <summary>
        /// Gets a value indicating whether this step will provide the next step in the wizard.
        /// </summary>
        public bool HasNextStep => NextStep is not null;
    }

}