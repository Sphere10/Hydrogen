using System;
using System.Threading.Tasks;

namespace Hydrogen.Application;

public class ClientBackgroundLicenseVerifier : BackgroundLicenseVerifierBase {

	public ClientBackgroundLicenseVerifier(IUserInterfaceServices userInterfaceServices,  IProductLicenseStorage productLicenseStorage, IProductLicenseClient licenseClient, IProductInformationProvider productInformationProvider,  IProductLicenseProvider productLicenseProvider, IProductLicenseActivator productLicenseActivator, IProductLicenseEnforcer productLicenseEnforcer)
		: base(userInterfaceServices, productLicenseStorage, productInformationProvider, productLicenseProvider, productLicenseActivator, productLicenseEnforcer) {
		LicenseClient = licenseClient;
	}

	protected IProductLicenseClient LicenseClient { get; }

	protected override async Task<Result<SignedItem<ProductLicenseCommandDTO>>> NotifyServerOfLicenseUsage(Guid productCode, string productKey, string machineName, string[] macAddresses) {
		var result = await LicenseClient.NotifyLicenseUsage(productCode, productKey, machineName, macAddresses);
		return result;
	}

}