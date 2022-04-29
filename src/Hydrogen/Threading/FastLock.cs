using System;
using System.Threading;

namespace Hydrogen {
	public sealed class FastLock {
		private readonly ReaderWriterLockSlim _lock;

        public FastLock() {
			_lock = new ReaderWriterLockSlim(LockRecursionPolicy.SupportsRecursion);
        }

		public IDisposable EnterLockScope() {
			_lock.EnterWriteLock();
			return new ActionScope(() => _lock.ExitWriteLock());
        }

	}
}