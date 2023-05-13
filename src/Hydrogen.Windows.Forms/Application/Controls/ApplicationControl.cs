// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;

using System.Windows.Forms;
using System.Globalization;
using Hydrogen;
using Hydrogen.Application;
using Microsoft.Extensions.DependencyInjection;

namespace Hydrogen.Windows.Forms {

	/// <summary>
	/// A base class for all controls in the application. Provides access to application services and 
	/// features like automatically detect child control state changes. Draws theme-aware borders.
	/// </summary>
	public partial class ApplicationControl : UserControlEx {

		//private readonly IFuture<IWinFormsApplicationProvider> _winFormsApplicationServices;

		private readonly IFuture<IUserInterfaceServices> _userInterfaceServices;

        public ApplicationControl() {
	        //_winFormsApplicationServices = Tools.Values.Future.LazyLoad( () => HydrogenFramework.Instance.ServiceProvider.GetService<IWinFormsApplicationProvider>());
	        if (!Tools.Runtime.IsDesignMode) {
				SettingsServices = HydrogenFramework.Instance.ServiceProvider.GetService<ISettingsServices>();
				_userInterfaceServices = Tools.Values.Future.LazyLoad( () =>  HydrogenFramework.Instance.ServiceProvider.GetService<IUserInterfaceServices>());
			}
        }

		//[Browsable(false)]
		//[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		//protected IWinFormsApplicationProvider ApplicationProvider => _winFormsApplicationServices.Value;

		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		protected ISettingsServices SettingsServices { get; }

		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		protected IUserInterfaceServices UserInterfaceServices => _userInterfaceServices.Value;
	}
}
