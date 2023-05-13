// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

namespace Hydrogen.Application;

public class NoOpProductLicenseStorage : IProductLicenseStorage {


	public bool TryGetDefaultLicense(out ProductLicenseActivationDTO licenseActivation) {
		licenseActivation = null;
		return false;
	}

	public bool TryGetActivatedLicense(out ProductLicenseActivationDTO licenseActivation) {
		licenseActivation = null;
		return false;
	}

	public void SaveActivatedLicense(ProductLicenseActivationDTO licenseActivation) {
	}

	public void RemoveActivatedLicense() {
	}

	public void SaveOverrideCommand(SignedItem<ProductLicenseCommandDTO> command) {
	}
}
