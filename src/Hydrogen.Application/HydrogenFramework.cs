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
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace Hydrogen.Application {
	public class HydrogenFramework {
		private readonly object _threadLock;
		private bool _registeredModules;
		private bool _frameworkOwnsServicesProvider;

		public event EventHandlerEx Initializing;
		public event EventHandlerEx Initialized;
		public event EventHandlerEx Finalizing;
		public event EventHandlerEx Finalized;

		static HydrogenFramework() {
			Instance = new HydrogenFramework();
		}

		public HydrogenFramework() {
			IsStarted = false;
			_registeredModules = false;
			_frameworkOwnsServicesProvider = false;
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

		public IServiceProvider ServiceProvider { get; private set; }

		public bool IsStarted { get; private set; }

		private IFuture<IModuleConfiguration[]> ModuleConfigurations { get; set; }

		public IServiceCollection RegisterModules(IServiceCollection serviceCollection) {
			CheckNotStarted();
			Guard.Against(_registeredModules, "Modules have already been registered");
			lock (_threadLock) {
				ModuleConfigurations.Value.ForEach(m => m.RegisterComponents(serviceCollection));
				_registeredModules = true;
			}
			return serviceCollection;
		}

		public void StartFramework() 
			=>  StartFramework(_ => { });

		public void StartFramework(Action<IServiceCollection> configure) {
			var serviceCollection = new ServiceCollection();
			if (configure != null)
				configure(serviceCollection);
			RegisterModules(serviceCollection);
			StartFramework(serviceCollection.BuildServiceProvider());
			_frameworkOwnsServicesProvider = true;
		}

		public void StartFramework(IServiceProvider serviceProvider) {
			CheckNotStarted();
			Guard.Ensure(_registeredModules, "Modules have not been registered");
			Guard.Against(IsStarted, "Hydrogen framework has already been started");
			ServiceProvider = serviceProvider;
			Initializing?.Invoke();
			InitializeModules(serviceProvider);
			InitializeApplication();
			Initialized?.Invoke();
			IsStarted = true;
		}

		public void EndFramework() {
			CheckStarted();
			Finalizing?.Invoke();
			FinalizeApplication();
			FinalizeModules(ServiceProvider);
			if (_frameworkOwnsServicesProvider && ServiceProvider is IDisposable disposable)
				try {
				disposable.Dispose();
				} catch (Exception ex) {
					var xxx = ex.ToString();
				}
			IsStarted = false;
			Finalized?.Invoke();
		}

		private void InitializeModules(IServiceProvider serviceProvider) 
			=> ModuleConfigurations
				.Value
				.ForEach(moduleConfiguration => Tools.Exceptions.ExecuteIgnoringException(() => moduleConfiguration.OnInitialize(serviceProvider)));
		
		private void InitializeApplication() {
			Initializing?.Invoke();

			var initializers = ServiceProvider.GetServices<IApplicationInitializer>().ToArray();

			// Execute non-parallelizable initializers in sequence first
			initializers
				.Where(x => !x.Parallelizable)
				.OrderBy(initTask => initTask.Priority)
				.ForEach(initTask => initTask.Initialize());


			// Parallel execute all parallelizable initialzers 
			Parallel.ForEach(initializers
			                 .Where(x => x.Parallelizable)
			                 .OrderBy(initTask => initTask.Priority),
				startTask => startTask.Initialize()
			);
			                      
		}

		internal void FinalizeModules(IServiceProvider serviceProvider) 
			=> ModuleConfigurations
				.Value
				.Update(moduleConfiguration => moduleConfiguration.OnFinalize(serviceProvider));
		
		private void FinalizeApplication() {
			ServiceProvider
				.GetServices<IApplicationFinalizer>()
				.ForEach(f => f.Finalize());
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
