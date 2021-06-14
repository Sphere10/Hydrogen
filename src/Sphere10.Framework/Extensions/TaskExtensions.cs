//-----------------------------------------------------------------------
// <copyright file="TaskExtensions.cs" company="Sphere 10 Software">
//
// Copyright (c) Sphere 10 Software. All rights reserved. (http://www.sphere10.com)
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// <author>Herman Schoenfeld</author>
// <date>2018</date>
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Threading;
using System.Threading.Tasks;

namespace Sphere10.Framework {

	public static class TaskExtensions {


		public static Task<T> IgnoringCancellationException<T>(this Task<T> task)
			=> IgnoringException<T, OperationCanceledException>(task);


		public static Task<T> IgnoringExceptions<T>(this Task<T> task)
			=> IgnoringException<T, Exception>(task);

		public static async Task<T> IgnoringException<T, TException>(this Task<T> task) where TException : Exception {
			Guard.ArgumentNotNull(task, nameof(task));
			T result;
			try {
				result = await task;
			} catch (TException) {
				result = default;
			}
			return result;
		}

        /// <summary>
        /// Makes a non-cancellable task cancellable.
        /// </summary>
        /// <typeparam name="T">Type of task's result</typeparam>
        /// <param name="task">The long-running task</param>
        /// <param name="cancellationToken">The cancellation token which triggers the cancel.</param>
        /// <returns>The task result</returns>
        /// <remarks>Inspired by https://johnthiriet.com/cancel-asynchronous-operation-in-csharp/</remarks>
        public static async Task<T> WithCancellationToken<T>(this Task<T> task, CancellationToken cancellationToken) {
            // We create a TaskCompletionSource of decimal
            var taskCompletionSource = new TaskCompletionSource<T>();

            // Registering a lambda into the cancellationToken
            cancellationToken.Register(() =>
            {
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
            Task.Run(() => task.Wait());
        }

        public static void WaitSafe(this Task task, CancellationToken cancellationToken) {
            Task.Run(() => task.Wait(cancellationToken));
        }

        public static void RunSyncronouslySafe(this Task task) {
            Task.Run(() => task.RunSynchronously());
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
}
