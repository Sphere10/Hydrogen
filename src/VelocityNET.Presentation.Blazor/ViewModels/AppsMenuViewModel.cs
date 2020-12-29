using System;
using System.Collections.Generic;
using System.Linq;
using VelocityNET.Presentation.Blazor.Plugins;
using VelocityNET.Presentation.Blazor.Shared;
using VelocityNET.Presentation.Blazor.Shared.Plugins;

namespace VelocityNET.Presentation.Blazor.ViewModels
{
    public class AppsMenuViewModel : ComponentViewModelBase
    {
        private IPluginManager PluginManager { get; }
        
        public IEnumerable<IApp> Apps { get; }

        public AppsMenuViewModel(IPluginManager pluginManager)
        {
            PluginManager = pluginManager ?? throw new ArgumentNullException(nameof(pluginManager));
            Apps = pluginManager.Plugins.SelectMany(x => x?.Apps);
        }
    }
}