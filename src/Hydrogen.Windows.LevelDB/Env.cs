// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

namespace Hydrogen.Windows.LevelDB;

/// <summary>
/// A default environment to access operating system functionality like 
/// the filesystem etc of the current operating system.
/// </summary>
public class Env : LevelDBHandle {
	public Env() {
		Handle = LevelDBInterop.leveldb_create_default_env();
	}

	protected override void FreeUnManagedObjects() {
		LevelDBInterop.leveldb_env_destroy(Handle);
	}
}
