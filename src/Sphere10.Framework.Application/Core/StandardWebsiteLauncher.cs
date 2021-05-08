//-----------------------------------------------------------------------
// <copyright file="StandardWebsiteLauncher.cs" company="Sphere 10 Software">
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

using System.Diagnostics;

namespace Sphere10.Framework.Application {

	public class StandardWebsiteLauncher : IWebsiteLauncher {

		public StandardWebsiteLauncher(IProductInformationServices productServices) {
			ProductServices = productServices;

		}

		public IProductInformationServices ProductServices { get; private set; }

		public void LaunchWebsite(string url) {
			Process.Start(url);
		}

		public void LaunchCompanyWebsite() {
			LaunchWebsite(ProductServices.ProductInformation.CompanyUrl);
		}

		public void LaunchProductWebsite() {
			LaunchWebsite(ProductServices.ProductInformation.ProductUrl);
		}

		public void LaunchProductPurchaseWebsite() {
			LaunchWebsite(ProductServices.ProductInformation.ProductPurchaseUrl);
		}

		

	}
}
