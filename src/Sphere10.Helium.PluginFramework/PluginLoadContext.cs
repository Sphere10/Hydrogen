using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Loader;
using Sphere10.Framework.Application;

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
            
            var assembly = assemblyPath != null ? LoadFromAssemblyPath(assemblyPath) : base.Load(assemblyName);
            Sphere10Framework.Instance.LoadPluginAssembly(assembly);  // this ensures module configurations are executed
            return assembly;
        }

        protected override IntPtr LoadUnmanagedDll(string unmanagedDllName)
        {
            var libraryPath = _resolver.ResolveUnmanagedDllToPath(unmanagedDllName);

            return libraryPath != null ? LoadUnmanagedDllFromPath(libraryPath) : IntPtr.Zero;
        }

    }
}