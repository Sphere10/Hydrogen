using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Loader;

namespace Sphere10.Helium.PluginFramework
{
    internal class PluginLoadContext : AssemblyLoadContext
    {
        private readonly AssemblyDependencyResolver _resolver;
        public readonly HashSet<string> DefaultLoadedAssemblies = new HashSet<string>();

        public PluginLoadContext(string pluginPath)
        {
            _resolver = new AssemblyDependencyResolver(pluginPath);
        }

        protected override Assembly Load(AssemblyName assemblyName)
        {

            if (DefaultLoadedAssemblies.Contains(assemblyName.Name))
            {
                // ReSharper disable once ConditionIsAlwaysTrueOrFalse
                if (Default.LoadFromAssemblyName(assemblyName) != null)
                {
                    return null;
                }
            }

            var assemblyPath = _resolver.ResolveAssemblyToPath(assemblyName);
            
            return assemblyPath != null ? LoadFromAssemblyPath(assemblyPath) : base.Load(assemblyName);
        }

        protected override IntPtr LoadUnmanagedDll(string unmanagedDllName)
        {
            var libraryPath = _resolver.ResolveUnmanagedDllToPath(unmanagedDllName);

            return libraryPath != null ? LoadUnmanagedDllFromPath(libraryPath) : IntPtr.Zero;
        }
    }
}