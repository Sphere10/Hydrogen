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

using System.Windows.Forms;

namespace Hydrogen.Windows.Forms {
    public sealed class NoOpControlStateProvider : IControlStateEventProvider {
        public event EventHandlerEx StateChanged;

        public void Clear() {
            // do nothing
        }

        public void SetControl(Control control) {
            // do nothing
        }
    }
}
