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

public class NoOpProductLicenseClient : IProductLicenseClient {

	public Task<Result<ProductLicenseDTO>> GetLicenseAsync(string productKey) => Task.FromResult<Result<ProductLicenseDTO>>(default);

	public Task<Result<ProductLicenseActivationDTO>> ActivateLicenseAsync(Guid productCode, string productKey, string machineName, string[] macAddresses) {
		throw new NotImplementedException();
	}

	public Task<Result<SignedItem<ProductLicenseCommandDTO>>> NotifyLicenseUsage(Guid productCode, string productKey, string machineName, string[] macAddresses) {
		throw new NotImplementedException();
	}

}
