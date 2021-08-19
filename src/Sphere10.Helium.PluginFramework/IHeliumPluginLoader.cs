using Sphere10.Helium.Framework;

namespace Sphere10.Helium.PluginFramework {
    public interface IHeliumPluginLoader {

        bool AllPluginsEnabled { get; }
        IHeliumFramework GetHeliumFramework();
        void LoadPlugins(string[] relativeAssemblyPathList);
        void EnableThesePlugins(string[] relativePathList);
        void DisableThesePlugins(string[] relativePathList);
        void DisableAllPlugins();
        void EnableAllPlugins();
        string[] GetEnabledPlugins();
        string[] GetDisabledPlugins();
    }
}