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

public class ActionWizard<T> : WizardBase<T> {
	private readonly Func<T, Task<Result>> _finishFunc;
	private readonly Func<T, Result> _cancelFunc;
	private readonly IEnumerable<WizardScreen<T>> _screens;

	public ActionWizard(string title, T propertyBag, IEnumerable<WizardScreen<T>> screens, Func<T, Task<Result>> finishFunc, Func<T, Result> cancelFunc = null)
		: base(title, propertyBag) {
		_finishFunc = finishFunc;
		_cancelFunc = cancelFunc ?? ((x) => Result.Default);
		_screens = screens;
	}

	public override Result CancelRequested() {
		return _cancelFunc(Model);
	}

	protected override IEnumerable<WizardScreen<T>> ConstructScreens() {
		return _screens;
	}

	protected override async Task<Result> Finish() {
		return await _finishFunc(Model);
	}
}
