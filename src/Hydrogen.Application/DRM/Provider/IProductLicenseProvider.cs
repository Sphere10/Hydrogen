// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;

namespace Hydrogen.Application;

public interface IProductLicenseProvider {
	bool TryGetLicense(out ProductLicenseActivationDTO registeredLicense);

	ProductRights CalculateRights();

	void ClearActivatedLicense();

}


public static class ProductLicenseProviderExtensions {
	public static ProductLicenseActivationDTO GetLicense(this IProductLicenseProvider productLicenseProvider) {
		if (!productLicenseProvider.TryGetLicense(out var license))
			throw new InvalidOperationException("No license found");
		return license;
	}
}
