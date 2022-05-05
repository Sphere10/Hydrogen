//-----------------------------------------------------------------------
// <copyright file="ILicenseEnforcer.cs" company="Sphere 10 Software">
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
	public interface ILicenseEnforcer {
		int CountAppliedLicense { get; }
		int CountNagged { get; }
		void ApplyLicense(bool nagUser = true);
		bool DetermineLicenseCompliance(out bool systemShouldNagUser, out string nagMessage);
		ProductRights Rights { get; }
	}
}
