// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Hamish Rose
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;

namespace Hydrogen.DApp.Presentation2.Logic {

	public class ActionMenuItem : ApplicationMenuItem {

		public Action Action { get; } = null;

		protected override void OnSelect() {
			base.OnSelect();
			Action?.Invoke();
		}

	}

}
