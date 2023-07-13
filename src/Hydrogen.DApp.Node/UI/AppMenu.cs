// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System.ComponentModel;

namespace Hydrogen.DApp.Node.UI;

public enum AppMenu {
	[Description("_File")] File,

	[Description("_Wallet")] Wallet,

	[Description("_Settings")] Settings,

	[Description("_Explorer")] Explorer,

	[Description("D_iagnostic")] Diagnostic,

	[Description("_Development")] Development,
}
