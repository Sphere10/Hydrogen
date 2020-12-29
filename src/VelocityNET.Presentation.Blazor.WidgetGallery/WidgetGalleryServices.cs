using Microsoft.Extensions.DependencyInjection;
using VelocityNET.Presentation.Blazor.Shared;
using VelocityNET.Presentation.Blazor.Shared.Plugins;
using VelocityNET.Presentation.Blazor.WidgetGallery.Services;

namespace VelocityNET.Presentation.Blazor.WidgetGallery
{
    /// <summary>
    /// Services collection for widget gallery plugin. Base class locates view models.
    /// </summary>
    public class WidgetGalleryServices : PluginServiceCollection
    {
        public WidgetGalleryServices()
        {
            this.AddViewModelsFromAssembly(GetType().Assembly);

            this.AddTransient<IRandomNumberService, RandomNumberService>();
        }
    }
}