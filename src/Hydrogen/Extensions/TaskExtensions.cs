// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT NON-AI software license, see the accompanying file
// LICENSE or visit https://sphere10.com/legal/NON-AI-MIT.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace Hydrogen;

public static class TaskExtensions {

	/// <summary>
	/// Creates a continuation that executes asynchronously on the same thread when the target <paramref name="task"/> completes.
	/// </summary>
	/// <param name="task">The target <see cref="Task"/>.</param>
	/// <returns>A new continuation <see cref="Task"/>.</returns>
	public static Task ContinueOnSameThread(this Task task) {
		var tcs = new TaskCompletionSource<bool>();

		// Start a new operation on the custom synchronization context.
		// This operation will wait for the original task to complete, 
		// and then set the result or exception on the TaskCompletionSource.
		// It does NOT need to be awaited.
		SameThreadSynchronizationContext.Run(async () => {
			try {
				// Await the original task.
				await task;

				// If the original task completes successfully, set the result on the TaskCompletionSource.
				tcs.SetResult(true);
			} catch (Exception e) {
				// If the original task throws an exception, set it on the TaskCompletionSource.
				tcs.SetException(e);
			}
		});

		// Return the Task from the TaskCompletionSource.
		// When the original task completes, this task will also complete.
		return tcs.Task;
	}

	/// <summary>
	/// Creates a continuation that executes asynchronously on the same thread when the target <paramref name="task"/> completes.
	/// This version is for tasks that return a value.
	/// </summary>
	/// <typeparam name="T">The type of the result produced by the target <paramref name="task"/>.</typeparam>
	/// <param name="task">The target <see cref="Task{TResult}"/>.</param>
	/// <returns>A new continuation <see cref="Task{TResult}"/>.</returns>
	public static Task<T> ContinueOnSameThread<T>(this Task<T> task) {
		var tcs = new TaskCompletionSource<T>();

		// Start a new operation on the custom synchronization context.
		// This operation will wait for the original task to complete, 
		// and then set the result or exception on the TaskCompletionSource.
		// It does NOT need to be awaited.
		SameThreadSynchronizationContext.Run(async () => {
			try {
				// Await the original task.
				T result = await task;

				// If the original task completes successfully, set the result on the TaskCompletionSource.
				tcs.SetResult(result);
			} catch (Exception e) {
				// If the original task throws an exception, set it on the TaskCompletionSource.
				tcs.SetException(e);
			}
		});

		// Return the Task from the TaskCompletionSource.
		// When the original task completes, this task will also complete.
		return tcs.Task;
	}

	/// <summary>
	/// Wraps the task with a try/catch that ignores all exceptions.
	/// </summary>
	/// <param name="task">The task</param>
	/// <returns>Wrapped task</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Task IgnoringExceptions(this Task task, ICollection<Exception> captureList = null)
		=> task.IgnoringExceptionOfType<Exception>(captureList);

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Task<T> IgnoringExceptions<T>(this Task<T> task, ICollection<Exception> captureList = null)
		=> task.IgnoringExceptionOfType<T, Exception>(captureList);

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Task IgnoringCancellationException(this Task task, ICollection<Exception> captureList = null)
		=> task.IgnoringExceptionOfType<OperationCanceledException>(captureList);

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Task<T> IgnoringCancellationException<T>(this Task<T> task, ICollection<Exception> captureList = null)
		=> task.IgnoringExceptionOfType<T, OperationCanceledException>(captureList);

	public static async Task IgnoringExceptionOfType<TException>(this Task task, ICollection<Exception> captureList = null) where TException : Exception {
		try {
			await task;
		} catch (TException exception) {
			captureList?.Add(exception);
		}
	}

	public static async Task<T> IgnoringExceptionOfType<T, TException>(this Task<T> task, ICollection<Exception> captureList = null) where TException : Exception {
		T result;
		try {
			result = await task;
		} catch (TException exception) {
			result = default;
			captureList?.Add(exception);
		}
		return result;
	}


