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

namespace Sphere10.Framework.Application {
	public class Sphere10Framework {
        private readonly object _threadLock;
        private bool _registeredConfig;
        private bool _registeredModuleComponents;


        static Sphere10Framework() {
            Instance = new Sphere10Framework();
        }

        public Sphere10Framework() {
            _threadLock = new object();
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

        public void RegisterAppConfig(string configSectionName = "ComponentRegistry") {
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
            if (_registeredModuleComponents)
                throw new SoftwareException("All modules have already been initialized, cannot initialize again.");

            lock (_threadLock) {
                if (_registeredModuleComponents) return;
				ModuleConfigurations.Value.Update(mconf => mconf.RegisterComponents(ComponentRegistry.Instance));
                _registeredModuleComponents = true;
            }
        }

		public void DeregisterAllModuleComponents() {
			if (!_registeredModuleComponents) return;

			lock (_threadLock) {
				ModuleConfigurations.Value.Update(mconf => mconf.DeregisterComponents(ComponentRegistry.Instance));
				_registeredModuleComponents = false;
			}
		}

		public void StartFramework() {
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

			// Execute all the start tasks asynchronously
			ComponentRegistry
				.Instance
				.ResolveAll<IApplicationStartTask>()
				.ForEach(
					startTask => Tools.Lambda.ActionAsAsyncronous(startTask.Start).IgnoringExceptions().Invoke()
				);
		}

        public void EndFramework(out bool abort, out string abortReason) {
            abortReason = String.Empty;
            abort = false;

            var results = new List<Result>();
            ComponentRegistry
                .Instance
                .ResolveAll<IApplicationEndTask>()
                .ForEach(
                    endTask => Tools.Exceptions.ExecuteIgnoringException(() => results.Add(endTask.End()))
                );

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

			DeregisterAllModuleComponents();

			ComponentRegistry.Instance.Dispose();
        }
		

        private IFuture<IModuleConfiguration[]> ModuleConfigurations { get; set; }
    }
}
