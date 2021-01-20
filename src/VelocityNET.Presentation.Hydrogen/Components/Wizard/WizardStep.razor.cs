using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
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
        
        /// <inheritdoc />
        public override Task<bool> OnNextAsync() => ViewModel!.OnNextAsync();
        
        /// <inheritdoc />
        public override Task<bool> OnPreviousAsync() => ViewModel!.OnPreviousAsync();
    }

    /// <summary>
    /// Non generic wizard step component base.
    /// </summary>
    public abstract class WizardStepBase : ComponentBase
    {
        /// <summary>
        /// Called when the wizard requests the next step. Returning true will allow
        /// the wizard to progress. delegates to view model implementation
        /// </summary>
        /// <returns> whether or not the step is finished and to move next</returns>
        public abstract Task<bool> OnNextAsync();

        /// <summary>
        /// Called when the wizard requests the prev step. Returning true will allow
        /// the wizard to progress. delegates to view model implementation
        /// </summary>
        /// <returns> whether or not the step is finished and to move prev</returns>
        public abstract Task<bool> OnPreviousAsync();
    }

}