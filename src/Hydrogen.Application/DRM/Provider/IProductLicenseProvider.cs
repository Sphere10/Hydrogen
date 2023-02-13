using System;

namespace Hydrogen.Application;

public interface IProductLicenseProvider {
	bool TryGetLicense(out ProductLicenseActivationDTO registeredLicense);

	ProductRights CalculateRights();

	void ClearActivatedLicense();

}


public static class ProductLicenseProviderExtensions {
	public static ProductLicenseActivationDTO GetLicense(this IProductLicenseProvider productLicenseProvider) {
		if (!productLicenseProvider.TryGetLicense(out var license))
			throw new InvalidOperationException("No license found");
		return license;
	}
}