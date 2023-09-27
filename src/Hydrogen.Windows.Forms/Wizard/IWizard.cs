// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Hydrogen.Windows.Forms;

public interface IWizard<T> {
	string Title { get; set; }
	T Model { get; }
	bool HasNext { get; }
	bool HasPrevious { get; }
	bool HideNext { get; set; }
	bool HidePrevious { get; set; }
	string NextText { get; set; }

	Task<WizardResult> Start(Form parent);

	Task Next();

	Task Previous();

	Result CancelRequested();

	Task InjectScreen(WizardScreen<T> screen);

	void RemoveScreen(WizardScreen<T> screen);

	void RemoveSubsequentScreensOfType(Type type);

	void RemoveSubsequentScreensOfType<U>();

}
