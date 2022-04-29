using System;
using System.Data.SqlTypes;
using System.Threading.Tasks;
using Hydrogen;
using Hydrogen.DApp.Presentation.Components.Modal;

namespace Hydrogen.DApp.Presentation.ViewModels {
    /// <summary>
    /// Basic modal view model providing base functionality.
    /// </summary>
    public class ModalViewModel : ComponentViewModelBase {
        /// <summary>
        /// Gets the modal completion source. This is used to signal completion of operations
        /// with the modal.
        /// </summary>
        protected TaskCompletionSource<ModalResult> ModalTaskCompletionSource { get; } = new();

        /// <summary>
        /// Show the modal, returning a task that once finished signals modal completion with result.
        /// </summary>
        /// <returns> result of modal interaction</returns>
        public Task<ModalResult> ShowAsync() {
            return ModalTaskCompletionSource.Task;
        }

        /// <summary>
        /// Modal interactions completed with OK result.
        /// </summary>
        /// <returns></returns>
        public Task Ok() {
            ModalTaskCompletionSource.SetResult(ModalResult.Ok);
            return Task.CompletedTask;
        }

        /// <summary>
        /// Modal interactions completed with OK result.
        /// </summary>
        /// <returns></returns>
        public Task OkData<T>(T data) {
            ModalTaskCompletionSource.SetResult(ModalResult.OkData(data ?? throw new ArgumentNullException(nameof(data))));
            return Task.CompletedTask;
        }

        /// <summary>
        /// Cancel result.
        /// </summary>
        public void Cancel() {
            ModalTaskCompletionSource.SetResult(ModalResult.Cancel);
        }

        /// <summary>
        /// Modal closed result.
        /// </summary>
        public virtual Task<bool> RequestCloseAsync() {
            if (!ModalTaskCompletionSource.Task.IsCompleted) {
                ModalTaskCompletionSource.SetResult(ModalResult.Exit);
            }

            return Task.FromResult(true);
        }
    }
}