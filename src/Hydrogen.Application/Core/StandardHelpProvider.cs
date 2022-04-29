//-----------------------------------------------------------------------
// <copyright file="StandardHelpProvider.cs" company="Sphere 10 Software">
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

namespace Sphere10.Framework.Application {


	public class StandardHelpServices : IHelpServices  {

		public StandardHelpServices(IWebsiteLauncher websiteLauncher, IProductInformationServices productInformationServices, IProductUsageServices productUsageServices) {
			WebsiteLauncher = websiteLauncher;
            ProductInformationServices = productInformationServices;
            ProductUsageServices = productUsageServices;
		}

        public IProductUsageServices ProductUsageServices { get; private set; }

        public IProductInformationServices ProductInformationServices { get; private set; }

		public IWebsiteLauncher WebsiteLauncher { get; set; }

		public void ShowContextHelp(IHelpableObject helpableObject) {
			ShowHelp();
		}

		public void ShowHelp() {

			WebsiteLauncher.LaunchProductWebsite();
		}

        public virtual HelpType PreferredHelpProvider
        {
            get
            {
                return HelpType.URL;
            }
        }



	}
}
