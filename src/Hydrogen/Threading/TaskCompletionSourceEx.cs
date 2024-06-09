// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT NON-AI software license, see the accompanying file
// LICENSE or visit https://sphere10.com/legal/NON-AI-MIT.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

#if NETSTANDARD

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using VOID = Hydrogen.Void;

namespace Hydrogen {

	/// <summary>
	/// TaskCompletionSource is missing from .NET Standard. An implementation which decorates an <see cref="TaskCompletionSource{TResult}"/> is used.
	/// </summary>
	/// <remarks>The post-fix `Ex` is required to avoid ambiguities in NET 5+ projects which reference this namespace.</remarks>
	public class TaskCompletionSourceEx {
		private readonly TaskCompletionSource<VOID> _internal;

		public TaskCompletionSourceEx() {
			_internal = new TaskCompletionSource<VOID>();
		}

		public TaskCompletionSourceEx(TaskCreationOptions creationOptions) {
			_internal = new TaskCompletionSource<VOID>(creationOptions);
		}

		public TaskCompletionSourceEx(object? state) {
			_internal = new TaskCompletionSource<VOID>(state);
		}

		public TaskCompletionSourceEx(object? state, TaskCreationOptions creationOptions) {
			_internal = new TaskCompletionSource<VOID>(state, creationOptions);
		}

		public Task Task => _internal.Task;

		public void SetException(Exception exception) => _internal.SetException(exception);

		public void SetException(IEnumerable<Exception> exceptions) => _internal.SetException(exceptions);

		public bool TrySetException(Exception exception) => _internal.TrySetException(exception);

		public bool TrySetException(IEnumerable<Exception> exceptions) => _internal.TrySetException(exceptions);

		public void SetResult() => _internal.SetResult(VOID.Value);

		public bool TrySetResult() => _internal.TrySetResult(VOID.Value);

		public void SetCanceled() => _internal.SetCanceled();

		public void SetCanceled(CancellationToken cancellationToken) {
			// note: there is no _internal.SetCancelled(cancellationToken)
			if (!TrySetCanceled(cancellationToken)) {
				throw new InvalidOperationException("An attempt was made to transition a task to a final state when it had already completed.");
			}
		}

		public bool TrySetCanceled() => _internal.TrySetCanceled();

		public bool TrySetCanceled(CancellationToken cancellationToken) => _internal.TrySetCanceled(cancellationToken);
	}
}

#endif
