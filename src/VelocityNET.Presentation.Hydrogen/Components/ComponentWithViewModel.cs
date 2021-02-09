using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using VelocityNET.Presentation.Hydrogen.ViewModels;

namespace VelocityNET.Presentation.Hydrogen.Components {
    /// <summary>
    /// Extends <see cref="ComponentBase"/> adding common functionality and view model support.
    /// </summary>
    /// <typeparam name="TViewModel"></typeparam>
    public abstract class ComponentWithViewModel<TViewModel> : ComponentBase where TViewModel : ComponentViewModelBase {
        /// <summary>
        /// Gets the view model for this component
        /// </summary>
        [Inject]
        // ReSharper disable once AutoPropertyCanBeMadeGetOnly.Global
        public TViewModel? ViewModel { get; set; } = null!;

        /// <summary>
        /// Method invoked when the component is ready to start, having received its
        /// initial parameters from its parent in the render tree.
        /// Override this method if you will perform an asynchronous operation and
        /// want the component to refresh when that operation is completed.
        /// </summary>
        /// <returns>A <see cref="T:System.Threading.Tasks.Task" /> representing any asynchronous operation.</returns>
        protected override async Task OnInitializedAsync() {
            if (ViewModel is null) {
                throw new InvalidOperationException("View model has not been injected successfully and is null");
            } else {
                await ViewModel.InitAsync();
                ViewModel.StateHasChangedDelegate = StateHasChanged;
                await base.OnInitializedAsync();
            }
        }
    }
}