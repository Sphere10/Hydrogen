//-----------------------------------------------------------------------
// <copyright file="StandardLicenseKeyProvider.cs" company="Sphere 10 Software">
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
	public class StandardLicenseKeyProvider : ILicenseKeyServices {

		public class LicenseKeySettings : SettingsObject {
			public string LicenseKey { get; set; }
			public bool HasRegisteredKey { get; set; }
			public ProductLicenseCommand LicenseOverrideCommand { get; set; }
		}

		private readonly LicenseKeySettings _settings;

		public StandardLicenseKeyProvider(IConfigurationServices configurationServices) {
			ConfigurationServices = configurationServices;
			AssemblyAttributesManager = new AssemblyAttributesManager();
			_settings = ConfigurationServices.SystemSettings.Get<LicenseKeySettings>();
		}

		public IConfigurationServices ConfigurationServices { get; private set; }

		internal AssemblyAttributesManager AssemblyAttributesManager { get; private set; }

		public bool HasDefaultLicenseKey() {
			return AssemblyAttributesManager.HasAssemblyDefaultProductKey();
		}

		public string GetDefaultLicenseKey() {
			AssemblyAttributesManager assemblyAttibutes = new AssemblyAttributesManager();
			return assemblyAttibutes.GetAssemblyDefaultProductKey();
		}

		public bool HasRegisteredLicenseKey() {
			return _settings.HasRegisteredKey;
		}

		public string GetRegisteredLicenseKey() {
			if (!HasRegisteredLicenseKey()) {
				throw new SoftwareException("No license key has been registered with this product");
			}
			return _settings.LicenseKey;
		}

		public void SetLicenseKey(string key) {
			if (string.IsNullOrEmpty(key)) {
				throw new SoftwareException("Unable to set license key to empty string");
			}
			_settings.LicenseKey = key;
			_settings.HasRegisteredKey = true;
			_settings.Save();
		}

		public void RemoveRegisteredLicenseKey() {
			if (HasRegisteredLicenseKey()) {
				_settings.LicenseKey = null;
				_settings.HasRegisteredKey = false;
				_settings.Save();
			}
		}

		public bool HasLicenseOverrideCommand() {
			return _settings.LicenseOverrideCommand != null;
		}

		public ProductLicenseCommand GetLicenseOverrideCommand() {
			if (_settings.LicenseOverrideCommand == null)
				throw new SoftwareException("No license override command has been found");
			return _settings.LicenseOverrideCommand;
		}

		public void SetLicenseOverrideCommand(ProductLicenseCommand value) {
			_settings.LicenseOverrideCommand = value;
			_settings.Save();
		}

		public void RemoveLicenseOverrideCommand() {
			_settings.LicenseOverrideCommand = null;
			_settings.Save();
		}
	}

}

