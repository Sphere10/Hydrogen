﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Sphere10.Framework;
using Sphere10.Helium.Framework;
using Sphere10.Helium.Handler;

namespace Sphere10.Helium.PluginFramework
{
    public class HeliumPluginLoader : IHeliumPluginLoader
    {
        private readonly ILogger _logger;
        private static IList<PluginAssemblyHandler> PluginAssemblyHandlerList { get; set; }

        public HeliumPluginLoader(ILogger logger)
        {
            _logger = logger;
            PluginAssemblyHandlerList = new List<PluginAssemblyHandler>();
        }

        public bool AllPluginsEnabled { get; private set; } = true;

        public HeliumFramework GetHeliumFramework()
        {
            var heliumFrameworkInstance = HeliumFramework.Instance;
            heliumFrameworkInstance.Logger = _logger;

            return heliumFrameworkInstance;
        }

        public void LoadPlugins(string[] relativeAssemblyPathList)
        {
            foreach (var path in relativeAssemblyPathList)
            {
                var pluginAssembly = LoadPluginAssembly(path);

                GetHandlers(pluginAssembly, path);
            }

            HeliumFramework.Instance.LoadHandlerTypes(PluginAssemblyHandlerList);
        }

        public void EnableThesePlugins(string[] relativePathList)
        {
            if (relativePathList == null || relativePathList.Length == 0)
                return;

            var listToUpdate = from relativePath in relativePathList.ToList()
                        join assemblyHandler in PluginAssemblyHandlerList
                    on relativePath.ToUpperInvariant() equals assemblyHandler.RelativePath.ToUpperInvariant()
                        select new { relativePath, assemblyHandler };

            foreach (var item in listToUpdate)
                item.assemblyHandler.IsEnabled = true;
        }

        public void DisableThesePlugins(string[] relativePathList)
        {
            if (relativePathList == null || relativePathList.Length == 0)
                return;

            var listToUpdate = from relativePath in relativePathList.ToList()
                        join assemblyHandler in PluginAssemblyHandlerList
                            on relativePath.ToUpperInvariant() equals assemblyHandler.RelativePath.ToUpperInvariant()
                        select new { relativePath, assemblyHandler };

            foreach (var item in listToUpdate)
                item.assemblyHandler.IsEnabled = false;
        }

        public void DisableAllPlugins()
        {
            AllPluginsEnabled = false;

            if (PluginAssemblyHandlerList == null || PluginAssemblyHandlerList.Count == 0) return;

            foreach (var x in PluginAssemblyHandlerList)
                if (x.IsEnabled) x.IsEnabled = false;
        }

        public void EnableAllPlugins()
        {
            AllPluginsEnabled = true;

            if (PluginAssemblyHandlerList == null || PluginAssemblyHandlerList.Count == 0) return;

            foreach (var x in PluginAssemblyHandlerList)
                if (x.IsEnabled == false) x.IsEnabled = true;
        }

        public string[] GetEnabledPlugins()
        {
            _logger.Info("I am in GetEnabledPlugins.");

            var result = PluginAssemblyHandlerList.Where(y => y.IsEnabled).
                Select(y => y.AssemblyFullName).Distinct().ToArray();

            return result;
        }

        public string[] GetDisabledPlugins()
        {
            var result = PluginAssemblyHandlerList.Where(y => y.IsEnabled == false).
                Select(y => y.AssemblyFullName).Distinct().ToArray();

            return result;
        }

        private static Assembly LoadPluginAssembly(string relativePath)
        {
            var root = Path.GetFullPath(Path.Combine(
                Path.GetDirectoryName(
                    Path.GetDirectoryName(
                        Path.GetDirectoryName(
                            Path.GetDirectoryName(
                                Path.GetDirectoryName(typeof(HeliumFramework).Assembly.Location))))) ?? throw new InvalidOperationException()));

            var pluginLocation = Path.GetFullPath(Path.Combine(root, relativePath.Replace('\\', Path.DirectorySeparatorChar)));
            var loadContext = new PluginLoadContext(pluginLocation);
            var assembly = loadContext.LoadFromAssemblyName(new AssemblyName(Path.GetFileNameWithoutExtension(pluginLocation)));

            return assembly;
        }

        private void GetHandlers(Assembly assembly, string relativePath)
        {
            var count = assembly.GetTypes().Aggregate(0, (current, type) => CheckIfInterfaceIsHandler(type, current, assembly.FullName, relativePath));

            if (count != 0) return;

            var availableTypes = string.Join(",", assembly.GetTypes().Select(t => t.FullName));
            var typeString = $"Can't find any type which implements IHandleMessage<IMessage> in {assembly} from {assembly.Location}.\nAvailable types: {availableTypes}";
            _logger.Error(typeString);

            typeString = typeString.Replace(",", "\n");
            Console.WriteLine(typeString);
        }

        private static int CheckIfInterfaceIsHandler(Type type, int count, string assemblyFullName, string relativePath)
        {
            foreach (var i in type.GetInterfaces())
            {
                if (!i.IsGenericType || !string.Equals(i.GetGenericTypeDefinition().AssemblyQualifiedName, typeof(IHandleMessage<>).AssemblyQualifiedName))
                    continue;

                count++;

                var handlerType = new PluginAssemblyHandler
                {
                    RelativePath = relativePath,
                    AssemblyFullName = assemblyFullName,
                    Handler = i,
                    Message = i.GetGenericArguments()[0]
                };

                PluginAssemblyHandlerList.Add(handlerType);
            }
            return count;
        }
    }
}