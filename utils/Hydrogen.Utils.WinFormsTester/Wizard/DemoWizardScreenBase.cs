// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hydrogen;
using Hydrogen.Utils.WinFormsTester.Wizard;
using Hydrogen.Windows.Forms;

namespace Hydrogen.Utils.WinFormsTester {

    // This base class is needed to stop WinForms designer from throwing. This class cannot be designed by it's descendents can. This is due to the generic base.
    public class DemoWizardScreenBase  : WizardScreen<DemoWizardModel> { 
    }

}
