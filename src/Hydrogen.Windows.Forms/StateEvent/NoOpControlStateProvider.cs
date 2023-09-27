// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Windows.Forms;

namespace Hydrogen.Windows.Forms;

public sealed class NoOpControlStateProvider : IControlStateEventProvider {
	public event EventHandlerEx StateChanged;

	public void Clear() {
		// do nothing
	}

	public Type ControlType => typeof(Control);

	public void SetControl(Control control) {
		// do nothing
	}
}
