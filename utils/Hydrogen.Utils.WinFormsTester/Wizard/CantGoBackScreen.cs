// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

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
    public partial class CantGoBackScreen : DemoWizardScreenBase {
        public CantGoBackScreen() {
            InitializeComponent();
        }

        public override async Task Initialize() {
        }

        public override async Task<Result> Validate() {
	        return Result.Valid;
        }

	}
}
