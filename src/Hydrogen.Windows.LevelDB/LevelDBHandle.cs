// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;

namespace Hydrogen.Windows.LevelDB;

/// <summary>
/// Base class for all LevelDB objects
/// Implement IDisposable as prescribed by http://msdn.microsoft.com/en-us/library/b1yfkh5e.aspx by overriding the two additional virtual methods
/// </summary>
public abstract class LevelDBHandle : IDisposable {
	bool Disposed;

	public IntPtr Handle { protected set; get; }

	public void Dispose() {
		Dispose(true);
		GC.SuppressFinalize(this);
	}

	protected virtual void FreeManagedObjects() {
	}

	protected virtual void FreeUnManagedObjects() {
	}

	void Dispose(bool disposing) {
		if (!Disposed) {
			if (disposing) {
				FreeManagedObjects();
			}
			if (Handle != IntPtr.Zero) {
				FreeUnManagedObjects();
				Handle = IntPtr.Zero;
			}
			Disposed = true;
		}
	}

	~LevelDBHandle() {
		Dispose(false);
	}
}
