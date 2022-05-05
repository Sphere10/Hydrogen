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
