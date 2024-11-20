// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT NON-AI software license, see the accompanying file
// LICENSE or visit https://sphere10.com/legal/NON-AI-MIT.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System.Threading;
using System.Threading.Tasks;

namespace Hydrogen;

public static class EventHandlerExExtensions {

	public static void InvokeAsync(this EventHandlerEx handler)
		=> Tools.Threads.RaiseAsync(handler);

	public static void InvokeAsync<T>(this EventHandlerEx<T> handler, T arg)
		=> Tools.Threads.RaiseAsync(handler, arg);

	public static void InvokeAsync<T1, T2>(this EventHandlerEx<T1, T2> handler, T1 arg1, T2 arg2)
		=> Tools.Threads.RaiseAsync(handler, arg1, arg2);

	public static void InvokeAsync<T1, T2, T3>(this EventHandlerEx<T1, T2, T3> handler, T1 arg1, T2 arg2, T3 arg3)
		=> Tools.Threads.RaiseAsync(handler, arg1, arg2, arg3);

	public static Task WaitNext(this EventHandlerEx @event) => WaitNext(@event, CancellationToken.None);

	public static Task WaitNext(this EventHandlerEx @event, CancellationToken token) {
		var tcs = new TaskCompletionSource();
		@event += () => tcs.SetResult();
		token.Register(tcs.SetCanceled);
		return tcs.Task;
	}

	public static Task<T> WaitNext<T>(this EventHandlerEx<T> @event) => WaitNext(@event, CancellationToken.None);

	public static Task<T> WaitNext<T>(this EventHandlerEx<T> @event, CancellationToken token) {
		var tcs = new TaskCompletionSource<T>();
		@event += tcs.SetResult;
		token.Register(tcs.SetCanceled);
		return tcs.Task;
	}

	public static Task<(T1, T2)> WaitNext<T1, T2>(this EventHandlerEx<T1, T2> @event) => WaitNext(@event, CancellationToken.None);

	public static Task<(T1, T2)> WaitNext<T1, T2>(this EventHandlerEx<T1, T2> @event, CancellationToken token) {
		var tcs = new TaskCompletionSource<(T1, T2)>();
		@event += (t1, t2) => tcs.SetResult((t1, t2));
		token.Register(tcs.SetCanceled);
		return tcs.Task;
	}

	public static Task<(T1, T2, T3)> WaitNext<T1, T2, T3>(this EventHandlerEx<T1, T2, T3> @event) => WaitNext(@event, CancellationToken.None);

	public static Task<(T1, T2, T3)> WaitNext<T1, T2, T3>(this EventHandlerEx<T1, T2, T3> @event, CancellationToken token) {
		var tcs = new TaskCompletionSource<(T1, T2, T3)>();
		@event += (t1, t2, t3) => tcs.SetResult((t1, t2, t3));
		token.Register(tcs.SetCanceled);
		return tcs.Task;
	}

	public static Task<(T1, T2, T3, T4)> WaitNext<T1, T2, T3, T4>(this EventHandlerEx<T1, T2, T3, T4> @event) => WaitNext(@event, CancellationToken.None);

	public static Task<(T1, T2, T3, T4)> WaitNext<T1, T2, T3, T4>(this EventHandlerEx<T1, T2, T3, T4> @event, CancellationToken token) {
		var tcs = new TaskCompletionSource<(T1, T2, T3, T4)>();
		@event += (t1, t2, t3, t4) => tcs.SetResult((t1, t2, t3, t4));
		token.Register(tcs.SetCanceled);
		return tcs.Task;
	}

	public static Task<(T1, T2, T3, T4, T5)> WaitNext<T1, T2, T3, T4, T5>(this EventHandlerEx<T1, T2, T3, T4, T5> @event) => WaitNext(@event, CancellationToken.None);

	public static Task<(T1, T2, T3, T4, T5)> WaitNext<T1, T2, T3, T4, T5>(this EventHandlerEx<T1, T2, T3, T4, T5> @event, CancellationToken token) {
		var tcs = new TaskCompletionSource<(T1, T2, T3, T4, T5)>();
		@event += (t1, t2, t3, t4, t5) => tcs.SetResult((t1, t2, t3, t4, t5));
		token.Register(tcs.SetCanceled);
		return tcs.Task;
	}

}
