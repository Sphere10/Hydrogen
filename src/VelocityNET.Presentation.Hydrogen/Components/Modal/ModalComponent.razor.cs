using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using VelocityNET.Presentation.Hydrogen.ViewModels;

namespace VelocityNET.Presentation.Hydrogen.Components.Modal
{
    /// <summary>
    /// Modal component. Modals should extend this component with custom
    /// content to be displayed in the modal.
    /// </summary>
    public abstract partial class ModalComponent<TViewModel> where TViewModel : ModalViewModelBase
    {
        [Inject] 
        // ReSharper disable once MemberCanBePrivate.Global
        // ReSharper disable once UnusedAutoPropertyAccessor.Global
        protected TViewModel? ViewModel { get; set; }

        public override Task<ModalResult> ShowAsync() => ViewModel!.ShowAsync();
    }
    
    public abstract class ModalComponentBase : ComponentBase
    {
        public abstract Task<ModalResult> ShowAsync();
    }
}