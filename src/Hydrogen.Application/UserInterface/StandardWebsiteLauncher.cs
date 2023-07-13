// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System.Diagnostics;

namespace Hydrogen.Application;

public class StandardWebsiteLauncher : IWebsiteLauncher {

	public StandardWebsiteLauncher(IProductInformationProvider productProvider) {
		ProductProvider = productProvider;

	}

	public IProductInformationProvider ProductProvider { get; private set; }

	public void LaunchWebsite(string url) {
		Process.Start(new ProcessStartInfo {
			FileName = $"{url.Trim('"')}",
			UseShellExecute = true,
		});
	}

	public void LaunchCompanyWebsite() {
		LaunchWebsite(ProductProvider.ProductInformation.CompanyUrl);
	}

	public void LaunchProductWebsite() {
		LaunchWebsite(ProductProvider.ProductInformation.ProductUrl);
	}

	public void LaunchProductPurchaseWebsite() {
		LaunchWebsite(ProductProvider.ProductInformation.ProductPurchaseUrl);
	}


}
