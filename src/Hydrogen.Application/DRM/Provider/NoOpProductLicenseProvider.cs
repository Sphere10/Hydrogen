// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

namespace Hydrogen.Application;

public class NoOpProductLicenseProvider : IProductLicenseProvider {

	public bool TryGetLicense(out ProductLicenseActivationDTO licenseActivation) {
		licenseActivation = null;
		return false;
	}

	public ProductRights CalculateRights() => ProductRights.None;

	public bool ValidateLicense(ProductLicenseActivationDTO licenseActivation) => false;

	public void ClearActivatedLicense() {
	}
}
