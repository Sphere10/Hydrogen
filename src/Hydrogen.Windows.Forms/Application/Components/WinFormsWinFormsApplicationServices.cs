//-----------------------------------------------------------------------
// <copyright file="WinFormsWinFormsApplicationServices.cs" company="Sphere 10 Software">
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
using Hydrogen.Application;
using System.Drawing;
using Hydrogen;

namespace Hydrogen.Windows.Forms {

	public class WinFormsWinFormsApplicationServices : ApplicationServices, IWinFormsApplicationServices{

		public WinFormsWinFormsApplicationServices(IProductInformationServices productInformationServices, IProductUsageServices productUsageServices, IProductInstancesCounter productInstancesCounter, IConfigurationServices configurationServices, ILicenseServices licenseServices, ILicenseEnforcer licenseEnforcer, IHelpServices helpServices, IUserInterfaceServices userInterfaceServices, IUserNotificationServices userNotificationServices, IWebsiteLauncher websiteLauncher, IAutoRunServices autoRunServices, IApplicationIconProvider applicationIconProvider)
			: base(productInformationServices, productUsageServices, productInstancesCounter, configurationServices, licenseServices, licenseEnforcer, helpServices, userInterfaceServices, userNotificationServices, websiteLauncher, autoRunServices) {
			ApplicationIconProvider = applicationIconProvider;
		}

		private IApplicationIconProvider ApplicationIconProvider { get; }


		public Icon ApplicationIcon => ApplicationIconProvider.ApplicationIcon;

	}
}


