using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using VelocityNET.Presentation.Hydrogen.ViewModels;

namespace VelocityNET.Presentation.Hydrogen.Components.Modal
{
    /// <summary>
    /// Modal component. Modals should extend this component with custom
    /// content to be displayed in the modal.
    /// </summary>
    public abstract partial class ModalComponent<TViewModel> where TViewModel : ModalViewModel
    {
        /// <summary>
        /// Gets or sets the view model
        /// </summary>
        [Inject]
        // ReSharper disable once MemberCanBePrivate.Global
        // ReSharper disable once UnusedAutoPropertyAccessor.Global
        protected TViewModel? ViewModel { get; set; } = null!;

        /// <inheritdoc />
        public override Task<ModalResult> ShowAsync() => ViewModel!.ShowAsync();

        /// <inheritdoc />
        public override void OnClose() => ViewModel!.Closed();
    }
    
    /// <summary>
    /// Modal component. Modals should extend this component with custom
    /// content to be displayed in the modal.
    /// </summary>
    public abstract class ModalComponentBase : ComponentBase
    {
        /// <summary>
        /// render completion source. Used to signal the modal component
        /// has been rendered for waiting observers.
        /// </summary>
        private readonly TaskCompletionSource _renderCompletionSource = new ();
        
        /// <summary>
        /// Gets the task indicating whether the component has been rendered / initialized.
        /// </summary>
        public Task ModalRendered => _renderCompletionSource.Task;
        
        /// <summary>
        /// Show the modal content. returns the result of modal interaction with the user.
        /// </summary>
        /// <returns> a task that is complete once the modal interaction is finished</returns>
        public abstract Task<ModalResult> ShowAsync();

        /// <summary>
        /// Handles the close event, such as when the user clicks away.
        /// </summary>
        public abstract void OnClose();

        /// <inheritdoc />
        protected override Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                _renderCompletionSource.SetResult();
            }
            
            return base.OnAfterRenderAsync(firstRender);
        }
    }
}