	/// <summary>
	/// Wraps the task with an exception handler.
	/// </summary>
	/// <param name="task">The task</param>
	/// <param name="exceptionHandler">The catch block.</param>
	/// <returns></returns>
	public static Task WithExceptionHandler(this Task task, Action<Exception> exceptionHandler)
		=> task.WithRetry(0, (_, e) => exceptionHandler(e));

	/// <summary>
	/// Wraps the task with an exception handler.
	/// </summary>
	/// <param name="task">The task</param>
	/// <param name="exceptionHandler">The catch block.</param>
	/// <returns></returns>
	public static Task<T> WithExceptionHandler<T>(this Task<T> task, Action<Exception> exceptionHandler)
		=> task.WithRetry(0, (_, e) => exceptionHandler(e));

	/// <summary>
	/// Wraps an action with retry fail-over code.
	/// </summary>
	/// <param name="task">The task</param>
	/// <param name="retryCount">Number of attempts to retry upon failure</param>
	/// <param name="failAction">Action to execute when a failure occurs (e.g. could log, sleep, etc)</param>
	/// <param name="onComplete">Action to execute when task completes</param>
	/// <returns></returns>
	public static Task WithRetry(this Task task, int retryCount, Action<int, Exception> failAction = null, Action<int> onComplete = null)
		=> task.WithFailOver(
			(attempt, error) => {
				failAction?.Invoke(attempt, error);
				return attempt < retryCount + 1;
			},
			onComplete
		);

	/// <summary>
	/// Wraps an action with retry fail-over code.
	/// </summary>
	/// <param name="task">The task</param>
	/// <param name="retryCount">Number of attempts to retry upon failure</param>
	/// <param name="failAction">Action to execute when a failure occurs (e.g. could log, sleep, etc)</param>
	/// <param name="onComplete">Action to execute when task completes</param>
	/// <returns></returns>
	public static Task<T> WithRetry<T>(this Task<T> task, int retryCount, Action<int, Exception> failAction = null, Action<int> onComplete = null)
		=> task.WithFailOver(
			(attempt, error) => {
				failAction?.Invoke(attempt, error);
				return attempt < retryCount + 1;
			},
			onComplete
		);

	/// <summary>
	/// Adds fail-over redundancy code to the task.
	/// </summary>
	/// <param name="task">The task.</param>
	/// <param name="decideRetry">Functor to decide whether or not to retry. Parameters are attempt number, last Exception and returns true/false.</param>
	/// <param name="onComplete">Action to execute when task completes</param>
	/// <param name="attempt">The attempt</param>
	/// <returns>The given task wrapped with fail-over code.</returns>
	public static async Task WithFailOver(this Task task, Func<int, Exception, bool> decideRetry, Action<int> onComplete = null, int attempt = 1) {
		try {
			await task;
			onComplete?.Invoke(attempt);
		} catch (Exception error) {
			if (decideRetry(attempt, error))
				await WithFailOver(task, decideRetry, onComplete, ++attempt);
		}
	}

	/// <summary>
	/// Adds fail-over redundancy code to the task.
	/// </summary>
	/// <param name="task">The task.</param>
	/// <param name="decideRetry">Functor to decide whether or not to retry. Parameters are attempt number, last Exception and returns true/false.</param>
	/// <param name="onComplete">Action to execute when task completes</param>
	/// <param name="attempt">The attempt count of this fail-over</param>
	/// <returns>The given task wrapped with fail-over code.</returns>
	public static async Task<T> WithFailOver<T>(this Task<T> task, Func<int, Exception, bool> decideRetry, Action<int> onComplete = null, int attempt = 1) {
		try {
			var result = await task;
			onComplete?.Invoke(attempt);
			return result;
		} catch (Exception error) {
			if (decideRetry(attempt, error))
				return await WithFailOver(task, decideRetry, onComplete, ++attempt);
			throw;
		}
	}

