//-----------------------------------------------------------------------
// <copyright file="IStateChangeEventSource.cs" company="Sphere 10 Software">
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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Hydrogen.Windows.Forms {
	public interface IControlStateEventProvider {
		event EventHandlerEx StateChanged;
		void SetControl(Control control);
		void Clear();
	}

	public interface IControlStateEventProvider<out TControl> : IControlStateEventProvider where TControl : Control {
		TControl Control { get;}
		
	}
}
