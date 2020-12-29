using System.Collections.Generic;
using System.Linq;
using VelocityNET.Presentation.Blazor.Shared;
using VelocityNET.Presentation.Blazor.Shared.Plugins;

namespace VelocityNET.Presentation.Blazor.ViewModels
{
    public class AppBlocksMenuViewModel : ComponentViewModelBase
    {
        public IEnumerable<IApp> Apps { get; }

        public AppBlocksMenuViewModel(IEnumerable<IPlugin> plugins)
        {
            Apps = plugins?.SelectMany(x => x?.Apps) ?? Enumerable.Empty<IApp>();
        }
    }
}