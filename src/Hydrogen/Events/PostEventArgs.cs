// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;

namespace Hydrogen {

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