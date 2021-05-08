//-----------------------------------------------------------------------
// <copyright file="ThreadSafeObject.cs" company="Sphere 10 Software">
//
// Copyright (c) Sphere 10 Software. All rights reserved. (http://www.sphere10.com)
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// <author>Herman Schoenfeld</author>
// <date>2019</date>
// </copyright>
//-----------------------------------------------------------------------

using System.Threading;

namespace Sphere10.Framework {

	public class SynchronizedObject : SynchronizedObject<Scope, Scope>, ISynchronizedObject {
        public SynchronizedObject() : this(LockRecursionPolicy.SupportsRecursion) {
        }

        protected SynchronizedObject(LockRecursionPolicy policy) 
            : base(policy) {
        }
    }

    public class SynchronizedObject<TReadScope, TWriteScope> : ISynchronizedObject<TReadScope, TWriteScope>
        where TReadScope : IScope, new()
        where TWriteScope : IScope, new() {

        protected SynchronizedObject() : this(LockRecursionPolicy.SupportsRecursion) {
        }

        protected SynchronizedObject(LockRecursionPolicy policy) {
            ThreadLock = new ReaderWriterLockSlim(policy);
        }

        public ReaderWriterLockSlim ThreadLock { get; }

        public TReadScope EnterReadScope() {
            ThreadLock.EnterReadLock();
            OnReadScopeOpen();
            var scope = new TReadScope();
            scope.ScopeEnd += (sender, args) => {
                ThreadLock.ExitReadLock();
                OnReadScopeClosed();
            };
            return scope;
        }

        public TWriteScope EnterWriteScope() {
            ThreadLock.EnterWriteLock();
            OnWriteScopeOpen();
            var scope = new TWriteScope();
            scope.ScopeEnd += (sender, args) => {
                ThreadLock.ExitWriteLock();
                OnWriteScopeClosed();
            };
            return scope;
        }

		protected virtual void OnReadScopeOpen() {
        }

        protected virtual void OnReadScopeClosed() {
        }

        protected virtual void OnWriteScopeOpen() {
        }

        protected virtual void OnWriteScopeClosed() {
        }

		protected virtual void EnsureReadable() {
			if (!(ThreadLock.IsReadLockHeld || ThreadLock.IsUpgradeableReadLockHeld))
				throw new SoftwareException("Resource has not entered a read scope");
		}

		protected virtual void EnsureWritable() {
			if (!ThreadLock.IsWriteLockHeld)
				throw new SoftwareException("Resource has not entered a write scope");
		}

	}
}