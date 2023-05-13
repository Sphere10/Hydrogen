// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hydrogen.Windows.Forms.Components.BlockFramework {
	public class HideScreenEventArgs : EventArgs {
		private bool _cancel;
		public HideScreenEventArgs() {
			Cancel = false;
		}

		public bool Cancel {
			get => _cancel;
			set => _cancel = _cancel || value;
		}

	}
}
