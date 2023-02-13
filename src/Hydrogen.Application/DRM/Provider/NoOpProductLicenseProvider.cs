using System;

namespace Hydrogen.Application;

public class NoOpProductLicenseProvider : IProductLicenseProvider {

	public bool TryGetLicense(out ProductLicenseActivationDTO licenseActivation) {
		licenseActivation = null;
		return false;
	}

	public ProductRights CalculateRights() => ProductRights.None;

	public bool ValidateLicense(ProductLicenseActivationDTO licenseActivation) => false;

	public void ClearActivatedLicense() {
	}
}
