//-----------------------------------------------------------------------
// <copyright file="HydrogenFrameworkExtensions.cs" company="Sphere 10 Software">
//
// Copyright (c) Sphere 10 Software. All rights reserved. (http://www.sphere10.com)
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// <author>Herman Schoenfeld</author>
// <date>2022</date>
// </copyright>
//-----------------------------------------------------------------------


using System.Linq;
using Hydrogen.Application;
using Microsoft.Extensions.Hosting;

namespace Hydrogen.Web.AspNetCore {
	public static class HydrogenFrameworkExtensions {
		private static IHost _host;
        public static void SetAspNetCoreHost(this HydrogenFramework framework, IHost host) 
			=> _host = host;

        public static IHost GetAspNetCoreHost(this HydrogenFramework framework) {
			CheckSet();
			return _host;
		}

		private static void CheckSet() 
			=> Guard.Ensure(_host != null, $"Hydrogen framework is not informed of the AspNetCore host. Please call {nameof(IHostExtensions.StartHydrogenFramework)} before running host.");

	}
}
