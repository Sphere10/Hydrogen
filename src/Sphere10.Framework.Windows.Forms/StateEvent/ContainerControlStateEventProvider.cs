//-----------------------------------------------------------------------
// <copyright file="CommonControlStateChangeManager.cs" company="Sphere 10 Software">
//
// Copyright (c) Sphere 10 Software. All rights reserved. (http://www.sphere10.com)
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// <author>Herman Schoenfeld</author>
// <date>2018</date>
// </copyright>
//-----------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace Sphere10.Framework.Windows.Forms {
    public class ContainerControlStateEventProvider : ControlStateEventProviderBase<Control> {

        private IDictionary<Control, IControlStateEventProvider> _childEventProviders;

        public ContainerControlStateEventProvider() {
            _childEventProviders = new Dictionary<Control, IControlStateEventProvider>();
        }

        protected override void RegisterStateChangedListener(Control control, EventHandlerEx eventHandler) {
            foreach (Control child in control.Controls)
                TryObserveChild(child, eventHandler);
            control.ControlAdded += (_, e) => TryObserveChild(e.Control, eventHandler);
            control.ControlRemoved += (_, e) => TryUnobserveChild(e.Control, eventHandler);
        }

        protected override void DeregisterStateChangedListener(Control control, EventHandlerEx eventHandler) {
            foreach (Control child in _childEventProviders.Keys.ToArray()) {
                TryUnobserveChild(control, eventHandler);
            }
        }


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
}
