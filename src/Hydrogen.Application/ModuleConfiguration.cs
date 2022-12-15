//-----------------------------------------------------------------------
// <copyright file="ModuleConfiguration.cs" company="Sphere 10 Software">
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
using System.IO;
using Microsoft.Extensions.DependencyInjection;

namespace Hydrogen.Application {
	public class ModuleConfiguration : ModuleConfigurationBase {

		public override int Priority => int.MinValue; // last to execute

		public override void RegisterComponents(IServiceCollection serviceCollection) {

			if (!serviceCollection.HasImplementationFor<IBackgroundLicenseVerifier>())
				serviceCollection.AddTransient<IBackgroundLicenseVerifier, NoOpBackgroundLicenseVerifier>();

			if (!serviceCollection.HasImplementationFor<IConfigurationServices>())
				serviceCollection.AddSingleton<IConfigurationServices, StandardConfigurationServices>();

			if (!serviceCollection.HasImplementationFor<IDuplicateProcessDetector>())
				serviceCollection.AddTransient<IDuplicateProcessDetector, StandardDuplicateProcessDetector>();

			if (!serviceCollection.HasImplementationFor<IHelpServices>())
				serviceCollection.AddTransient<IHelpServices, StandardHelpServices>();

			if (serviceCollection.HasImplementationFor<ILicenseEnforcer>())
				throw new SoftwareException("Illegal tampering with ILicenseEnforcer");
			serviceCollection.AddSingleton<ILicenseEnforcer, StandardLicenseEnforcer>();

			if (serviceCollection.HasImplementationFor<ILicenseKeyDecoder>())
				throw new SoftwareException("Illegal tampering with ILicenseKeyDecoder");
			serviceCollection.AddTransient<ILicenseKeyDecoder, StandardLicenseKeyDecoder>();

			if (serviceCollection.HasImplementationFor<ILicenseKeyValidator>())
				throw new SoftwareException("Illegal tampering with ILicenseKeyValidator");
			serviceCollection.AddTransient<ILicenseKeyValidator, StandardLicenseKeyValidatorWithVersionCheck>();

			if (serviceCollection.HasImplementationFor<ILicenseKeyEncoder>())
				throw new SoftwareException("Illegal tampering with ILicenseKeyEncoder");
			serviceCollection.AddTransient<ILicenseKeyEncoder, StandardLicenseKeyEncoder>();

			if (serviceCollection.HasImplementationFor<ILicenseKeyServices>())
				throw new SoftwareException("Illegal tampering with ILicenseKeyServices");
			serviceCollection.AddTransient<ILicenseKeyServices, StandardLicenseKeyProvider>();

			if (serviceCollection.HasImplementationFor<ILicenseServices>())
				throw new SoftwareException("Illegal tampering with ILicenseServices");
			serviceCollection.AddSingleton<ILicenseServices, StandardLicenseServices>();

			if (serviceCollection.HasImplementationFor<IProductInformationServices>())
				throw new SoftwareException("Illegal tampering with IProductInformationServices");
			serviceCollection.AddSingleton<IProductInformationServices, StandardProductInformationServices>();

			if (!serviceCollection.HasImplementationFor<IProductInstancesCounter>())
				serviceCollection.AddTransient<IProductInstancesCounter, StandardProductInstancesCounter>();

			if (!serviceCollection.HasImplementationFor<IProductUsageServices>())
				serviceCollection.AddSingleton<IProductUsageServices, StandardProductUsageServices>();

			if (!serviceCollection.HasImplementationFor<IWebsiteLauncher>())
				serviceCollection.AddTransient<IWebsiteLauncher, StandardWebsiteLauncher>();

			serviceCollection.AddTransient<ITokenResolver, ApplicationTokenResolver>();

			// Register settings provider last
			if (!serviceCollection.HasImplementationFor<Local<ISettingsProvider>>()) {
				serviceCollection.AddSingleton(
					new Local<ISettingsProvider>(
						new CachedSettingsProvider(
							new DirectorySettingsProvider(Path.Combine(Tools.Text.FormatEx("{UserDataDir}"), Tools.Text.FormatEx("{ProductName}")))
						)
					)
				);
			}

			if (!serviceCollection.HasImplementationFor<Global<ISettingsProvider>>()) {
				serviceCollection.AddSingleton(
					new Global<ISettingsProvider>(
						new CachedSettingsProvider(
							new DirectorySettingsProvider(Path.Combine(Tools.Text.FormatEx("{SystemDataDir}"), Tools.Text.FormatEx("{ProductName}")))
						)
					)
				);
			}

			// HS 2021-07-12: top-level application should register this, since it is optional
			//if (!registry.HasInitializer<IncrementUsageByOneTask>())
			//	registry.RegisterInitializer<IncrementUsageByOneTask>();

			// Start Tasks
			// ....


			// End Tasks

		}

		public override void OnInitialize(IServiceProvider serviceProvider) {
			base.OnInitialize(serviceProvider);
			if (Tools.Runtime.GetEntryAssembly().TryGetCustomAttributeOfType<AssemblyProductSecretAttribute>(false, out var attribute)) {
				EncryptedAttribute.ApplicationSharedSecret = attribute.Secret;
			}
			GlobalSettings.Provider = serviceProvider.GetService<Global<ISettingsProvider>>()?.Item;
			UserSettings.Provider = serviceProvider.GetService<Local<ISettingsProvider>>()?.Item;
		}
	}
}
