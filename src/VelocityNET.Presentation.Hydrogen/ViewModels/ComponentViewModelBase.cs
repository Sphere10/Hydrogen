using System;
using System.Threading.Tasks;

namespace VelocityNET.Presentation.Hydrogen.ViewModels
{

    /// <summary>
    /// Base class for component view models.
    /// </summary>
    public abstract class ComponentViewModelBase
    {
        /// <summary>
        /// Gets or sets the state change delegate
        /// </summary>
        public Action? StateHasChangedDelegate { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the view model is initialized.
        /// Set to true by calling <see cref="InitAsync"/>
        /// </summary>
        public bool IsInitialized { get; protected set; }

        /// <summary>
        /// Initialize the view model.
        /// </summary>
        /// <returns> a task.</returns>
        public async Task InitAsync()
        {
            await InitCoreAsync();
            IsInitialized = true;
        }

        /// <summary>
        /// Called when view is initialized, override to provide custom initialization logic. 
        /// </summary>
        /// <returns></returns>
        protected virtual Task InitCoreAsync()
        {
            return Task.CompletedTask;
        }
    }
}