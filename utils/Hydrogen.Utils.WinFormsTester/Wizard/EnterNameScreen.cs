// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System.Threading.Tasks;

namespace Hydrogen.Utils.WinFormsTester.Wizard;

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
