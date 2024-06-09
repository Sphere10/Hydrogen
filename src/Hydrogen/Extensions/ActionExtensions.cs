// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT NON-AI software license, see the accompanying file
// LICENSE or visit https://sphere10.com/legal/NON-AI-MIT.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;

namespace Hydrogen;

public static class ActionExtensions {

	public static Func<T, Void> AsVoid<T>(this Action<T> action) {
		return arg => {
			action(arg);
			return Void.Value;
		};
	}


	/// <summary>
	/// Wraps the action with a try/catch that ignores all exceptions.
	/// </summary>
	/// <param name="action">The action.</param>
	/// <returns>Wrapped action</returns>
	public static Action IgnoringExceptions(this Action action) {
		return Tools.Lambda.ActionIgnoringExceptions(action);
	}

	/// <summary>
	/// Wraps the action with an exception handler.
	/// </summary>
	/// <param name="action">The action.</param>
	/// <param name="exceptionHandler">The catch block.</param>
	/// <returns></returns>
	public static Action WithExceptionHandler(this Action action, Action<Exception> exceptionHandler) {
		return Tools.Lambda.ActionWithExceptionHandler(action, exceptionHandler);
	}


	/// <summary>
	/// Returns the action to run asyncronously (queued in the ThreadPool).
	/// </summary>
	/// <param name="action">The action</param>
	/// <returns>Wrapped action.</returns>
	public static Action AsAsyncronous(this Action action) {
		return Tools.Lambda.ActionAsAsyncronous(action);
	}

	/// <summary>
	/// Returns the action to run asyncronously (queued in the ThreadPool).
	/// </summary>
	/// <param name="action">The action</param>
	/// <returns>Wrapped action.</returns>
	public static Action<T1> AsAsyncronous<T1>(this Action<T1> action) {
		return Tools.Lambda.ActionAsAsyncronous(action);
	}

	/// <summary>
	/// Returns the action to run asyncronously (queued in the ThreadPool).
	/// </summary>
	/// <param name="action">The action</param>
	/// <returns>Wrapped action.</returns>
	public static Action<T1, T2> AsAsyncronous<T1, T2>(this Action<T1, T2> action) {
		return Tools.Lambda.ActionAsAsyncronous(action);
	}

	/// <summary>
	/// Returns the action to run asyncronously (queued in the ThreadPool).
	/// </summary>
	/// <param name="action">The action</param>
	/// <returns>Wrapped action.</returns>
	public static Action<T1, T2, T3> AsAsyncronous<T1, T2, T3>(this Action<T1, T2, T3> action) {
		return Tools.Lambda.ActionAsAsyncronous(action);
	}

	/// <summary>
	/// Wraps an action with retry failover code.
	/// </summary>
	/// <param name="action">The action</param>
	/// <param name="retryCount">Number of attempts to retry upon failure</param>
	/// <param name="failAction">Action to execute when a failure occurs (e.g. could log, sleep, etc)</param>
	/// <param name="completedAction">Action to execute when action completes </param>
	/// <returns></returns>
	public static Action WithRetry(this Action action, int retryCount, Action<int, Exception> failAction = null, Action<int> completedAction = null) {
		return Tools.Lambda.ActionWithRetry(action, retryCount, failAction, completedAction);
	}


	/// <summary>
	/// Adds failover redundancy code to the action.
	/// </summary>
	/// <param name="action">The action.</param>
	/// <param name="decideRetry">Functor to decide whether or not to retry. Parameters are attempt, Exception and returns true/false.</param>
	/// <param name="attempt">The attempt.</param>
	/// <returns>The given action wrapped with failover code.</returns>
	public static Action WithFailOver(this Action action, Func<int, Exception, bool> decideRetry, Action<int> completedAction = null, int attempt = 1) {
		return Tools.Lambda.ActionWithFailOver(action, decideRetry, completedAction, attempt);
	}


}
