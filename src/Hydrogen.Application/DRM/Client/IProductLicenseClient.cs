using System;
using System.Threading.Tasks;

namespace Hydrogen.Application;

public interface IProductLicenseClient {

	Task<Result<ProductLicenseDTO>> GetLicenseAsync(string productKey);

	Task<Result<ProductLicenseActivationDTO>> ActivateLicenseAsync(Guid productCode, string productKey, string machineName, string[] macAddresses);

	Task<Result<SignedItem<ProductLicenseCommandDTO>>> NotifyLicenseUsage(Guid productCode, string productKey, string machineName, string[] macAddresses);

}