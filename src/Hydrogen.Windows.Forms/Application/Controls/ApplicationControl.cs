//-----------------------------------------------------------------------
// <copyright file="ApplicationControl.cs" company="Sphere 10 Software">
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

		private readonly IFuture<IWinFormsApplicationServices> _winFormsApplicationServices;
        public ApplicationControl() {
	        _winFormsApplicationServices = Tools.Values.Future.LazyLoad( () => HydrogenFramework.Instance.ServiceProvider.GetService<IWinFormsApplicationServices>());
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        protected IWinFormsApplicationServices ApplicationServices => _winFormsApplicationServices.Value;
	
	}
}
