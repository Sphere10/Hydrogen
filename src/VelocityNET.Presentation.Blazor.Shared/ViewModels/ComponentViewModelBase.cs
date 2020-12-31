using System;
using System.Threading.Tasks;

namespace VelocityNET.Presentation.Blazor.Shared.ViewModels
{
    /// <summary>
    /// Base class for component view models.
    /// </summary>
    public abstract class ComponentViewModelBase
    {
        /// <summary>
        /// Gets or sets the state change delegate
        /// </summary>
        public Action StateHasChangedDelegate { get; set; }
        
        /// <summary>
        /// Initialize the view model.
        /// </summary>
        /// <returns> a task.</returns>
        public virtual Task InitAsync() => Task.CompletedTask;
    }
}