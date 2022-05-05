//-----------------------------------------------------------------------
// <copyright file="BITSBackgroundLicenseVerifier.cs" company="Sphere 10 Software">
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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Net.NetworkInformation;
using Hydrogen.Application;

namespace Hydrogen.Windows.Forms {
	public class BITSBackgroundLicenseVerifier : StandardBackgroundLicenseVerifier {

		public BITSBackgroundLicenseVerifier(IProductInformationServices productInformationServices, IWebsiteLauncher websiteLauncher, ILicenseServices licenseServices, ILicenseEnforcer licenseEnforcer)
			: base(productInformationServices, websiteLauncher, licenseServices, licenseEnforcer) {

		}
		
		public override void VerifyLicense() {
			var verifyJob = new BITSLicenseCommandDownloadJob(
				base.LicenseServices,
				base.ProductInformationServices,
				base.GenerateVerificationUrl()
			);
			verifyJob.CommandDownloaded += (sender, downloadEvent) => base.ApplyLicenseCommand(downloadEvent.Command);

			// start the job now
			verifyJob.Start();
		}

	

	}
}
