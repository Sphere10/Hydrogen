//-----------------------------------------------------------------------
// <copyright file="DefaultWizardScreen.cs" company="Sphere 10 Software">
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
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sphere10.Framework;
using Sphere10.Framework.Windows.Forms;
using Sphere10.FrameworkTester.WinForms.Wizard;

namespace Sphere10.FrameworkTester.WinForms {

    // This base class is needed to stop WinForms designer from throwing. This class cannot be designed by it's descendents can. This is due to the generic base.
    public class DemoWizardScreenBase  : WizardScreen<DemoWizardModel> { 
    }

}
