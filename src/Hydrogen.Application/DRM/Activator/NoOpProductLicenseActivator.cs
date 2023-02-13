using System;
using System.Threading.Tasks;

namespace Hydrogen.Application;

internal class NoOpProductLicenseActivator : IProductLicenseActivator {

	public Task ActivateLicense(string productKey) 
		=> Task.CompletedTask;

	public Task ApplyLicense(ProductLicenseActivationDTO licenseActivation) => Task.CompletedTask;

}
