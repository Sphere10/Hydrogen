// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System.Windows.Forms;

namespace Hydrogen.Windows.Forms;

public interface IControlStateEventProvider {
	event EventHandlerEx StateChanged;

	void SetControl(Control control);

	void Clear();
}


public interface IControlStateEventProvider<out TControl> : IControlStateEventProvider where TControl : Control {
	TControl Control { get; }

}
