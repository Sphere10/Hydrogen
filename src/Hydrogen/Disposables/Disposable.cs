// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT NON-AI software license, see the accompanying file
// LICENSE or visit https://sphere10.com/legal/NON-AI-MIT.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Threading.Tasks;

namespace Hydrogen;

// This class shows how to use a disposable resource.
// The resource is first initialized and passed to
// the constructor, but it could also be
// initialized in the constructor.
// The lifetime of the resource does not 
// exceed the lifetime of this instance.
// This type does not need a finalizer because it does not
// directly create a native resource like a file handle
// or memory in the unmanaged heap.


public abstract class Disposable : IAsyncDisposable, IDisposable {
	private bool _disposed;

	public void Dispose() {
		Dispose(true);
		GC.SuppressFinalize(this); // Use SupressFinalize in case a subclass of this type implements a finalizer.
	}

	public async ValueTask DisposeAsync() {
		await FreeManagedResourcesAsync().ConfigureAwait(false);

		Dispose(disposing: false);

#pragma warning disable CA1816 // Dispose methods should call SuppressFinalize
		GC.SuppressFinalize(this);
#pragma warning restore CA1816 // Dispose methods should call SuppressFinalize
	}

	protected virtual void Dispose(bool disposing) {
		if (_disposed) return;

		// If you need thread safety, use a lock around these 
		// operations, as well as in your methods that use the resource.

		if (disposing)
			FreeManagedResources();

		FreeUnmanagedResources();

		_disposed = true;
	}

	protected abstract ValueTask FreeManagedResourcesAsync();

	protected abstract void FreeManagedResources();

	protected virtual void FreeUnmanagedResources() {
	}

}
