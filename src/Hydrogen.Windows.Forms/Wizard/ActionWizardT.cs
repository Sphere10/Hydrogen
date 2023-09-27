// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Hydrogen.Windows.Forms;

public class ActionWizard : ActionWizard<IDictionary<string, object>> {
	public ActionWizard(string title, IDictionary<string, object> propertyBag, IEnumerable<WizardScreen<IDictionary<string, object>>> forms, Func<IDictionary<string, object>, Task<Result>> finishFunc,
	                    Func<IDictionary<string, object>, Result> cancelFunc = null)
		: base(title, propertyBag, forms, finishFunc, cancelFunc) {
	}
}
