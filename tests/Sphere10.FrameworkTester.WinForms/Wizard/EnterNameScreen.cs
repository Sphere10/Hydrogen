//-----------------------------------------------------------------------
// <copyright file="WizardDialog1.cs" company="Sphere 10 Software">
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
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Sphere10.Framework.Windows.Forms;
using Sphere10.Framework;

namespace Sphere10.FrameworkTester.WinForms.Wizard {
    public partial class EnterNameScreen : DemoWizardScreenBase {
        public EnterNameScreen() {
            InitializeComponent();
        }

        public override async Task Initialize() {
        }

        public override async Task<Result> Validate() {
            if (!checkBox1.Checked)
                return Result.Error("Checkbox not checked");

            return Result.Default;
        }
	}
}
