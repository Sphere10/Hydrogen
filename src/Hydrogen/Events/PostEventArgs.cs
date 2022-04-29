using System;

namespace Sphere10.Framework {

	public class PostEventArgs : EventArgs {
	}

	public class PostEventArgs<TArgs> : PostEventArgs {
		public TArgs CallArgs { get; set; }
	}

	public class PostEventArgs<TArgs, TResult> : PostEventArgs<TArgs>
		where TArgs : CallArgs {
		public PostEventArgs(TResult result = default) {
			Result = result;
		}
		public TResult Result { get; set; }
	}

	

}