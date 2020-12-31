using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using VelocityNET.Presentation.Blazor.Shared.ViewModels;

namespace VelocityNET.Presentation.Blazor.Shared.Components
{
    /// <summary>
    /// Extends <see cref="ComponentBase"/> adding common functionality and view model support.
    /// </summary>
    /// <typeparam name="TViewModel"></typeparam>
    public abstract class ComponentWithViewModel<TViewModel> : ComponentBase where TViewModel : ComponentViewModelBase
    {
        [Inject]
        // ReSharper disable once UnassignedGetOnlyAutoProperty -- get from .razor components
        // ReSharper disable once UnusedAutoPropertyAccessor.Global -- set via blazor inject pipeline
        protected TViewModel ViewModel { get; set; }

        /// <summary>
        /// Method invoked when the component is ready to start, having received its
        /// initial parameters from its parent in the render tree.
        /// Override this method if you will perform an asynchronous operation and
        /// want the component to refresh when that operation is completed.
        /// </summary>
        /// <returns>A <see cref="T:System.Threading.Tasks.Task" /> representing any asynchronous operation.</returns>
        protected override Task OnInitializedAsync()
        {
            ViewModel.InitAsync();
            ViewModel.StateHasChangedDelegate = StateHasChanged;
            return base.OnInitializedAsync();
        }
    }
}