//-----------------------------------------------------------------------
// <copyright file="Disposable.cs" company="Sphere 10 Software">
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

namespace Hydrogen {

	// This class shows how to use a disposable resource.
	// The resource is first initialized and passed to
	// the constructor, but it could also be
	// initialized in the constructor.
	// The lifetime of the resource does not 
	// exceed the lifetime of this instance.
	// This type does not need a finalizer because it does not
	// directly create a native resource like a file handle
	// or memory in the unmanaged heap.

	public abstract class Disposable : IDisposable {
        private bool _disposed;

        public void Dispose() {
            Dispose(true);

            // Use SupressFinalize in case a subclass
            // of this type implements a finalizer.
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing) {
            if (_disposed) return;

            // If you need thread safety, use a lock around these 
            // operations, as well as in your methods that use the resource.

            if (disposing) {
                FreeManagedResources();
            }

            FreeUnmanagedResources();

            _disposed = true;
        }

        protected abstract void FreeManagedResources();
        protected virtual void FreeUnmanagedResources() { }
    }
}
