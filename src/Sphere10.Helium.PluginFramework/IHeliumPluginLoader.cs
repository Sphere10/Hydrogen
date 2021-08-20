using System.Collections.Generic;
using Sphere10.Helium.Framework;

namespace Sphere10.Helium.PluginFramework {
    public interface IHeliumPluginLoader {
	    public IList<PluginAssemblyHandler> PluginAssemblyHandlerList { get; set; }
	    public bool AllPluginsEnabled { get; }

	    public IHeliumFramework GetHeliumFramework();
	    public void LoadPlugins(string[] relativeAssemblyPathList);
	    public void EnablePlugin(string[] relativePathList);
	    public void DisablePlugin(string[] relativePathList);
	    public void DisableAllPlugins();
	    public void EnableAllPlugins();
	    public string[] GetEnabledPlugins();
	    public string[] GetDisabledPlugins();
    }
}