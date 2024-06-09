// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT NON-AI software license, see the accompanying file
// LICENSE or visit https://sphere10.com/legal/NON-AI-MIT.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace Hydrogen;

/// <summary>
/// A synchronization mechanism similar to the <see cref="Monitor"/> but with "condition-based locking". This can be used by a producer/consumer queue to synchronize
/// access to resource. Readers "block until" the collection has items and writers "block until" the collection has free space.
///
/// The constructs are provided by <see cref="BlockUntil"/> and <see cref="DelayUntil"/> which return a scope that functions conceptually similar to a C# "lock" scope except
/// with a condition.
///
/// 
/// </summary>
/// <remarks>
/// lock(obj AND CONDITION) { 
///     ... code has exclusive lock on obj and condition is true
/// }
/// </remarks>
public class ProducerConsumerLock : IDisposable {
	private readonly object _sharedLock;
	private volatile bool _disposed;

	private readonly SemaphoreSlim _producerSemaphore;
	private readonly SemaphoreSlim _consumerSemaphore;

	public ProducerConsumerLock()
		: this(new object()) {
	}

	public ProducerConsumerLock(object sharedLock) {
		_sharedLock = sharedLock;
		_disposed = false;
		_producerSemaphore = new(1);
		_consumerSemaphore = new(1);
	}

	public object SharedLock => _sharedLock;

	public IDisposable ProducerBlockUntil(Func<bool> condition) => BlockUntil(_producerSemaphore, condition);

	public IDisposable ConsumerBlockUntil(Func<bool> condition) => BlockUntil(_consumerSemaphore, condition);

	public IDisposable BlockUntil(SemaphoreSlim semaphore, Func<bool> condition) {
		Guard.ArgumentNotNull(condition, nameof(condition));
		var threadID = Thread.CurrentThread.ManagedThreadId;
		var role = semaphore == _producerSemaphore ? ProducerConsumerType.Producer : ProducerConsumerType.Consumer;
		//Tools.Debugger.AddMessage($"{role} TID:{threadID} waiting");
		semaphore.Wait(); // note: if MANY more threads than cores start piling up here, this can become very slow!
		//Tools.Debugger.AddMessage($"{role} TID:{threadID} entered");
		try {
			Monitor.Enter(_sharedLock);
			while (!condition() || _disposed) {
				if (_disposed)
					throw new InvalidOperationException($"{nameof(ProducerConsumerLock)} disposed whilst thread {Thread.CurrentThread.ManagedThreadId} was waiting in queue");
				Monitor.Pulse(_sharedLock);
				Monitor.Wait(_sharedLock);
				Debug.Assert(Thread.CurrentThread.ManagedThreadId == threadID, "Different thread in lock loop, can lead to forever locks");
			}
		} catch {
			if (Monitor.IsEntered(_sharedLock)) {
				Monitor.Pulse(_sharedLock);
				Monitor.Exit(_sharedLock);
			}
			semaphore.Release();
			throw;
		}


		return new ActionScope(
			() => {
				Debug.Assert(Thread.CurrentThread.ManagedThreadId == threadID, "Different thread terminating access scope, can lead to forever locks");
				Monitor.Pulse(_sharedLock); // This is needed since other call queues may be connected with _lock, waiting for whatever this operation does 
				Monitor.Exit(_sharedLock);
				semaphore.Release();
				//Tools.Debugger.AddMessage($"{role} TID:{threadID} exited");
			}
		);
	}
	public async Task<IDisposable> ProducerDelayUntil(Func<bool> condition) {
		return await Task.Run(() => ProducerBlockUntil(condition));
	}

	public async Task<IDisposable> ConsumerDelayUntil(Func<bool> condition) {
		return await Task.Run(() => ConsumerBlockUntil(condition));
	}

	public void Pulse() {
		lock (_sharedLock)
			Monitor.Pulse(_sharedLock);
	}

	public void Dispose() {
		if (_disposed)
			return;
		_disposed = true;
		lock (_sharedLock)
			Monitor.PulseAll(_sharedLock);
		Tools.Exceptions.ExecuteIgnoringException(_producerSemaphore.Dispose);
		Tools.Exceptions.ExecuteIgnoringException(_consumerSemaphore.Dispose);
	}

}
