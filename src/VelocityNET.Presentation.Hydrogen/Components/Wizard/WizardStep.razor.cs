using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
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
        where TViewModel : WizardStepViewModelBase<TModel>
    {
        /// <summary>
        /// Gets or sets the step view model
        /// </summary>
        [Inject]
        // ReSharper disable once AutoPropertyCanBeMadeGetOnly.Global
        public TViewModel ViewModel { get; set; } = null!;

        /// <summary>
        /// Gets or sets the wizard instance
        /// </summary>
        [Parameter]
        public IWizard<TModel> Wizard
        {
            get => ViewModel!.Wizard;
            set => ViewModel!.Wizard = value;
        }

        /// <inheritdoc />
        public override Task<Result> OnNextAsync() => ViewModel!.OnNextAsync();
        
        /// <inheritdoc />
        public override Task<Result> OnPreviousAsync() => ViewModel!.OnPreviousAsync();
        
        /// <inheritdoc />
        public override Task<Result> ValidateAsync() => ViewModel!.ValidateAsync();

        /// <inheritdoc />
        protected override void OnParametersSet()
        {
            if (Wizard is null)
            {
                throw new InvalidOperationException("Wizard step requires wizard parameter be set.");
            }

            base.OnParametersSet();
        }
    }

    /// <summary>
    /// Non generic wizard step component base.
    /// </summary>
    public abstract class WizardStepBase : ComponentBase
    {
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
        /// Validate this the model at this step.  
        /// </summary>
        /// <returns> validation results.</returns>
        public abstract Task<Result> ValidateAsync();
        
        /// <summary>
        /// Called when the wizard requests the next step. Returning true will allow
        /// the wizard to progress.
        /// </summary>
        /// <returns> whether or not the step is finished and to move next</returns>
        public abstract Task<Result> OnNextAsync();

        /// <summary>
        /// Called when the wizard requests the prev step. Returning true will allow
        /// the wizard to progress.
        /// </summary>
        /// <returns> whether or not the step is finished and to move prev</returns>
        public abstract Task<Result> OnPreviousAsync();
    }
}