using System.Threading.Tasks;

namespace VelocityNET.Presentation.Blazor.Shared
{
    /// <summary>
    /// Base class for component view models.
    /// </summary>
    public abstract class ComponentViewModelBase
    {
        /// <summary>
        /// Initialize the view model.
        /// </summary>
        /// <returns> a task.</returns>
        public virtual Task InitAsync() => Task.CompletedTask;
    }
}