using System.Threading.Tasks;
using VelocityNET.Presentation.Hydrogen.Components.Modal;

namespace VelocityNET.Presentation.Hydrogen.ViewModels
{
    public abstract class ModalViewModelBase : ComponentViewModelBase
    {
        protected TaskCompletionSource<ModalResult> ModalTaskCompletionSource { get; } = new ();

        public Task<ModalResult> ShowAsync()
        {
            return ModalTaskCompletionSource.Task;
        }
    }
}