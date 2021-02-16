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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Net.NetworkInformation;
using Sphere10.Framework;

namespace Sphere10.Framework.Application {
	public class NoOpBackgroundLicenseVerifier : IBackgroundLicenseVerifier {
		public void VerifyLicense() {
			// No op
		}
	}
}
