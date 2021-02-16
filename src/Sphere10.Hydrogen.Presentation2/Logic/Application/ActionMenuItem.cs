using System;

namespace Sphere10.Hydrogen.Presentation2.Logic {

	public class ActionMenuItem : ApplicationMenuItem {
		
		public Action Action { get; } = null;

		protected override void OnSelect() {
			base.OnSelect();
			Action?.Invoke();
		}

	}

}
