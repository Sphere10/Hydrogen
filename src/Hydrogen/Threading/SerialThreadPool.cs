// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT NON-AI software license, see the accompanying file
// LICENSE or visit https://sphere10.com/legal/NON-AI-MIT.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Hydrogen;

/// <summary>
/// Allows the the serial execution of Actions in the order they are added. The background execution thread is not released until all the queued actions are processed.
/// </summary>
public class SerialThreadPool {

	private readonly SynchronizedQueue<Action> _inputQueue;
	private readonly Queue<Action> _executionQueue;
	private readonly Action<Exception> _errorHandler;
	private readonly object _lock;
	public SerialThreadPool(Action<Exception> errorHandler = null) : this(new QueueAdapter<Action>(), errorHandler) {
	}

	public SerialThreadPool(IQueue<Action> inputQueueImpl, Action<Exception> errorHandler = null) {
		_inputQueue = new SynchronizedQueue<Action>(inputQueueImpl);
		_executionQueue = new Queue<Action>();
		_errorHandler = errorHandler ?? (_ => { });
		_lock = new object();
	}

	public SerialThreadPoolPolicy Policy { get; init; } = SerialThreadPoolPolicy.Burst;

	public static SerialThreadPool Global { get; } = new(error => SystemLog.Exception(error));


	public void QueueUserWorkItem(Action action) {
		using (_inputQueue.EnterWriteScope()) {
			_inputQueue.Add(action); // the input queue can be added to whilst background thread is processing
			if (_inputQueue.Count == 1 && !Monitor.IsEntered(_lock))
				// Background execution thread is started only when input queue is 1 and execution queue is 0
				// Since this is the only condition when ProcessInBurst is not running and not scheduled to run.
				Tools.Threads.QueueAction(
					Policy switch {
						SerialThreadPoolPolicy.Burst => ProcessInBurst,
						_ => throw new NotImplementedException($"{Policy}")
					}
				);
		}
	}

	private void ProcessInBurst() {
		// Gadget 1: This ensures only 1 ProcessInBurst is ever entered (even if multiple get scheduled)
		if (!Monitor.TryEnter(_lock))
			return;

		try {
			// Gadget 2: In the below condition, _inputQueue.Count enters into a read-lock before Count is evaluated. As a result, any simultaneous
			// QueueUserWorkItem calls must necessarily complete before Count is evaluated since QueueUserWorkItem enters _inputQueue into a write-lock.
			// This gadget ensures the loop never leaves with an item in the _inputQueue.
			while (_inputQueue.Count > 0) {
				// transfer input queue into execution queue
				using (_inputQueue.EnterWriteScope()) {
					if (!_inputQueue.Any())
						return;
					foreach (var action in _inputQueue)
						_executionQueue.Enqueue(action);
					_inputQueue.Clear();
				}

				// process the execution queue serially
				while (_executionQueue.Count > 0) {
					var action = _executionQueue.Dequeue();
					try {
						action();
					} catch (Exception error) {
						_errorHandler(error);
					}
				}
			}
		} finally {
			Monitor.Exit(_lock);
		}


	}
}
