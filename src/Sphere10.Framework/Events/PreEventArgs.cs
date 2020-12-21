using System;
using System.Data;

namespace Sphere10.Framework {

	public class PreEventArgs : EventArgs {
	}


	public class PreEventArgs<TArgs> : PreEventArgs
		where TArgs : CallArgs {
		public PreEventArgs() {
		}

		public TArgs CallArgs { get; set; }
	}


}