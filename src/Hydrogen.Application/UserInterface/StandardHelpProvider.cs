// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

namespace Hydrogen.Application;

public class StandardHelpServices : IHelpServices {

	public StandardHelpServices(IWebsiteLauncher websiteLauncher, IProductInformationProvider productInformationProvider, IProductUsageServices productUsageServices) {
		WebsiteLauncher = websiteLauncher;
		ProductInformationProvider = productInformationProvider;
		ProductUsageServices = productUsageServices;
	}

	public IProductUsageServices ProductUsageServices { get; private set; }

	public IProductInformationProvider ProductInformationProvider { get; private set; }

	public IWebsiteLauncher WebsiteLauncher { get; set; }

	public void ShowContextHelp(IHelpableObject helpableObject) {
		ShowHelp();
	}

	public void ShowHelp() {

		WebsiteLauncher.LaunchProductWebsite();
	}

	public virtual HelpType PreferredHelpProvider {
		get { return HelpType.URL; }
	}


}
