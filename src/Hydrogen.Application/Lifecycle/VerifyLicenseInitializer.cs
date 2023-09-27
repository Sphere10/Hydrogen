// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

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
