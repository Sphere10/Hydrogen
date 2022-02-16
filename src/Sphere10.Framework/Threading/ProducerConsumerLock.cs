//-----------------------------------------------------------------------
// <copyright file="CallQueue.cs" company="Sphere 10 Software">
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


// HS 2022-02-17: When using SemaphoreSlim (.NET 6), the producer semaphore seems to get pile up without letting threads on very heavy usage and intermittently. This issue was very difficult to
// reproduce since it occured intermittently and never appeared in Debug mode (only in Release mode). When adding logging around the BlockUntil in Release mode, it also seemed to make
// the issue go away. After WEEKS of extraordinary testing I conclude there must be some very weird bug in SemaphoreSlim which occurs when being blasted with multiple threads.
// As evidence of this, the issue goes way when using the slower Semaphore. I recommend that this define be commented out in future date after (hoping) Microsoft fix SemaphoreSlim implementation
// in .NET 6+. The unit test which reproduces the bug is ProducerConsumerTests.SemaphoreSlim_Bug.
#define USE_SLOW_SEMAPHORE

using System;
using System.Threading;
using System.Threading.Tasks;

namespace Sphere10.Framework {

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
#if USE_SLOW_SEMAPHORE
		private readonly Semaphore _producerSemaphore;
		private readonly Semaphore _consumerSemaphore;
#else
		private readonly SemaphoreSlim _producerSemaphore;
		private readonly SemaphoreSlim _consumerSemaphore;
#endif
		private volatile int _waitCount;
		
		public ProducerConsumerLock()
			: this(new object()) {
		}

		public ProducerConsumerLock(object sharedLock) {
			_sharedLock = sharedLock;
			_disposed = false;
#if USE_SLOW_SEMAPHORE
			_producerSemaphore = new(1,1, Guid.NewGuid().ToString());
			_consumerSemaphore = new(1,1, Guid.NewGuid().ToString());
#else
			_producerSemaphore = new(1);
			_consumerSemaphore = new(1);
#endif
			_waitCount = 0;
		}

		public object SharedLock => _sharedLock;

#if !USE_SLOW_SEMAPHORE
		public int Count => _producerSemaphore.CurrentCount + _consumerSemaphore.CurrentCount + _waitCount;
#endif

		public IDisposable ProducerBlockUntil(Func<bool> condition) => BlockUntil(_producerSemaphore, condition);

