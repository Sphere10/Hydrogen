// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.IO;
using System.IO.Enumeration;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace Hydrogen.Application;

public class HydrogenFramework {
	private readonly object _threadLock;
	private bool _registeredModules;
	private bool _frameworkOwnsServicesProvider;

	public event EventHandlerEx Initializing;
	public event EventHandlerEx Initialized;
	public event EventHandlerEx Finalizing;
	public event EventHandlerEx Finalized;

	public event EventHandlerEx<ProductInformation, ProductInformation> VersionChangeDetected;

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
				.SelectMany(a => a.GetTypes())
				.Where(t => t.IsClass && !t.IsAbstract && typeof(ICoreModuleConfiguration).IsAssignableFrom(t))
				.Select(TypeActivator.Activate)
				.Cast<ICoreModuleConfiguration>()
				.OrderByDescending(x => x.Priority)
				.ToArray()
		);
	}

	public static HydrogenFramework Instance { get; }

	public IServiceProvider ServiceProvider { get; private set; }

	public bool IsStarted { get; private set; }

	public HydrogenFrameworkOptions Options { get; private set; }

	private IFuture<ICoreModuleConfiguration[]> ModuleConfigurations { get; set; }

	public IServiceCollection RegisterModules(IServiceCollection serviceCollection) {
		CheckNotStarted();
		Guard.Against(_registeredModules, "Modules have already been registered");
		lock (_threadLock) {
			ModuleConfigurations.Value.Where(x => x is IModuleConfiguration).Cast<IModuleConfiguration>().ForEach(m => m.RegisterComponents(serviceCollection));
			_registeredModules = true;
		}
		return serviceCollection;
	}

	public void StartFramework(HydrogenFrameworkOptions options = HydrogenFrameworkOptions.Default)
		=> StartFramework(_ => { }, options);

	public void StartFramework(Action<IServiceCollection> configure, HydrogenFrameworkOptions options = HydrogenFrameworkOptions.Default) {
		Options = options;
		var serviceCollection = new ServiceCollection();
		configure?.Invoke(serviceCollection);
		RegisterModules(serviceCollection);
		StartFramework(serviceCollection.BuildServiceProvider(), options);
		_frameworkOwnsServicesProvider = true;
	}

	public void StartFramework(IServiceProvider serviceProvider, HydrogenFrameworkOptions options = HydrogenFrameworkOptions.Default) {
		CheckNotStarted();
		Guard.Ensure(_registeredModules, "Modules have not been registered");
		Guard.Against(IsStarted, "Hydrogen framework has already been started");
		Options = options;
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
			Tools.Exceptions.ExecuteIgnoringException(disposable.Dispose);
		IsStarted = false;
		Finalized?.Invoke();
	}

	public void TerminateApplication(int exitCode) {
		if (IsStarted)
			EndFramework();
		System.Environment.Exit(exitCode);
	}

	private void InitializeModules(IServiceProvider serviceProvider)
		=> ModuleConfigurations
			.Value
			.ForEach(moduleConfiguration => moduleConfiguration.OnInitialize(serviceProvider));

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

	public ILogger CreateApplicationLogger(string fileName, bool visibleToAllUsers = false)
		=> CreateApplicationLogger(fileName, RollingFileLogger.DefaultMaxFiles, RollingFileLogger.DefaultMaxFileSizeB, visibleToAllUsers);

	public ILogger CreateApplicationLogger(string fileName, int maxFiles, int maxFileSize, bool visibleToAllUsers = false)
		=> new ThreadIdLogger(
			new TimestampLogger(
				new RollingFileLogger(
					Path.Combine(Tools.Text.FormatEx(visibleToAllUsers ? "{SystemDataDir}" : "{UserDataDir}"), Tools.Text.FormatEx("{ProductName}"), "logs", fileName),
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
			throw new InvalidOperationException("Hydrogen Framework is already started");
	}
}
