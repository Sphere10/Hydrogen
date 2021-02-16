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
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sphere10.Framework;
using Tools;

namespace Sphere10.Framework.Application {
    public class Sphere10Framework {
        private readonly object _threadLock;
        private bool _registeredConfig;
        private bool _registeredModuleConfig;
        private volatile IModuleConfiguration[] _moduleConfigurations;


        static Sphere10Framework() {
            Instance = new Sphere10Framework();
        }

        public Sphere10Framework() {
            _threadLock = new object();
            _moduleConfigurations = null;
        }

        public static Sphere10Framework Instance { get; }

        public void RegisterAppConfig(string configSectionName = "ComponentRegistry") {
            if (_registeredConfig)
                throw new SoftwareException("Components defined in App/Web config have already been registered");

            lock (_threadLock) {
                if (_registeredConfig) return;
                var section = ConfigurationManager.GetSection(configSectionName) as ComponentRegistryDefinition;
                if (section != null) {
                    ComponentRegistry.Instance.RegisterDefinition(section);
                }                
                _registeredConfig = true;
            }
        }

        public void RegisterAllModuleConfig() {
            if (_registeredModuleConfig)
                throw new SoftwareException("All modules have already been initialized, cannot initialize again.");

            lock (_threadLock) {
                if (_registeredModuleConfig) return;
                RegisterAllModuleComponents(ComponentRegistry.Instance);
                _registeredModuleConfig = true;
            }
        }

        public void StartFramework() {
			// Register App/Web Config components

			if (!_registeredConfig)
                RegisterAppConfig();

			if (!_registeredModuleConfig)
                RegisterAllModuleConfig();

	        // Execute all the initialization tasks syncronously and in sequence
            ComponentRegistry
                .Instance
                .ResolveAll<IApplicationInitializeTask>()
                .OrderBy(initTask => initTask.Sequence)
                .ForEach(
                    initTask => Exceptions.ExecuteIgnoringException(initTask.Initialize)
                );

            // Execute all the start tasks asyncronously
            ComponentRegistry
                .Instance
                .ResolveAll<IApplicationStartTask>()
                .ForEach(
                    startTask => Lambda.ActionAsAsyncronous(startTask.Start).IgnoringExceptions().Invoke()
                );

            StartAllModules();
        }

        public void EndFramework(out bool abort, out string abortReason) {
            abortReason = String.Empty;
            abort = false;

            EndAllModules();

            var results = new List<Result>();
            ComponentRegistry
                .Instance
                .ResolveAll<IApplicationEndTask>()
                .ForEach(
                    endTask => Exceptions.ExecuteIgnoringException(() => results.Add(endTask.End()))
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

            EndAllModules();
            ComponentRegistry.Instance.Dispose();
        }

        private void RegisterAllModuleComponents(ComponentRegistry registry) {
            ModuleConfigurations.Update(mconf => mconf.RegisterComponents(registry));
        }

        private void StartAllModules() {
            ModuleConfigurations.Update(mconf => Exceptions.ExecuteIgnoringException(mconf.OnApplicationStart));
        }

        private void EndAllModules() {
            ModuleConfigurations.Update(mconf => Exceptions.ExecuteIgnoringException(mconf.OnApplicationEnd));
        }


        private IEnumerable<IModuleConfiguration> ModuleConfigurations {
            get {
                if (_moduleConfigurations == null) {
                    lock (_threadLock) {
                        if (_moduleConfigurations == null) {
                            _moduleConfigurations =
                                AppDomain
                                    .CurrentDomain
                                    .GetNonFrameworkAssemblies()
									//.Apply(a => System.IO.File.AppendAllText("c:\\temp\\log.txt", a.FullName + Environment.NewLine))
                                    .SelectMany(a => a.GetTypes())
                                    .Where(t => t.IsClass && !t.IsAbstract && typeof(IModuleConfiguration).IsAssignableFrom(t))
                                    .Select(TypeActivator.Create)
                                    .Cast<IModuleConfiguration>()
	                                .OrderByDescending(x => x.Priority)
                                    .ToArray();
                        }
                    }
                }
                return _moduleConfigurations;
            }
        }
    }
}
