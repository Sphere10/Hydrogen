// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.IO;
using Microsoft.Extensions.DependencyInjection;
using Sphere10.DRM;

namespace Hydrogen.Application;

public class ModuleConfiguration : ModuleConfigurationBase {

	public override int Priority => int.MinValue; // last to execute

	public override void RegisterComponents(IServiceCollection serviceCollection) {
		if (HydrogenFramework.Instance.Options.HasFlag(HydrogenFrameworkOptions.EnableDrm))
			EnableDRM(serviceCollection);
		else
			DisableDRM(serviceCollection);

		if (!serviceCollection.HasImplementationFor<ISettingsServices>())
			serviceCollection.AddSingleton<ISettingsServices, StandardSettingsServices>();

		if (!serviceCollection.HasImplementationFor<IDuplicateProcessDetector>())
			serviceCollection.AddTransient<IDuplicateProcessDetector, StandardDuplicateProcessDetector>();

		if (!serviceCollection.HasImplementationFor<IHelpServices>())
			serviceCollection.AddTransient<IHelpServices, StandardHelpServices>();

		if (!serviceCollection.HasImplementationFor<IProductInformationProvider>())
			serviceCollection.AddSingleton<IProductInformationProvider, AssemblyAttributesProductInformationProvider>();

		if (!serviceCollection.HasImplementationFor<IProductInstancesCounter>())
			serviceCollection.AddTransient<IProductInstancesCounter, StandardProductInstancesCounter>();

		if (!serviceCollection.HasImplementationFor<IProductUsageServices>())
			serviceCollection.AddSingleton<IProductUsageServices, ProductUsageServices>();

		if (!serviceCollection.HasImplementationFor<IWebsiteLauncher>())
			serviceCollection.AddTransient<IWebsiteLauncher, StandardWebsiteLauncher>();

		serviceCollection.AddTransient<ITokenResolver, ApplicationTokenResolver>();
		serviceCollection.AddTransient<ITokenResolver, ProductInformationTokenResolver>();
		serviceCollection.AddTransient<ITokenResolver, ProductUsageInformationTokenResolver>();

		// Register settings provider last
		if (!serviceCollection.HasImplementationFor<Local<ISettingsProvider>>()) {
			serviceCollection.AddSingleton(
				new Local<ISettingsProvider>(
					new CachedSettingsProvider(
						new DirectoryFileSettingsProvider(
							Tools.Values.Future.LazyLoad(
								() => Path.Combine(Tools.Text.FormatEx("{UserDataDir}"), Tools.Text.FormatEx("{ProductName}"))
							)
						)
					)
				)
			);
		}

		if (!serviceCollection.HasImplementationFor<Global<ISettingsProvider>>()) {
			serviceCollection.AddSingleton(
				new Global<ISettingsProvider>(
					new CachedSettingsProvider(
						new DirectoryFileSettingsProvider(
							Tools.Values.Future.LazyLoad(
								() => Path.Combine(Tools.Text.FormatEx("{SystemDataDir}"), Tools.Text.FormatEx("{ProductName}"))
							)
						)
					)
				)
			);
		}

		// Initializers/Finalizers
		serviceCollection.AddInitializer<IncrementUsageByOneInitializer>();
	}

	private void EnableDRM(IServiceCollection serviceCollection) {

		if (!serviceCollection.HasImplementationFor<IProductLicenseEnforcer>())
			serviceCollection.AddSingleton<IProductLicenseEnforcer, ProductLicenseEnforcer>();

		if (!serviceCollection.HasImplementationFor<IProductLicenseStorage>())
			serviceCollection.AddTransient<IProductLicenseStorage, ProductLicenseSettingsStorage>();

		if (!serviceCollection.HasImplementationFor<IProductLicenseProvider>())
			serviceCollection.AddTransient<IProductLicenseProvider, ProductLicenseProvider>();

		if (!serviceCollection.HasImplementationFor<IProductLicenseClient>())
			serviceCollection.AddTransient<IProductLicenseClient, AssemblyAttributeConfiguredProductLicenseClient>();

		if (!serviceCollection.HasImplementationFor<IProductLicenseActivator>())
			serviceCollection.AddTransient<IProductLicenseActivator, ProductLicenseActivator>();

		if (!serviceCollection.HasImplementationFor<IBackgroundLicenseVerifier>())
			serviceCollection.AddTransient<IBackgroundLicenseVerifier, ClientBackgroundLicenseVerifier>();

		if (HydrogenFramework.Instance.Options.HasFlag(HydrogenFrameworkOptions.BackgroundLicenseVerify))
			serviceCollection.AddInitializer<VerifyLicenseInitializer>();
	}

	private void DisableDRM(IServiceCollection serviceCollection) {
		if (!serviceCollection.HasImplementationFor<IProductLicenseEnforcer>())
			serviceCollection.AddSingleton<IProductLicenseEnforcer, NoOpProductLicenseEnforcer>();

		if (!serviceCollection.HasImplementationFor<IProductLicenseStorage>())
			serviceCollection.AddTransient<IProductLicenseStorage, NoOpProductLicenseStorage>();

		if (!serviceCollection.HasImplementationFor<IProductLicenseProvider>())
			serviceCollection.AddTransient<IProductLicenseProvider, NoOpProductLicenseProvider>();

		if (!serviceCollection.HasImplementationFor<IProductLicenseClient>())
			serviceCollection.AddTransient<IProductLicenseClient, NoOpProductLicenseClient>();

		if (!serviceCollection.HasImplementationFor<IProductLicenseActivator>())
			serviceCollection.AddTransient<IProductLicenseActivator, NoOpProductLicenseActivator>();

		if (!serviceCollection.HasImplementationFor<IBackgroundLicenseVerifier>())
			serviceCollection.AddTransient<IBackgroundLicenseVerifier, NoOpBackgroundLicenseVerifier>();

	}

	public override void OnInitialize(IServiceProvider serviceProvider) {
		base.OnInitialize(serviceProvider);
		if (Tools.Runtime.GetEntryAssembly().TryGetCustomAttributeOfType<AssemblyProductSecretAttribute>(false, out var attribute)) {
			EncryptedStringAttribute.ApplicationSharedSecret = attribute.Secret;
		}
		GlobalSettings.Provider = serviceProvider.GetService<Global<ISettingsProvider>>()?.Item;
		UserSettings.Provider = serviceProvider.GetService<Local<ISettingsProvider>>()?.Item;
		StringFormatter.RegisterResolvers(serviceProvider.GetServices<ITokenResolver>()); // this should be after provider
	}

	public override void OnFinalize(IServiceProvider serviceProvider) {
		base.OnFinalize(serviceProvider);
	}
}
