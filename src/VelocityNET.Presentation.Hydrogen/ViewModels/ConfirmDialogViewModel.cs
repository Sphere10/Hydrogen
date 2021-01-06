using VelocityNET.Presentation.Hydrogen.Components.Modal;

namespace VelocityNET.Presentation.Hydrogen.ViewModels
{
    public class ConfirmDialogViewModel : ModalViewModelBase
    {
        public void Ok()
        {
            ModalTaskCompletionSource.SetResult(new ModalResult<string>("baz"));
        }

        public void Cancel()
        {
            ModalTaskCompletionSource.SetResult(new ModalResult<string>("foo"));
        }
    }
}