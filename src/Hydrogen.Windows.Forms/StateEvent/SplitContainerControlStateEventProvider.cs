// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System.Windows.Forms;

namespace Hydrogen.Windows.Forms;

public class SplitContainerControlStateEventProvider : ControlStateEventProviderBase<Control> {
	private ContainerControlStateEventProvider _panel1Listener;
	private ContainerControlStateEventProvider _panel2Listener;


	public SplitContainerControlStateEventProvider() {
	}

	protected override void RegisterStateChangedListener(Control control, EventHandlerEx eventHandler) {
		Guard.ArgumentCast<SplitContainer>(control, out var splitContainer, nameof(control));
		_panel1Listener = new ContainerControlStateEventProvider();
		_panel2Listener = new ContainerControlStateEventProvider();
		_panel1Listener.SetControl(splitContainer.Panel1);
		_panel2Listener.SetControl(splitContainer.Panel2);
		_panel1Listener.StateChanged += eventHandler;
		_panel2Listener.StateChanged += eventHandler;
	}

	protected override void DeregisterStateChangedListener(Control control, EventHandlerEx eventHandler) {
		_panel1Listener.StateChanged -= eventHandler;
		_panel2Listener.StateChanged -= eventHandler;
	}


}
