// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Hamish Rose
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

namespace Hydrogen.DApp.Presentation2.Logic {

	public abstract class ApplicationMenuItem : IApplicationMenuItem {
		public event EventHandlerEx Hover;
		public event EventHandlerEx Select;

		public string Icon { get; init; }

		public string Title { get; init; }

		protected virtual void OnHover() {
		}

		protected virtual void OnSelect() {
		}


		internal void NotifyHover() {
			Hover?.Invoke();
			OnHover();
		}

		internal void NotifySelect() {
			Select?.Invoke();
			OnSelect();
		}
	}

}
