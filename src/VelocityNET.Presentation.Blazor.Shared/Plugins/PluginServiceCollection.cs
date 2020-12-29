using Microsoft.Extensions.DependencyInjection;

namespace VelocityNET.Presentation.Blazor.Shared.Plugins
{

    /// <summary>
    /// Extends the default implementation of <see cref="ServiceCollection"/> adding common
    /// component location strategies. 
    /// </summary>
    public class PluginServiceCollection : ServiceCollection, IPluginServiceCollection
    {
    }
}