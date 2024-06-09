// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT NON-AI software license, see the accompanying file
// LICENSE or visit https://sphere10.com/legal/NON-AI-MIT.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System.Threading.Tasks;

namespace Hydrogen;

public static class ValueTaskExtensions {
	public static Task ContinueOnSameThread(this ValueTask task) {
		var tcs = new TaskCompletionSource<bool>();

		SameThreadSynchronizationContext.Run(async () => {
			await task;
			tcs.SetResult(true);
		});

		return tcs.Task;
	}

	public static Task<T> ContinueOnSameThread<T>(this ValueTask<T> task) {
		var tcs = new TaskCompletionSource<T>();

		SameThreadSynchronizationContext.Run(async () => {
			T result = await task;
			tcs.SetResult(result);
		});

		return tcs.Task;
	}
}
