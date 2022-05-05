//-----------------------------------------------------------------------
// <copyright file="StandardLicenseKeyValidatorWithVersionCheck.cs" company="Sphere 10 Software">
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

namespace Hydrogen.Application {
	public class StandardLicenseKeyValidatorWithVersionCheck : StandardLicenseKeyValidator {

		public StandardLicenseKeyValidatorWithVersionCheck(IProductInformationServices productInformationServices, ILicenseKeyDecoder licenseDecoder) 
			: base (licenseDecoder) {
			ProductInformationServices = productInformationServices;
		}

		protected IProductInformationServices ProductInformationServices { get; private set; }

		
		public override bool PerformAdditionalValidation(ProductLicense license) {
			bool valid = false;
			if (license.VersionAware) {
				if (StandardLicenseTool.GetProductMajorVersion(ProductInformationServices.ProductInformation.ProductVersion) == license.MajorVersionApplicable) {
					valid = true;
				}
			} else {
				valid = true;
			}
			return valid;
		}
		

	}
}
