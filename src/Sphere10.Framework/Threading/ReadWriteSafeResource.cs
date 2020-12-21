//-----------------------------------------------------------------------
// <copyright file="ReadWriteSafeResource.cs" company="Sphere 10 Software">
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

using System;
using System.Threading;

namespace Sphere10.Framework {

    public abstract class ReadWriteSafeResource : ReadWriteSafeResource<Scope, Scope> {
    }

    public abstract class ReadWriteSafeResource<TReadScope, TWriteScope> : ReadWriteSafeObject<TReadScope, TWriteScope>
        where TReadScope : IScope, new()
        where TWriteScope : IScope, new() {
        private readonly ReadWritable<ScopeTracker> _scopeTracker;

        protected ReadWriteSafeResource() : this(LockRecursionPolicy.SupportsRecursion) {
        }

        protected ReadWriteSafeResource(LockRecursionPolicy policy) : base(policy) {
            _scopeTracker = new ReadWritable<ScopeTracker>(ScopeTracker.Zero);
        }

        protected virtual void OnSetupRead() {
        }

        protected virtual void OnSetupWrite() {
        }

        protected virtual void OnCleanupRead() {
        }

        protected virtual void OnCleanupWrite() {
        }

        protected override void OnReadScopeOpen() {
            base.OnReadScopeOpen();
            var setup = false;
            using (_scopeTracker.EnterWriteScope()) {
                if (_scopeTracker.Value.Writes == 0 && _scopeTracker.Value.Reads == 0)
                    setup = true;
                _scopeTracker.Value.Reads++;
            }
            if (setup) 
                OnSetupRead();
        }

        protected override void OnWriteScopeOpen() {
            base.OnWriteScopeOpen();
            var setup = false;
            using (_scopeTracker.EnterWriteScope()) {
                if (_scopeTracker.Value.Reads > 0) 
                    throw new SoftwareException("Resource already opened in read-only mode"); // this should never be thrown
                if (_scopeTracker.Value.Writes == 0)
                    setup = true;
                _scopeTracker.Value.Writes++;
            }
            if (setup)
                OnSetupWrite();
        }

        protected override void OnReadScopeClosed() {
            base.OnReadScopeClosed();
            var cleanup = false;
            using (_scopeTracker.EnterWriteScope()) {
                // never cleanup a read scope if an outer write is still open
                if (_scopeTracker.Value.Writes == 0 && _scopeTracker.Value.Reads == 1)  
                    cleanup = true;
                _scopeTracker.Value.Reads--;
            }
            if (cleanup)
                OnCleanupRead();
        }

        protected override void OnWriteScopeClosed() {
            base.OnWriteScopeClosed();
            var cleanup = false;
            using (_scopeTracker.EnterWriteScope()) {
                if (_scopeTracker.Value.Writes == 1)
                    cleanup = true;
                _scopeTracker.Value.Writes--;
            }
            if (cleanup)
                OnCleanupWrite();
        }

        protected override void EnsureReadable() {
            using (_scopeTracker.EnterReadScope())
                if (_scopeTracker.Value.Reads <= 0)
                    throw new SoftwareException("Resource has not entered a read scope");
        }

        protected override void EnsureWriteable() {
            using (_scopeTracker.EnterReadScope())
                if (_scopeTracker.Value.Writes <= 0)
                    throw new SoftwareException("Resource has not entered a write scope");
        }


        public class ScopeTracker {
            public int Reads;
            public int Writes;
            public static ScopeTracker Zero => new ScopeTracker { Reads = 0, Writes = 0 };
        }

    }
}
