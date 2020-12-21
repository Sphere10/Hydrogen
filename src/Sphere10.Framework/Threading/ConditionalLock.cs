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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;


namespace Sphere10.Framework {

    /// <summary>
    /// A FIFO queue used to block calls into a region of code based on a condition. Conceptually, this is like
    /// a lock using a condition.
    /// </summary>
    /// <remarks>
    /// lock(obj AND CONDITION) { 
    ///     ... code has exclusive lock on obj and condition is true
    /// }
    /// </remarks>
    
#warning Needs to be reviewed, implementation possibly faulty
    public class ConditionalLock : IDisposable {
        private readonly object _lock;
        private bool _disposed;
        private readonly SemaphoreSlim _semaphore;
        private int _waitCount;

        public ConditionalLock() : this(new object()) {            
        }
        
        public ConditionalLock(object @lock) {
            Volatile.Write(ref _lock, @lock);
            Volatile.Write(ref _disposed, false);
            _semaphore = new SemaphoreSlim(1);
            _waitCount = 0;
        }

        public int Count {
            get {
                lock (_lock)
                    return _semaphore.CurrentCount + Volatile.Read(ref _waitCount);
            }
        }

        public IDisposable BlockUntil(Func<bool> condition) {
            if (condition == null) throw new ArgumentNullException(nameof(condition));
			_semaphore.Wait();
			try {
				Monitor.Enter(_lock);
				while (!condition() || Volatile.Read(ref _disposed)) {
					if (_disposed) {
						throw new SoftwareException("CallQueue disposed whilst blocking");
					}
					//Monitor.Pulse(_lock); // You only need this if the CallQueue was not a queue, but "whoever satisfies first" collection
					Interlocked.Increment(ref _waitCount);
					Monitor.Wait(_lock);
					Interlocked.Decrement(ref _waitCount);
				}
			} catch {
				_semaphore.Release();
				throw;
			}
            return new ActionScope(
                () => {
                    Monitor.Pulse(_lock); // This is needed since other call queues may be connected with _lock, waiting for whatever this operation does 
                    Monitor.Exit(_lock);
                    _semaphore.Release();
                }
            );
        }

        public async Task<IDisposable> Until(Func<bool> condition) {
            return await Task.Run(() => BlockUntil(condition));
        }
        
        public void Pulse() {
            lock (_lock) 
                Monitor.Pulse(_lock);
        }

        public void Dispose() {
            if (Volatile.Read(ref _disposed))
                return;

            lock (_lock) {
                Volatile.Write(ref _disposed, true);
                Monitor.PulseAll(_lock);
            }
        }

    }
}
