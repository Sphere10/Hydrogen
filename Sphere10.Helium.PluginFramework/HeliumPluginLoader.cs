using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Sphere10.Helium.Framework;
using Sphere10.Helium.Handler;

namespace Sphere10.Helium.PluginFramework
{
    public class HeliumPluginLoader
    {
        private static IList<PluginAssemblyHandler> PluginAssemblyHandlerList { get; set; }

        public HeliumPluginLoader()
        {
            PluginAssemblyHandlerList = new List<PluginAssemblyHandler>();
        }

        public HeliumFramework GetHeliumFramework()
        {
            return HeliumFramework.Instance;
        }

        public void LoadPlugins(string[] relativeAssemblyPathList)
        {
            foreach (var path in relativeAssemblyPathList)
            {
                var pluginAssembly = LoadPluginAssembly(path);

                GetHandlers(pluginAssembly);
            }

            HeliumFramework.Instance.PluginAssemblyHandlerList = PluginAssemblyHandlerList;
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

        private static void GetHandlers(Assembly assembly)
        {
            var count = assembly.GetTypes().Aggregate(0, (current, type) => CheckIfInterfaceIsHandler(type, current));

            if (count != 0) return;

            var availableTypes = string.Join(",", assembly.GetTypes().Select(t => t.FullName));
            var typeString = $"Can't find any type which implements IHandleMessage<IMessage> in {assembly} from {assembly.Location}.\nAvailable types: {availableTypes}";

            //TODO: Jake: typeString needs to be written to a log file. ILogger needs to be injected somehow?
            typeString = typeString.Replace(",", "\n");
            Console.WriteLine(typeString);
        }

        private static int CheckIfInterfaceIsHandler(Type type, int count) {

            foreach (var i in type.GetInterfaces()) {
                //TODO: Jake: do not compare string but rather compare Type. How the bleep am I going to that?
                if (!i.IsGenericType || !string.Equals(i.GetGenericTypeDefinition().AssemblyQualifiedName, typeof(IHandleMessage<>).AssemblyQualifiedName))
                    continue;

                count++;

                var handlerType = new PluginAssemblyHandler {
                    Handler = i,
                    Message = i.GetGenericArguments()[0]
                };

                PluginAssemblyHandlerList.Add(handlerType);
            }
            return count;
        }
    }
}
