//-----------------------------------------------------------------------
// <copyright file="StandardBackgroundLicenseVerifier.cs" company="Sphere 10 Software">
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
	public class NoOpBackgroundLicenseVerifier : IBackgroundLicenseVerifier {
		public void VerifyLicense() {
			// No op
		}
	}
}
