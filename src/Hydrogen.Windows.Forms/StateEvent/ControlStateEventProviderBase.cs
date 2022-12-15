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

using System;
using System.Windows.Forms;

namespace Hydrogen.Windows.Forms {
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
}