		public IDisposable ConsumerBlockUntil(Func<bool> condition) => BlockUntil(_consumerSemaphore, condition);

#if USE_SLOW_SEMAPHORE
		public IDisposable BlockUntil(Semaphore semaphore, Func<bool> condition) {
#else
		public IDisposable BlockUntil(SemaphoreSlim semaphore, Func<bool> condition) {
#endif
			Guard.ArgumentNotNull(condition, nameof(condition));

#if USE_SLOW_SEMAPHORE
			semaphore.WaitOne();
#else
			semaphore.Wait();
#endif
			try {
				Monitor.Enter(_sharedLock);
				while (!condition() || _disposed) {
					if (_disposed) {
						throw new InvalidOperationException($"{nameof(ProducerConsumerLock)} disposed whilst thread {Thread.CurrentThread.ManagedThreadId} was waiting in queue");
					}
					Monitor.Pulse(_sharedLock);
					Interlocked.Increment(ref _waitCount);
					Monitor.Wait(_sharedLock);
					Interlocked.Decrement(ref _waitCount);
				}
			} catch {
				Monitor.Pulse(_sharedLock);
				if (Monitor.IsEntered(_sharedLock)) {
					Monitor.Exit(_sharedLock);
				}

				semaphore.Release();
				throw;
			}


			return new ActionScope(
				() => {
					Monitor.Pulse(_sharedLock); // This is needed since other call queues may be connected with _lock, waiting for whatever this operation does 
					Monitor.Exit(_sharedLock);
					semaphore.Release();
				}
			);
		}
		public async Task<IDisposable> ProducerDelayUntil(ProducerConsumerType requestingRole, Func<bool> condition) {
			return await Task.Run(() => ProducerBlockUntil(condition));
		}

		public async Task<IDisposable> ConsumerDelayUntil(ProducerConsumerType requestingRole, Func<bool> condition) {
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


		/* Use below methods to diagnose issue in future

		public IDisposable ProducerBlockUntil(Func<bool> condition) {
			Guard.ArgumentNotNull(condition, nameof(condition));
			Tools.Debugger.ProducersWaitingSemaphore.Add(Thread.CurrentThread.ManagedThreadId);
			_producerSemaphore.Wait();
			Tools.Debugger.ProducersWaitingSemaphore.Remove(Thread.CurrentThread.ManagedThreadId);
			Tools.Debugger.ProducersInsideSemaphore.Add(Thread.CurrentThread.ManagedThreadId);
			try {
				Tools.Debugger.ProducersWaitingLock.Add(Thread.CurrentThread.ManagedThreadId);
				Monitor.Enter(_sharedLock);
				Tools.Debugger.ProducersWaitingLock.Remove(Thread.CurrentThread.ManagedThreadId);
				Tools.Debugger.ProducersInsideLock.Add(Thread.CurrentThread.ManagedThreadId);
				var loopCount = 0;
				while (!condition() || _disposed) {
					Tools.Debugger.AddMessage($"Loop Count - ThreadID: {Thread.CurrentThread.ManagedThreadId},  Role: {ProducerConsumerType.Producer}, Count: {++loopCount} ");
					if (_disposed) {
						throw new InvalidOperationException($"{nameof(ProducerConsumerLock)} disposed whilst thread {Thread.CurrentThread.ManagedThreadId} was waiting in queue");
					}

					Monitor.Pulse(_sharedLock);
					Interlocked.Increment(ref _waitCount);
					Tools.Debugger.ProducersInsideLock.Remove(Thread.CurrentThread.ManagedThreadId);
					Monitor.Wait(_sharedLock);
					Tools.Debugger.ProducersInsideLock.Add(Thread.CurrentThread.ManagedThreadId);
					Interlocked.Decrement(ref _waitCount);
				}
			} catch {
				Monitor.Pulse(_sharedLock);
				if (Monitor.IsEntered(_sharedLock)) {
					Monitor.Exit(_sharedLock);
					Tools.Debugger.ProducersInsideLock.Remove(Thread.CurrentThread.ManagedThreadId);
				}
				Tools.Debugger.ProducersInsideSemaphore.Remove(Thread.CurrentThread.ManagedThreadId);
				_producerSemaphore.Release();
				throw;
			}


			return new ActionScope(
				() => {
					Monitor.Pulse(_sharedLock); // This is needed since other call queues may be connected with _lock, waiting for whatever this operation does 
					Monitor.Exit(_sharedLock);
					Tools.Debugger.ProducersInsideLock.Remove(Thread.CurrentThread.ManagedThreadId);
					_producerSemaphore.Release();
					Tools.Debugger.ProducersInsideSemaphore.Remove(Thread.CurrentThread.ManagedThreadId);
				}
			);
		}

		public IDisposable ConsumerBlockUntil(Func<bool> condition) {
			Guard.ArgumentNotNull(condition, nameof(condition));
			Tools.Debugger.ConsumersWaitingSemaphore.Add(Thread.CurrentThread.ManagedThreadId);
			_consumerSemaphore.Release();
			Tools.Debugger.ConsumersWaitingSemaphore.Remove(Thread.CurrentThread.ManagedThreadId);
			Tools.Debugger.ConsumersInsideSemaphore.Add(Thread.CurrentThread.ManagedThreadId);
			try {
				Tools.Debugger.ConsumersWaitingLock.Add(Thread.CurrentThread.ManagedThreadId);
				Monitor.Enter(_sharedLock);
				Tools.Debugger.ConsumersWaitingLock.Remove(Thread.CurrentThread.ManagedThreadId);
				Tools.Debugger.ConsumersInsideLock.Add(Thread.CurrentThread.ManagedThreadId);
				var loopCount = 0;
				while (!condition() || _disposed) {
					Tools.Debugger.AddMessage($"Loop Count - ThreadID: {Thread.CurrentThread.ManagedThreadId},  Role: {ProducerConsumerType.Consumer}, Count: {++loopCount} ");
					if (_disposed) {
						throw new InvalidOperationException($"{nameof(ProducerConsumerLock)} disposed whilst thread {Thread.CurrentThread.ManagedThreadId} was waiting in queue");
					}

					Monitor.Pulse(_sharedLock);
					Interlocked.Increment(ref _waitCount);
					Tools.Debugger.ConsumersInsideLock.Remove(Thread.CurrentThread.ManagedThreadId);
					Monitor.Wait(_sharedLock);
					Tools.Debugger.ConsumersInsideLock.Add(Thread.CurrentThread.ManagedThreadId);
					Interlocked.Decrement(ref _waitCount);
				}
			} catch {
				Monitor.Pulse(_sharedLock);
				if (Monitor.IsEntered(_sharedLock)) {
					Monitor.Exit(_sharedLock);
					Tools.Debugger.ConsumersInsideLock.Remove(Thread.CurrentThread.ManagedThreadId);
				}
				Tools.Debugger.ConsumersInsideSemaphore.Remove(Thread.CurrentThread.ManagedThreadId);
				_consumerSemaphore.Release();
				throw;
			}


			return new ActionScope(
				() => {
					Monitor.Pulse(_sharedLock); // This is needed since other call queues may be connected with _lock, waiting for whatever this operation does 
					Monitor.Exit(_sharedLock);
					Tools.Debugger.ProducersInsideLock.Remove(Thread.CurrentThread.ManagedThreadId);
					_producerSemaphore.Release();
					Tools.Debugger.ProducersInsideSemaphore.Remove(Thread.CurrentThread.ManagedThreadId);
				}
			);
		}
 
		*/
	}
}
