// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using Hydrogen.Windows.Forms;

namespace Hydrogen.Utils.WinFormsTester;

public partial class ScreenB : ApplicationScreen {
	public ScreenB() {
		InitializeComponent();

		listMerger1.LeftHeader = "Left Stuff";
		listMerger1.LeftItems = new object[] { "L1", "L2", "L3" };
		listMerger1.RightHeader = "Right Stuff";
		listMerger1.RightItems = new object[] { "R1", "R2", "R3" };
	}
}
