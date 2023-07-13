// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;

namespace Hydrogen.Application;

public class ProductLicenseEnforcerWithDisplay : IProductLicenseEnforcer {
	private readonly IProductLicenseEnforcer _internalEnforcer;

	public ProductLicenseEnforcerWithDisplay(IUserInterfaceServices userInterfaceServices, IProductLicenseEnforcer internalEnforcer) {
		UserInterfaceServices = userInterfaceServices;
		_internalEnforcer = internalEnforcer;
	}

	private IUserInterfaceServices UserInterfaceServices { get; }


	public bool ValidateLicense(ProductLicenseActivationDTO licenseActivation, ProductLicenseAuthorityDTO licenseAuthority)
		=> _internalEnforcer.ValidateLicense(licenseActivation, licenseAuthority);

	public bool ValidateLicenseCommand(ProductLicenseActivationDTO licenseActivation, SignedItem<ProductLicenseCommandDTO> command)
		=> _internalEnforcer.ValidateLicenseCommand(licenseActivation, command);


	public void HandleEnforcementError(Exception error, bool isDrmServerError) {
		UserInterfaceServices.ReportError(error);
		_internalEnforcer.HandleEnforcementError(error, isDrmServerError);
	}

	public void ClearDrmServerErrors() => _internalEnforcer.ClearDrmServerErrors();

	public void EnforceLicense(bool suppressNag)
		=> _internalEnforcer.EnforceLicense(suppressNag);

	public ProductRights CalculateRights(out string nagMessage)
		=> _internalEnforcer.CalculateRights(out nagMessage);

}
