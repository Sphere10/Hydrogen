﻿using System.Threading.Tasks;
using VelocityNET.Presentation.Hydrogen.Components.Modal;

namespace VelocityNET.Presentation.Hydrogen.ViewModels
{
/// <summary>
/// Basic modal view model providing base functionality.
/// </summary>
    public class ModalViewModel : ComponentViewModelBase
    {
        /// <summary>
        /// Gets the modal completion source. This is used to signal completion of operations
        /// with the modal.
        /// </summary>
        protected TaskCompletionSource<ModalResult> ModalTaskCompletionSource { get; } = new();

        /// <summary>
        /// Show the modal, returning a task that once finished signals modal completion with result.
        /// </summary>
        /// <returns> result of modal interaction</returns>
        public Task<ModalResult> ShowAsync()
        {
            return ModalTaskCompletionSource.Task;
        }
        
        /// <summary>
        /// Modal interactions completed with OK result.
        /// </summary>
        /// <returns></returns>
        public Task Ok()
        {
            ModalTaskCompletionSource.SetResult(ModalResult.Ok);
            return Task.CompletedTask;
        }

        /// <summary>
        /// Cancel result.
        /// </summary>
        public void Cancel()
        {
            ModalTaskCompletionSource.SetResult(ModalResult.Cancel);
        }

        /// <summary>
        /// Modal closed result.
        /// </summary>
        public virtual void Closed()
        {
            if (!ModalTaskCompletionSource.Task.IsCompleted)
            {
                ModalTaskCompletionSource.SetResult(ModalResult.Exit);
            }
        }
    }
}