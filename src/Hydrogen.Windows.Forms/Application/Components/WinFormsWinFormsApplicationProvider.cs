//// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
//// Author: Herman Schoenfeld
////
//// Distributed under the MIT software license, see the accompanying file
//// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
////
//// This notice must not be removed when duplicating this file or its contents, in whole or in part.

//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using Hydrogen.Application;
//using System.Drawing;
//using Hydrogen;

//namespace Hydrogen.Windows.Forms {

//	public class WinFormsWinFormsApplicationProvider : ApplicationProvider, IApplicationIconProvider{

//		public WinFormsWinFormsApplicationProvider(IProductInformationProvider productInformationProvider, IProductUsageServices productUsageServices, IProductInstancesCounter productInstancesCounter, ISettingsServices settingsServices, /*ILicenseServices licenseServices, IProductLicenseEnforcer productLicenseEnforcer,*/ IHelpServices helpServices, IUserInterfaceServices userInterfaceServices, IUserNotificationServices userNotificationServices, IWebsiteLauncher websiteLauncher, IAutoRunServices autoRunServices, IApplicationIconProvider applicationIconProvider)
//			: base(productInformationProvider, productUsageServices, productInstancesCounter, settingsServices,/* licenseServices, productLicenseEnforcer,*/ helpServices, userInterfaceServices, userNotificationServices, websiteLauncher, autoRunServices) {
//			ApplicationIconProvider = applicationIconProvider;
//		}

//		private IApplicationIconProvider ApplicationIconProvider { get; }


//		public Icon ApplicationIcon => ApplicationIconProvider.ApplicationIcon;

//	}
//}



