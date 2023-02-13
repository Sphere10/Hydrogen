namespace Hydrogen.Application;

public class NoOpProductLicenseStorage : IProductLicenseStorage {


	public bool TryGetDefaultLicense(out ProductLicenseActivationDTO licenseActivation) {
		licenseActivation = null;
		return false;
	}

	public bool TryGetActivatedLicense(out ProductLicenseActivationDTO licenseActivation) {
		licenseActivation = null;
		return false;
	}

	public void SaveActivatedLicense(ProductLicenseActivationDTO licenseActivation) {
	}

	public void RemoveActivatedLicense() {
	}

	public void SaveOverrideCommand(SignedItem<ProductLicenseCommandDTO> command) {
	}
}
