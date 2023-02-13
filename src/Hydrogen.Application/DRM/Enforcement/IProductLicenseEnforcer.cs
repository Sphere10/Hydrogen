using System;
using System.Collections;
using System.Collections.Generic;

namespace Hydrogen.Application;

public interface IProductLicenseEnforcer {
	bool ValidateLicense(ProductLicenseActivationDTO licenseActivation, ProductLicenseAuthorityDTO licenseAuthority);
	bool ValidateLicenseCommand(ProductLicenseActivationDTO licenseActivation, SignedItem<ProductLicenseCommandDTO> command);
	void EnforceLicense(bool suppressNag);
	ProductRights CalculateRights(out string nagMessage);
	void HandleEnforcementError(Exception error, bool isDrmServerError);
	void ClearDrmServerErrors();

}