using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using VelocityNET.Presentation.Hydrogen.ViewModels;

namespace VelocityNET.Presentation.Hydrogen.Components
{
    /// <summary>
    /// Modal component. Modals should extend this component with custom
    /// content to be displayed in the modal.
    /// </summary>
    public abstract class ModalComponent<TViewModel> : ModalComponent where TViewModel : IModalViewModel
    {
        [Inject]
        public TViewModel ViewModel { get; set; }

        public override Task<ModalResult> ShowAsync()
        {
            return ViewModel.ShowAsync();
        }
    }

    public abstract class ModalComponent : ComponentBase
    {
        public abstract Task<ModalResult> ShowAsync();
    }
}