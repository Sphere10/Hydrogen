//-----------------------------------------------------------------------
// <copyright file="StandardLicenseServices.cs" company="Sphere 10 Software">
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

namespace Hydrogen.Application {


	/// <summary>
	/// Provides all the licensing services for the application. 
	/// </summary>
	public class StandardLicenseServices : ILicenseServices {

		public StandardLicenseServices(IProductInformationServices productInformationServices, ILicenseKeyValidator licenseKeyValidator, ILicenseKeyDecoder licenseKeyDecoder, IConfigurationServices configurationServices, ILicenseKeyServices licenseKeyProvider) {
			ProductInformationServices = productInformationServices;
			LicenseKeyDecoder = licenseKeyDecoder;
			LicenseKeyValidator = licenseKeyValidator;
			LicenseKeyProvider = licenseKeyProvider;
			ConfigurationServices = configurationServices;
			//PreExpirationNotice = "You are using an evaluation version of this software. Please purchase the full version to unlock all the features.";
			//PostExpirationNotice = "Please purchase the full version to continue using this software.";
			LicenseInformation = DetermineLicenseInformation();
		}

		public IProductInformationServices ProductInformationServices { get; set; }

		public ILicenseKeyDecoder LicenseKeyDecoder { get; set; }

		public ILicenseKeyValidator LicenseKeyValidator { get; set; }

		public ILicenseKeyServices LicenseKeyProvider { get; set; }

		public IConfigurationServices ConfigurationServices { get; set; }

		public LicenseInformation LicenseInformation {
			get;
			private set;
		}

		public void RegisterLicenseKey(string key) {
			if (!LicenseKeyValidator.IsValid(key)) {
				throw new SoftwareException("The provided license key is invalid");
			}
			LicenseKeyProvider.SetLicenseKey(key);
			Apply();
		}

		public void RegisterLicenseOverrideCommand(ProductLicenseCommand command) {
			LicenseKeyProvider.SetLicenseOverrideCommand(command);
			Apply();
		}

		public void RemoveLicenseOverrideCommand() {
			LicenseKeyProvider.RemoveLicenseOverrideCommand();
			Apply();
		}

		protected virtual LicenseInformation DetermineLicenseInformation() {
			var hasDefaultLicenseKey = LicenseKeyProvider.HasDefaultLicenseKey();
			var hasRegisteredLicenseKey = LicenseKeyProvider.HasRegisteredLicenseKey();
			var hasLicenseOverride = LicenseKeyProvider.HasLicenseOverrideCommand();
			var defaultLicenseKey = hasDefaultLicenseKey ? LicenseKeyProvider.GetDefaultLicenseKey() : null;
			var registeredLicenseKey = hasRegisteredLicenseKey ? LicenseKeyProvider.GetRegisteredLicenseKey() : null;

			return new LicenseInformation {
				HasDefaultLicenseKey = hasDefaultLicenseKey,
				DefaultLicenseKey = defaultLicenseKey,
				DefaultLicense = hasDefaultLicenseKey ? LicenseKeyDecoder.Decode(defaultLicenseKey) : null,
				HasLicenseOverrideCommand = hasLicenseOverride,
				LicenseOverrideCommand = hasLicenseOverride ? LicenseKeyProvider.GetLicenseOverrideCommand() : null,
				HasRegisteredLicenseKey = hasRegisteredLicenseKey,
				RegisteredLicenseKey = registeredLicenseKey,
				RegisteredLicense = hasRegisteredLicenseKey ? LicenseKeyDecoder.Decode(registeredLicenseKey) : null
			};
		}

		private void Apply() {
			LicenseInformation = DetermineLicenseInformation();
			ConfigurationServices.NotifyConfigurationChangedEvent();
		}




	}
}