	public static Task WithTimeout(this Task task, int delayMS)
		=> WithTimeout(task, TimeSpan.FromMilliseconds(delayMS));

	public static Task WithTimeout(this Task task, TimeSpan delay)
		=> WithCancellationToken(task, new CancellationTokenSource(delay).Token);


	public static Task<T> WithTimeout<T>(this Task<T> task, int delayMS)
		=> WithTimeout(task, TimeSpan.FromMilliseconds(delayMS));

	public static Task<T> WithTimeout<T>(this Task<T> task, TimeSpan delay)
		=> WithCancellationToken(task, new CancellationTokenSource(delay).Token);


	/// <summary>
	/// Makes a non-cancellable task cancellable.
	/// </summary>
	/// <typeparam name="T">Type of task's result</typeparam>
	/// <param name="task">The long-running task</param>
	/// <param name="cancellationToken">The cancellation token which triggers the cancel.</param>
	/// <returns>The task result</returns>
	/// <remarks>Inspired by https://johnthiriet.com/cancel-asynchronous-operation-in-csharp/</remarks>
	public static async Task WithCancellationToken(this Task task, CancellationToken cancellationToken) {
		// We create a TaskCompletionSource of decimal
		var taskCompletionSource = new TaskCompletionSource();

		// Registering a lambda into the cancellationToken
		cancellationToken.Register(() => {
			// We received a cancellation message, cancel the TaskCompletionSource.Task
			taskCompletionSource.TrySetCanceled();
		});

		// Wait for the first task to finish among the two
		var completedTask = await Task.WhenAny(task, taskCompletionSource.Task);

		// If the completed task is our long running operation we set its result.
		if (completedTask == task) {

			// Set the taskCompletionSource result
			taskCompletionSource.TrySetResult();
		} else {
			// TODO: ABORT task?? 
		}
		// Note a cancellation exception is thrown if the completedTask was the taskCompletionSource

		// Return the result of the TaskCompletionSource.Task
		await taskCompletionSource.Task;
	}

	public static async Task<T> WithCancellationToken<T>(this Task<T> task, CancellationToken cancellationToken) {
		// We create a TaskCompletionSource of decimal
		var taskCompletionSource = new TaskCompletionSource<T>();

		// Registering a lambda into the cancellationToken
		cancellationToken.Register(() => {
			// We received a cancellation message, cancel the TaskCompletionSource.Task
			taskCompletionSource.TrySetCanceled();
		});

		// Wait for the first task to finish among the two
		var completedTask = await Task.WhenAny(task, taskCompletionSource.Task);

		// If the completed task is our long running operation we set its result.
		if (completedTask == task) {
			// Extract the result, the task is finished and the await will return immediately
			var result = await task;

			// Set the taskCompletionSource result
			taskCompletionSource.TrySetResult(result);
		}
		// Note a cancellation exception is thrown if the completedTask was the taskCompletionSource

		// Return the result of the TaskCompletionSource.Task
		return await taskCompletionSource.Task;
	}

	public static void WaitSafe(this Task task) {
		Task.Run(task.Wait);
	}

	public static void WaitSafe(this Task task, CancellationToken cancellationToken) {
		Task.Run(() => task.Wait(cancellationToken), cancellationToken);
	}

	public static T ResultSafe<T>(this Task<T> task) {
		return Task.Run(() => task.Result).Result;
	}
	public static T ResultSafe<T>(this Task<T> task, CancellationToken cancellationToken) {
		return Task.Run(() => task.Result, cancellationToken).Result;
	}

	public static async Task WithAggregateException(this Task task, bool unwrapSingleAggregateException = true) {
		try {
			await task;
		} catch (Exception) {
			if (task.Exception == null)
				throw;
			if (unwrapSingleAggregateException && task.Exception.InnerExceptions.Count == 1) {
				throw task.Exception.InnerExceptions[0];
			}
			throw task.Exception;
		}
	}
}
