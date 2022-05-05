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
