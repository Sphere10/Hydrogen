// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT NON-AI software license, see the accompanying file
// LICENSE or visit https://sphere10.com/legal/NON-AI-MIT.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;


/// <summary>
/// A custom synchronization context that executes all queued actions on the same thread.
/// </summary>
public class SameThreadSynchronizationContext : SynchronizationContext {
	private readonly BlockingCollection<Action> _queue = new BlockingCollection<Action>();

	/// <summary>
	/// Dispatches an asynchronous message to the synchronization context.
	/// </summary>
	/// <param name="d">The <see cref="SendOrPostCallback"/> delegate to call.</param>
	/// <param name="state">The object passed to the delegate.</param>
	public override void Post(SendOrPostCallback d, object state) {
		// Add the delegate to be invoked (along with its state) to the queue of actions
		_queue.Add(() => d(state));
	}

	/// <summary>
	/// Starts an event loop on the current thread, and processes all 
	/// actions added through <see cref="Post(SendOrPostCallback, object)"/>.
	/// </summary>
	public void RunOnCurrentThread() {
		// Execute all actions in the queue on the current thread
		foreach (var action in _queue.GetConsumingEnumerable()) {
			action();
		}
	}

	/// <summary>
	/// Executes a specified asynchronous method using this synchronization context.
	/// </summary>
	/// <param name="asyncMethod">The asynchronous method to execute.</param>
	/// <returns>A <see cref="Task"/> that represents the execution of the asynchronous method.</returns>
	public static async Task Run(Func<Task> asyncMethod) {
		// Create an instance of this synchronization context
		var context = new SameThreadSynchronizationContext();

		// Set this context to be the current synchronization context
		SynchronizationContext.SetSynchronizationContext(context);

		// Start the async method
		var t = asyncMethod();

		// When the async method finishes, mark the queue as complete,
		// signalling that no more actions will be added to it
		t.ContinueWith(_ => context._queue.CompleteAdding());

		// Start the event loop on the current thread
		context.RunOnCurrentThread();

		// Wait for the async method to complete
		await t;
	}
}
