using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;

namespace Hydrogen {
	/// <summary>
	/// A semaphore that uses <see cref="Monitor"/> locking under the hood. 
	/// </summary>
	/// <remarks>This was created as a replacement for <see cref="SemaphoreSlim"/> which was believed to be a cause of a bug. Turns out Semaphore's can become very slow if logical threads >> physical cores and they wait on a semaphore.
	/// <see cref="ProducerConsumerLock"/> comments for discussion).</remarks>
	internal class SemaphoreMin : Disposable {

		private readonly object _lock;
		private long _currentCount;

		public SemaphoreMin(int initialCount) {
			Guard.Argument(initialCount == 1, nameof(initialCount), "Only value of 1 is supported in current implementation");
			_lock = new object();
			_currentCount = 0;
		}

		public int CurrentCount => (int)Interlocked.Read(ref _currentCount);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Wait() {
			Interlocked.Increment(ref _currentCount);
			Monitor.Enter(_lock);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Release() {
			Interlocked.Decrement(ref _currentCount);
			Monitor.Exit(_lock);
		}

		protected override void FreeManagedResources() {
			if (_currentCount != 0)
				throw new InvalidOperationException("SemaphoreMin has an active waiter");
			if (Monitor.IsEntered(_lock))
				Monitor.Exit(_lock);
		}
	}
}
