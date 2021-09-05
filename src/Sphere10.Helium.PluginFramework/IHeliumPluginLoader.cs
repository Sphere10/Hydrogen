using System.Collections.Generic;
using Sphere10.Helium.Framework;

namespace Sphere10.Helium.PluginFramework {
    public interface IHeliumPluginLoader {
	    public IList<PluginAssemblyHandlerDto> PluginAssemblyHandlerList { get; set; }

	    public IHeliumFramework GetHeliumFramework();

	    public PluginAssemblyHandlerDto[] LoadPlugins(string[] relativeAssemblyPathList);

	    public void EnablePlugin(string relativePath) => EnablePlugins(new[] { relativePath });

		public void EnablePlugins(string[] relativePathList);

		public void DisablePlugin(string relativePath) => DisablePlugins(new[] { relativePath });

		public void DisablePlugins(string[] relativePathList);
	    
	    public void DisableAllPlugins();
	    public void EnableAllPlugins();
	    
	    public string[] GetEnabledPlugins();
	    public string[] GetDisabledPlugins();
    }
}