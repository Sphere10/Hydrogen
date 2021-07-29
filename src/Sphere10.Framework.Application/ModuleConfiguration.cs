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

namespace Sphere10.Framework.Application {
	public class ModuleConfiguration : ModuleConfigurationBase {

		public override int Priority => int.MinValue; // last to execute

		public override void RegisterComponents(ComponentRegistry registry) {

			if (!registry.HasImplementationFor<IBackgroundLicenseVerifier>())
				registry.RegisterComponent<IBackgroundLicenseVerifier, NoOpBackgroundLicenseVerifier>();

			if (!registry.HasImplementationFor<ISettingsProvider>("UserSettings"))
				registry.RegisterComponentInstance<ISettingsProvider>(new DirectorySettingsProvider(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), AppDomain.CurrentDomain.FriendlyName)), "UserSettings");

			if (!registry.HasImplementationFor<ISettingsProvider>("SystemSettings"))
				registry.RegisterComponentInstance<ISettingsProvider>(new DirectorySettingsProvider(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), AppDomain.CurrentDomain.FriendlyName)), "SystemSettings");

			if (!registry.HasImplementationFor<IConfigurationServices>())
				registry.RegisterComponent<IConfigurationServices, StandardConfigurationServices>(activation: ActivationType.Singleton);

			if (!registry.HasImplementationFor<IDuplicateProcessDetector>())
				registry.RegisterComponent<IDuplicateProcessDetector, StandardDuplicateProcessDetector>();

			if (!registry.HasImplementationFor<IHelpServices>())
				registry.RegisterComponent<IHelpServices, StandardHelpServices>();

			if (registry.HasImplementationFor<ILicenseEnforcer>())
				throw new SoftwareException("Illegal tampering with ILicenseEnforcer");
			registry.RegisterComponent<ILicenseEnforcer, StandardLicenseEnforcer>(activation: ActivationType.Singleton);

			if (registry.HasImplementationFor<ILicenseKeyDecoder>())
				throw new SoftwareException("Illegal tampering with ILicenseKeyDecoder");
			registry.RegisterComponent<ILicenseKeyDecoder, StandardLicenseKeyDecoder>();

			if (registry.HasImplementationFor<ILicenseKeyValidator>())
				throw new SoftwareException("Illegal tampering with ILicenseKeyValidator");
			registry.RegisterComponent<ILicenseKeyValidator, StandardLicenseKeyValidatorWithVersionCheck>();

			if (registry.HasImplementationFor<ILicenseKeyEncoder>())
				throw new SoftwareException("Illegal tampering with ILicenseKeyEncoder");
			registry.RegisterComponent<ILicenseKeyEncoder, StandardLicenseKeyEncoder>();

			if (registry.HasImplementationFor<ILicenseKeyServices>())
				throw new SoftwareException("Illegal tampering with ILicenseKeyServices");
			registry.RegisterComponent<ILicenseKeyServices, StandardLicenseKeyProvider>();

			if (registry.HasImplementationFor<ILicenseServices>())
				throw new SoftwareException("Illegal tampering with ILicenseServices");
			registry.RegisterComponent<ILicenseServices, StandardLicenseServices>(activation: ActivationType.Singleton);

			if (registry.HasImplementationFor<IProductInformationServices>())
				throw new SoftwareException("Illegal tampering with IProductInformationServices");
			registry.RegisterComponent<IProductInformationServices, StandardProductInformationServices>(activation: ActivationType.Singleton);
			
			if (!registry.HasImplementationFor<IProductInstancesCounter>())
				registry.RegisterComponent<IProductInstancesCounter, StandardProductInstancesCounter>();

			if (!registry.HasImplementationFor<IProductUsageServices>())
				registry.RegisterComponent<IProductUsageServices, StandardProductUsageServices>(activation: ActivationType.Singleton);

			if (!registry.HasImplementationFor<IWebsiteLauncher>())
				registry.RegisterComponent<IWebsiteLauncher, StandardWebsiteLauncher>();

			// HS 2021-07-12: top-level application should register this, since it is optional
			//if (!registry.HasInitializationTask<IncrementUsageByOneTask>())
			//	registry.RegisterInitializationTask<IncrementUsageByOneTask>();


			// Start Tasks
			// ....


			// End Tasks
			if (!registry.HasEndTask<SaveSettingsEndTask>())
				registry.RegisterEndTask<SaveSettingsEndTask>();

		}
	}
}
