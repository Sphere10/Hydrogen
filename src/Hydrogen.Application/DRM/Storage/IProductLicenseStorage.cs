//-----------------------------------------------------------------------
// <copyright file="IProductLicenseStorage.cs" company="Sphere 10 Software">
//
// Copyright (c) Sphere 10 Software. All rights reserved. (http://www.sphere10.com)
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// <author>Herman Schoenfeld</author>
// <date>2018</date>
// </copyright>
//-----------------------------------------------------------------------


using System.Runtime.InteropServices;

namespace Hydrogen.Application;

public interface IProductLicenseStorage {
	
	bool TryGetDefaultLicense(out ProductLicenseActivationDTO licenseActivation);

	bool TryGetActivatedLicense(out ProductLicenseActivationDTO licenseActivation);

	void SaveActivatedLicense(ProductLicenseActivationDTO licenseActivation);

	void RemoveActivatedLicense();

	void SaveOverrideCommand(SignedItem<ProductLicenseCommandDTO> command);

}