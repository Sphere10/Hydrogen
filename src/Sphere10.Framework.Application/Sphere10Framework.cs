//-----------------------------------------------------------------------
// <copyright file="ApplicationLifecycle.cs" company="Sphere 10 Software">
//
// Copyright (c) Sphere 10 Software. All rights reserved. (http://www.sphere10.com)
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// <author>Herman Schoenfeld</author>
// <date>2018</date>
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;

namespace Sphere10.Framework.Application {
	public class Sphere10Framework {
        private readonly object _threadLock;
        private bool _registeredConfig;
        private bool _registeredModuleComponents;

        public event EventHandlerEx Initializing;
        public event EventHandlerEx Starting;
        public event EventHandlerEx Ending;
        public event EventHandlerEx Finalizing;

        static Sphere10Framework() {
            Instance = new Sphere10Framework();
        }

        public Sphere10Framework() {
            IsStarted = false;
            _threadLock = new object();
            PluginConfigurations = new Dictionary<Assembly, IModuleConfiguration>();
			ModuleConfigurations = Tools.Values.LazyLoad( () =>
				AppDomain
					.CurrentDomain
					.GetNonFrameworkAssemblies()
					//.Apply(a => System.IO.File.AppendAllText("c:\\temp\\log.txt", a.FullName + Environment.NewLine))
					.SelectMany(a => a.GetTypes())
					.Where(t => t.IsClass && !t.IsAbstract && typeof(IModuleConfiguration).IsAssignableFrom(t))
					.Select(TypeActivator.Create)
					.Cast<IModuleConfiguration>()
					.OrderByDescending(x => x.Priority)
					.ToArray()
			);
        }

		public static Sphere10Framework Instance { get; }

        public bool IsStarted { get; private set; }

        private IFuture<IModuleConfiguration[]> ModuleConfigurations { get; }

		private IDictionary<Assembly, IModuleConfiguration[]> PluginConfigurations { get; }

        public void StartFramework() {
            CheckNotStarted();

            // Register App/Web Config components

            if (!_registeredConfig)
                RegisterAppConfig();

            if (!_registeredModuleComponents)
                RegisterAllModuleComponents();

            // Initialize Modules
            ModuleConfigurations.Value.Update(mconf => Tools.Exceptions.ExecuteIgnoringException(mconf.OnInitialize));


            // Execute all the application initialization tasks synchronously and in sequence
            ComponentRegistry
                .Instance
                .ResolveAll<IApplicationInitializeTask>()
                .OrderBy(initTask => initTask.Priority)
                .ForEach(
                    initTask => Tools.Exceptions.ExecuteIgnoringException(initTask.Initialize)
                );
            Initializing?.Invoke();

            // Execute all the start tasks asynchronously
            ComponentRegistry
                .Instance
                .ResolveAll<IApplicationStartTask>()
                .ForEach(
                    startTask => Tools.Lambda.ActionAsAsyncronous(startTask.Start).IgnoringExceptions().Invoke()
                );
            Starting?.Invoke();

            IsStarted = true;
        }

        public void EndFramework()
            => EndFramework(out _, out _);

        public void EndFramework(out bool abort, out string abortReason) {
            CheckStarted();

            abortReason = string.Empty;
            abort = false;

            var results = new List<Result>();
            ComponentRegistry
                .Instance
                .ResolveAll<IApplicationEndTask>()
                .ForEach(
                    endTask => Tools.Exceptions.Execute(() => endTask.End(), error => results.Add(Result.Error(error.ToDisplayString())))
                );
            Ending?.Invoke();

            if (results.Any(r => r.Failure)) {
                var textBuilder = new ParagraphBuilder();
                results.ForEach(
                    result => {
                        result.ErrorMessages.ForEach(
                            notification => textBuilder.AppendSentence(notification));
                        textBuilder.AppendParagraphBreak();
                    }
                 );
                abort = true;
                abortReason = textBuilder.ToString();
            }

            // Finalize modules
            ModuleConfigurations.Value.Update(mconf => Tools.Exceptions.ExecuteIgnoringException(mconf.OnFinalize));
            Finalizing?.Invoke();

            DeregisterAllModuleComponents();

            ComponentRegistry.Instance.Dispose();

            IsStarted = false;
        }

        public void LoadPluginAssembly(Assembly assembly) {
	        var pluginModuleConfigurations = assembly
		        .GetTypes()
		        .Where(t => t.IsClass && !t.IsAbstract && typeof(IModuleConfiguration).IsAssignableFrom(t))
		        .Select(TypeActivator.Create)
		        .Cast<IModuleConfiguration>()
		        .OrderByDescending(x => x.Priority)
		        .ToArray();

	        pluginModuleConfigurations.Update(mconf => mconf.RegisterComponents(ComponentRegistry.Instance));
			pluginModuleConfigurations.Update(mconf => Tools.Exceptions.ExecuteIgnoringException(mconf.OnInitialize));
			PluginConfigurations.Add(assembly, pluginModuleConfigurations);
		}

        public void UnloadPluginAssembly(Assembly assembly) {
	        if (!PluginConfigurations.TryGetValue(assembly, out var pluginModuleConfigurations))
		        throw new SoftwareException($"Plugin '{assembly.FullName} was not loaded");

	        pluginModuleConfigurations.Update(mconf => mconf.OnFinalize());
			pluginModuleConfigurations.Update(mconf => mconf.DeregisterComponents(ComponentRegistry.Instance));
        }


		public void RegisterAppConfig(string configSectionName = "ComponentRegistry") {
            CheckNotStarted();

            if (_registeredConfig)
                throw new SoftwareException("Components defined in App/Web config have already been registered");

            lock (_threadLock) {
                if (_registeredConfig) return;
				if (ConfigurationManager.GetSection(configSectionName) is ComponentRegistryDefinition definition) {
                    ComponentRegistry.Instance.RegisterDefinition(definition);
                }                
                _registeredConfig = true;
            }
        }

        public void RegisterAllModuleComponents() {
            CheckNotStarted();

            if (_registeredModuleComponents)
                throw new SoftwareException("All modules have already been initialized, cannot initialize again.");

            lock (_threadLock) {
                if (_registeredModuleComponents) return;
				ModuleConfigurations.Value.Update(mconf => mconf.RegisterComponents(ComponentRegistry.Instance));
                _registeredModuleComponents = true;
            }
        }

		public void DeregisterAllModuleComponents() {
            CheckStarted();
			if (!_registeredModuleComponents) return;

			lock (_threadLock) {
				ModuleConfigurations.Value.Update(mconf => mconf.DeregisterComponents(ComponentRegistry.Instance));
				_registeredModuleComponents = false;
			}
		}

        private void CheckStarted() {
            if (!IsStarted)
                throw new InvalidOperationException("Sphere 10 Framework was not started");
        }

        private void CheckNotStarted() {
            if (IsStarted)
                throw new InvalidOperationException("Sphere 10 Framework is alraedy started");
        }
    }
}
