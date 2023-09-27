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

public abstract class ControlStateEventProviderBase<TControl> : IControlStateEventProvider<TControl> where TControl : Control {
	public event EventHandlerEx StateChanged;


	public Type ControlType => typeof(TControl);

	public TControl Control { get; private set; }

	public void Clear() {
		if (Control != null)
			DeregisterStateChangedListener(Control, NotifyStateChanged);
		Control = null;
	}


	public void SetControl(Control control) {
		Guard.ArgumentCast<TControl>(control, out var tctrl, nameof(control));
		if (Control != null)
			Clear();
		Control = tctrl;
		RegisterStateChangedListener(Control, NotifyStateChanged);
	}

	protected abstract void RegisterStateChangedListener(TControl control, EventHandlerEx eventHandler);

	protected abstract void DeregisterStateChangedListener(TControl control, EventHandlerEx eventHandler);

	private void NotifyStateChanged() {
		StateChanged?.Invoke();
	}
}
