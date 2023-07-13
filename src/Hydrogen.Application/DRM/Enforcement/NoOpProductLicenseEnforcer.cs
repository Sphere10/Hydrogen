// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;

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
