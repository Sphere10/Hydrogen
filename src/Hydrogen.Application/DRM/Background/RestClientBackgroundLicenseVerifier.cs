// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Threading.Tasks;

namespace Hydrogen.Application;

public class ClientBackgroundLicenseVerifier : BackgroundLicenseVerifierBase {

	public ClientBackgroundLicenseVerifier(IUserInterfaceServices userInterfaceServices, IProductLicenseStorage productLicenseStorage, IProductLicenseClient licenseClient, IProductInformationProvider productInformationProvider,
	                                       IProductLicenseProvider productLicenseProvider, IProductLicenseActivator productLicenseActivator, IProductLicenseEnforcer productLicenseEnforcer)
		: base(userInterfaceServices, productLicenseStorage, productInformationProvider, productLicenseProvider, productLicenseActivator, productLicenseEnforcer) {
		LicenseClient = licenseClient;
	}

	protected IProductLicenseClient LicenseClient { get; }

	protected override async Task<Result<SignedItem<ProductLicenseCommandDTO>>> NotifyServerOfLicenseUsage(Guid productCode, string productKey, string machineName, string[] macAddresses) {
		var result = await LicenseClient.NotifyLicenseUsage(productCode, productKey, machineName, macAddresses);
		return result;
	}

}
