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


namespace Hydrogen.Windows.Forms {

    public class ApplicationForm : FormEx {

        public ApplicationForm() {
            ApplicationServices = new WinFormsApplicationServices();
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        protected IWinFormsApplicationServices ApplicationServices { get; private set; }

        protected override void PopulatePrimingData() {
            base.PopulatePrimingData();
            Text = ApplicationServices.ProductInformation.ProcessTokensInString(this.Text);
            var useSettingsAttribute = this.GetType().GetCustomAttributesOfType<UseSettingsAttribute>().SingleOrDefault();
        }

    }
}
