// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace Hydrogen.Windows.Forms;

public class ContainerControlStateEventProviderBase<TControl> : ControlStateEventProviderBase<TControl> where TControl : Control {

	private readonly IDictionary<Control, IControlStateEventProvider> _childEventProviders;

	public ContainerControlStateEventProviderBase() {
		_childEventProviders = new Dictionary<Control, IControlStateEventProvider>();
	}

	protected override void RegisterStateChangedListener(TControl control, EventHandlerEx eventHandler) {
		foreach (Control child in GetChildControls(control))
			TryObserveChild(child, eventHandler);
		control.ControlAdded += (_, e) => TryObserveChild(e.Control, eventHandler);
		control.ControlRemoved += (_, e) => TryUnobserveChild(e.Control, eventHandler);
	}

	protected override void DeregisterStateChangedListener(TControl control, EventHandlerEx eventHandler) {
		foreach (Control child in _childEventProviders.Keys.ToArray()) {
			TryUnobserveChild(control, eventHandler);
		}
	}

	protected virtual IEnumerable<Control> GetChildControls(TControl control) => control.Controls.Cast<Control>();

	private bool TryObserveChild(Control control, EventHandlerEx eventHandler) {
		if (!_childEventProviders.ContainsKey(control) &&
		    ControlStateEventProviderManager.Instance.TryGetControlStateProvider(control.GetType(), out var provider)) {
			provider.SetControl(control);
			provider.StateChanged += eventHandler;
			_childEventProviders.Add(control, provider);
			return true;
		}
		return false;
	}

	private void TryUnobserveChild(Control control, EventHandlerEx eventHandler) {
		if (_childEventProviders.ContainsKey(control)) {
			var provider = _childEventProviders[control];
			provider.StateChanged -= eventHandler;
			provider.Clear();
			_childEventProviders.Remove(control);
		}
	}

}
