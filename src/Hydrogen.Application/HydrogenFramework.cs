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

using RailwaySharp.ErrorHandling;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;

namespace Hydrogen.Application {
	public class HydrogenFramework {
		private readonly object _threadLock;
		private bool _registeredConfig;
		private bool _registeredModuleComponents;
		private bool _initializedModules;
		private bool _executedApplicationInitializers;

		public event EventHandlerEx Initializing;
		public event EventHandlerEx Initialized;
		public event EventHandlerEx Finalizing;
		public event EventHandlerEx Finalized;
		

		static HydrogenFramework() {
			Instance = new HydrogenFramework();
		}

		public HydrogenFramework() {
			IsStarted = false;
			_threadLock = new object();
			ModuleConfigurations = Tools.Values.Future.LazyLoad(() =>
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

		public static HydrogenFramework Instance { get; }

		public bool IsStarted { get; private set; }

		private IFuture<IModuleConfiguration[]> ModuleConfigurations { get; set; }
		
		public void StartFramework() {
			CheckNotStarted();
			Initializing?.Invoke();

			// Register App/Web Config components
			if (!_registeredConfig)
				RegisterAppConfig();

			if (!_registeredModuleComponents)
				RegisterAllModuleComponents();

			// Initialize Modules
			if (!_initializedModules)
				InitializeModules();

			if (!_executedApplicationInitializers)
				InitializeApplication();

			Initialized?.Invoke();

			IsStarted = true;
		}

		public void EndFramework()
			=> EndFramework(out _, out _);

		public void EndFramework(out bool abort, out string abortReason) {
			CheckStarted();
			abortReason = string.Empty;
			abort = false;

			Finalizing?.Invoke();

			var result = FinalizeApplication();
			if (result.Failure) {
				abort = true;
				abortReason = result.ToString();
				return;
			}

			FinalizeModules();
			IsStarted = false;
			Finalized?.Invoke();
		}

		public ILogger CreateApplicationLogger(bool visibleToAllUsers = false)
			=> CreateApplicationLogger(Tools.Text.FormatEx("{ProductName}"), visibleToAllUsers: visibleToAllUsers);

		public ILogger CreateApplicationLogger(string logName, bool visibleToAllUsers = false)
			=> CreateApplicationLogger(logName, RollingFileLogger.DefaultMaxFiles, RollingFileLogger.DefaultMaxFileSizeB, visibleToAllUsers);

		public ILogger CreateApplicationLogger(string logFileName, int maxFiles, int maxFileSize, bool visibleToAllUsers = false)
			=> new ThreadIdLogger(
				new TimestampLogger(
					new RollingFileLogger(
						Path.Combine(Tools.Text.FormatEx(visibleToAllUsers ? "{SystemDataDir}" : "{UserDataDir}"), Tools.Text.FormatEx("{ProductName}"), "logs", logFileName),
						maxFiles,
						maxFileSize
					)
				)
			);

		internal void RegisterAppConfig(string configSectionName = "ComponentRegistry") {
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

		internal void RegisterAllModuleComponents() {
			CheckNotStarted();

			if (_registeredModuleComponents)
				throw new SoftwareException("All modules have already been initialized, cannot initialize again.");

			lock (_threadLock) {
				if (_registeredModuleComponents) return;
				ModuleConfigurations.Value.Update(mconf => mconf.RegisterComponents(ComponentRegistry.Instance));
				_registeredModuleComponents = true;
			}
		}

		internal void InitializeModules() {
			CheckNotStarted();
			if (_initializedModules)
				throw new SoftwareException("All modules have already been initialized, cannot initialize again.");

			ModuleConfigurations.Value.Update(moduleConfiguration => Tools.Exceptions.ExecuteIgnoringException(moduleConfiguration.OnInitialize));
		}

		internal void InitializeApplication() {
			CheckNotStarted();
			Initializing?.Invoke();
			// Execute all the application initialization tasks synchronously and in sequence
			ComponentRegistry
				.Instance
				.ResolveAll<IApplicationInitializer>()
				.Where(x => !x.Parallelizable)
				.OrderBy(initTask => initTask.Priority)
				.ForEach(
					initTask => Tools.Exceptions.ExecuteIgnoringException(initTask.Initialize)
				);
			

			// Execute all the start tasks asynchronously
			ComponentRegistry
				.Instance
				.ResolveAll<IApplicationInitializer>()
				.Where(x => !x.Parallelizable)
				.OrderBy(initTask => initTask.Priority)
				.ForEach(
					startTask => Tools.Lambda.ActionAsAsyncronous(startTask.Initialize).IgnoringExceptions().Invoke()
				);
		}

		internal Result FinalizeApplication() {
			var results = new List<Result>();
			ComponentRegistry
				.Instance
				.ResolveAll<IApplicationFinalizer>()
				.ForEach(
					endTask => Tools.Exceptions.Execute(endTask.Finalize, error => results.Add(Result.Error(error.ToDisplayString())))
				);
			return Result.Combine(results);
		}

		internal void FinalizeModules() {
			ModuleConfigurations.Value.Update(moduleConfiguration => Tools.Exceptions.ExecuteIgnoringException(moduleConfiguration.OnFinalize));
			DeregisterAllModuleComponents();
			ComponentRegistry.Instance.Dispose();

		}

		internal void DeregisterAllModuleComponents() {
			CheckStarted();
			if (!_registeredModuleComponents) return;

			lock (_threadLock) {
				ModuleConfigurations.Value.Update(mconf => mconf.DeregisterComponents(ComponentRegistry.Instance));
				_registeredModuleComponents = false;
			}
		}

		private void CheckStarted() {
			if (!IsStarted)
				throw new InvalidOperationException("Hydrogen Framework was not started");
		}

		private void CheckNotStarted() {
			if (IsStarted)
				throw new InvalidOperationException("Hydrogen Framework is alraedy started");
		}
	}
}
