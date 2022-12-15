//-----------------------------------------------------------------------
// <copyright file="ApplicationForm.cs" company="Sphere 10 Software">
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
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Reflection;
using System.Threading;
using Hydrogen;
using Hydrogen.Application;
using Hydrogen.Windows.Forms;
using Microsoft.Extensions.DependencyInjection;

namespace Hydrogen.Windows.Forms {

    public class ApplicationForm : FormEx {
		private readonly IFuture<IWinFormsApplicationServices> _winFormsApplicationServices;
        public ApplicationForm() {
	        _winFormsApplicationServices = Tools.Values.Future.LazyLoad( () => HydrogenFramework.Instance.ServiceProvider.GetService<IWinFormsApplicationServices>());
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        protected IWinFormsApplicationServices WinFormsApplicationServices => _winFormsApplicationServices.Value;

        protected override void PopulatePrimingData() {
            base.PopulatePrimingData();
            Text = WinFormsApplicationServices.ProductInformation.ProcessTokensInString(this.Text);
            var useSettingsAttribute = this.GetType().GetCustomAttributesOfType<UseSettingsAttribute>().SingleOrDefault();
        }

    }
}
