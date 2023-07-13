// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using Hydrogen;
using Hydrogen.Application;
using Newtonsoft.Json;

namespace Sphere10.DRM;

/// <summary>
/// Stores DRM licenses as product settings.
/// </summary>
public class ProductLicenseSettingsStorage : IProductLicenseStorage {

	private readonly DRMSettings _settings;

	public ProductLicenseSettingsStorage(ISettingsServices settingsServices) {
		SettingsServices = settingsServices;
		_settings = SettingsServices.SystemSettings.Get<DRMSettings>();
	}

	protected ISettingsServices SettingsServices { get; }

	public bool TryGetDefaultLicense(out ProductLicenseActivationDTO licenseActivation) {
		licenseActivation = HydrogenAssemblyAttributesHelper.GetAssemblyDefaultProductLicenseActivation();
		if (licenseActivation == null)
			return false;
		return true;
	}
	public bool TryGetActivatedLicense(out ProductLicenseActivationDTO licenseActivation) {
		if (_settings.LicenseActivationJson == null) {
			licenseActivation = null;
			return false;
		}
		licenseActivation = Tools.Json.ReadFromString<ProductLicenseActivationDTO>(_settings.LicenseActivationJson);
		return true;
	}

	public void SaveActivatedLicense(ProductLicenseActivationDTO licenseActivation) {
		Guard.ArgumentNotNull(licenseActivation, nameof(licenseActivation));
		_settings.LicenseActivationJson = Tools.Json.WriteToString(licenseActivation);
		_settings.Save();
	}

	public void RemoveActivatedLicense() {
		_settings.LicenseActivationJson = null;
		_settings.Save();
	}

	public void SaveOverrideCommand(SignedItem<ProductLicenseCommandDTO> command) {
		if (_settings.LicenseActivationJson == null)
			throw new InvalidOperationException("No license has been activated");
		var licenseActivation = Tools.Json.ReadFromString<ProductLicenseActivationDTO>(_settings.LicenseActivationJson);
		licenseActivation.Command = command;
		_settings.LicenseActivationJson = Tools.Json.WriteToString(licenseActivation);
		_settings.Save();
	}


	public class DRMSettings : SettingsObject {

		[JsonProperty("licenseActivation")]
		[EncryptedString]
		public string LicenseActivationJson { get; set; }

	}

}
