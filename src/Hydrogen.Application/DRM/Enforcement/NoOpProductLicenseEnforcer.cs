using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hydrogen.Application;

namespace Hydrogen.Application;

internal class NoOpProductLicenseEnforcer : IProductLicenseEnforcer {
	public int CountLicenseEnforcements => 0;
	public int CountNagged => 0;

	public bool ValidateLicense(ProductLicenseActivationDTO licenseActivation, ProductLicenseAuthorityDTO licenseAuthority) => false;

	public bool ValidateLicenseCommand(ProductLicenseActivationDTO licenseActivation, SignedItem<ProductLicenseCommandDTO> command) => false;

	public void EnforceLicense(bool suppressNag) {
	}

	public ProductRights CalculateRights(out string nagMessage) {
		nagMessage = null;
		return ProductRights.None;
	}

	public void HandleEnforcementError(Exception error, bool isDrmServerError) {
		// NoOp
	}

	public void ClearDrmServerErrors() {
		// No op
	}


	public ProductRights Rights => ProductRights.None;
}