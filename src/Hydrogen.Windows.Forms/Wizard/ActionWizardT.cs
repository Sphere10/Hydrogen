//-----------------------------------------------------------------------
// <copyright file="ActionWizardManager.cs" company="Sphere 10 Software">
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
using Hydrogen;

namespace Hydrogen.Windows.Forms {

    public class ActionWizard : ActionWizard<IDictionary<string, object>> {
        public ActionWizard(string title, IDictionary<string, object> propertyBag, IEnumerable<WizardScreen<IDictionary<string, object>>> forms, Func<IDictionary<string, object>, Task<Result>> finishFunc, Func<IDictionary<string, object>, Result> cancelFunc = null)
            : base(title, propertyBag, forms, finishFunc, cancelFunc) {
        }
    }
}
