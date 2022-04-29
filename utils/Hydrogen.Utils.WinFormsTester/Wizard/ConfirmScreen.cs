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
using Hydrogen.Windows.Forms;
using Hydrogen;

namespace Hydrogen.Utils.WinFormsTester.Wizard {
    public partial class ConfirmScreen : DemoWizardScreenBase {
        public ConfirmScreen() {
            InitializeComponent();
        }

        public override async Task Initialize() {
        }

        public override async Task<Result> Validate() {
            return Result.Default;
        }
    }
}
