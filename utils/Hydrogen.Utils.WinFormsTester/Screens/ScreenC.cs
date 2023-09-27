// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using Hydrogen.Windows.Forms;

namespace Hydrogen.Utils.WinFormsTester;

public partial class ScreenC : ApplicationScreen {
	public ScreenC() {
		InitializeComponent();
		taskPane1.Padding.Bottom = 0;
		taskPane1.Padding.Left = 0;
		taskPane1.Padding.Right = 0;
		taskPane1.Padding.Top = 0;

	}
}
