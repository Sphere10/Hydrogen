using System.ComponentModel;
using System.Threading.Tasks;

namespace Hydrogen.Application;

public interface IProductLicenseActivator {

	Task ActivateLicense(string productKey);

	Task ApplyLicense(ProductLicenseActivationDTO licenseActivation);

}