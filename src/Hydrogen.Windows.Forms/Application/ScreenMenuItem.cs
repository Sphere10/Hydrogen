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

public class ScreenMenuItem : LinkMenuItem, IScreenMenuItem {
	private Type _screen;

	public ScreenMenuItem()
		: this(string.Empty, null) {
	}

	public ScreenMenuItem(string text, Type viewType)
		: this(text, viewType, null) {
	}


	public ScreenMenuItem(string text, Type screenType, Image image16x16)
		: this(text, screenType, image16x16, true, true, false) {
	}

	public ScreenMenuItem(string text, Type screenType, Image image16x16, bool showOnExplorerBar, bool showOnToolBar, bool isStartScreen)
		: base(text, image16x16, showOnExplorerBar, showOnToolBar, isStartScreen) {
		_screen = screenType;
	}

	public virtual Type Screen {
		get { return _screen; }
		set { _screen = value; }
	}

	public override void Dispose() {
		base.Dispose();
	}
}
