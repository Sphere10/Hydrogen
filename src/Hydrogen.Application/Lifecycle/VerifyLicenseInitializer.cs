//-----------------------------------------------------------------------
// <copyright file="IncrementUsageByOneTask.cs" company="Sphere 10 Software">
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

using System;
using System.ComponentModel;
using System.Threading;

namespace Hydrogen.Application;

public class VerifyLicenseInitializer : ApplicationInitializerBase {
	public VerifyLicenseInitializer(IBackgroundLicenseVerifier backgroundLicenseVerifier, IUserInterfaceServices userInterfaceServices) {
		BackgroundLicenseVerifier = backgroundLicenseVerifier;
		UserInterfaceServices = userInterfaceServices;
	}

	protected IBackgroundLicenseVerifier BackgroundLicenseVerifier { get; }

	public IUserInterfaceServices UserInterfaceServices { get; }

	public override bool Parallelizable => true;

	public override int Priority => int.MaxValue; // run last

	public override async void Initialize() {
		await BackgroundLicenseVerifier.VerifyLicense(CancellationToken.None);
	}

}