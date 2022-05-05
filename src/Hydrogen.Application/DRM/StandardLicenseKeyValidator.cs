//-----------------------------------------------------------------------
// <copyright file="StandardLicenseKeyValidator.cs" company="Sphere 10 Software">
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
	public class StandardLicenseKeyValidator : ILicenseKeyValidator {

		public StandardLicenseKeyValidator(ILicenseKeyDecoder licenseDecoder) {
			LicenseDecoder = licenseDecoder;
		}

		protected ILicenseKeyDecoder LicenseDecoder { get; private set; }

		public virtual bool IsValid(string key) {
			ProductLicense license;
			return LicenseDecoder.TryDecode(key, out license) && PerformAdditionalValidation(license);
		}


		public virtual bool PerformAdditionalValidation(ProductLicense license) {
			return true;
		}
		

	}
}

