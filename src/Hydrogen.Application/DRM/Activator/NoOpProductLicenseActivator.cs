// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System.Threading.Tasks;

namespace Hydrogen.Application;

internal class NoOpProductLicenseActivator : IProductLicenseActivator {

	public Task ActivateLicense(string productKey)
		=> Task.CompletedTask;

	public Task ApplyLicense(ProductLicenseActivationDTO licenseActivation) => Task.CompletedTask;

}
