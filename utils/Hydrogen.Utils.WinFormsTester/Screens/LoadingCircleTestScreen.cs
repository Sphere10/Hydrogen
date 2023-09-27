// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Threading.Tasks;
using Hydrogen.Windows.Forms;

namespace Hydrogen.Utils.WinFormsTester;

public partial class LoadingCircleTestScreen : ApplicationScreen {
	public LoadingCircleTestScreen() {
		InitializeComponent();
	}

	private async void button1_Click(object sender, EventArgs e) {
		using (LoadingCircle.EnterAnimationScope(_panel)) {
			await Task.Delay(2000);
		}
	}

	private async void button2_Click(object sender, EventArgs e) {
		loadingCircle1.StartAnimating();
		await Task.Delay(2000);
		loadingCircle1.StopAnimating();

	}
}
