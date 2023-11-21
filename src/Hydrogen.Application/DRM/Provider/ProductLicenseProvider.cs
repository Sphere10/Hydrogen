// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

namespace Hydrogen.Application;

public class ProductLicenseProvider : IProductLicenseProvider {


	public ProductLicenseProvider(IProductLicenseStorage productLicenseStorage, IProductLicenseEnforcer productLicenseEnforcer) {
		Storage = productLicenseStorage;
		ProductLicenseEnforcer = productLicenseEnforcer;
	}
	protected IProductLicenseStorage Storage { get; }
	protected IProductLicenseEnforcer ProductLicenseEnforcer { get; }

	public bool TryGetLicense(out ProductLicenseActivationDTO licenseActivation) {
		Storage.TryGetActivatedLicense(out licenseActivation);
		Storage.TryGetDefaultLicense(out var defaultLicense);

		var authority = defaultLicense.Authority ?? licenseActivation.Authority; // The authority is the default one shipped with product, or the one provided by server if none (protects against MItM attack)		

		if (licenseActivation != null && !ProductLicenseEnforcer.ValidateLicense(licenseActivation, authority)) {
			Storage.RemoveActivatedLicense();
			licenseActivation = null;
			//throw new ProductLicenseTamperedException();
		}

		if (defaultLicense != null && !ProductLicenseEnforcer.ValidateLicense(defaultLicense, authority))
			throw new ProductLicenseTamperedException();


		licenseActivation ??= defaultLicense; // if no registered license, then the default license is what we want

		return true;
	}

	public virtual ProductRights CalculateRights() {
		var registeredLicense = Storage.TryGetActivatedLicense(out var licenseActivation) ? licenseActivation : null;
		var defaultLicense = Storage.TryGetDefaultLicense(out licenseActivation) ? licenseActivation : null;
		Guard.Against(registeredLicense != null && defaultLicense == null, "No default license was present yet a registered license was found");
		return ProductLicenseEnforcer.CalculateRights(out _);
	}

	public void ClearActivatedLicense() {
		Storage.RemoveActivatedLicense();
	}

}
