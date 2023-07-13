// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Drawing;

namespace Hydrogen.Windows.Forms;

#warning Add Option Dialogs
#warning Add 8x8 special icon (outlook bar bottom)


public interface IApplicationBlock : IDisposable {
	int Position { get; }
	string Name { get; }
	IMenu[] Menus { get; }
	Image Image32x32 { get; }
	Image Image8x8 { get; }
	string HelpFileCHM { get; }
	bool ShowInMenuStrip { get; }
	bool ShowInToolStrip { get; }

	Type DefaultScreen { get; }
}
