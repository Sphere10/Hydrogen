using System;

namespace Hydrogen {

	public class PreEventArgs : EventArgs {
	}


	public class PreEventArgs<TArgs> : PreEventArgs
		where TArgs : CallArgs {
		public PreEventArgs() {
		}

		public TArgs CallArgs { get; set; }
	}


}