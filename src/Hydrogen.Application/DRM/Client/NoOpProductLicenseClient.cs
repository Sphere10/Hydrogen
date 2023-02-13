using System;
using System.Threading.Tasks;

namespace Hydrogen.Application;

public class NoOpProductLicenseClient : IProductLicenseClient {

	public Task<Result<ProductLicenseDTO>> GetLicenseAsync(string productKey) => Task.FromResult<Result<ProductLicenseDTO>>(default);

	public Task<Result<ProductLicenseActivationDTO>> ActivateLicenseAsync(Guid productCode, string productKey, string machineName, string[] macAddresses) {
		throw new NotImplementedException();
	}

	public Task<Result<SignedItem<ProductLicenseCommandDTO>>> NotifyLicenseUsage(Guid productCode, string productKey, string machineName, string[] macAddresses) {
		throw new NotImplementedException();
	}

}
