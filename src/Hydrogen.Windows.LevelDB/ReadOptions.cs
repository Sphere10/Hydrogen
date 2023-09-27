// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

namespace Hydrogen.Windows.LevelDB;

/// <summary>
/// Options that control read operations.
/// </summary>
public class ReadOptions : LevelDBHandle {
	public ReadOptions() {
		Handle = LevelDBInterop.leveldb_readoptions_create();
	}

	/// <summary>
	/// If true, all data read from underlying streams will be
	/// verified against corresponding checksums.
	/// </summary>
	public bool VerifyCheckSums {
		set { LevelDBInterop.leveldb_readoptions_set_verify_checksums(Handle, value ? (byte)1 : (byte)0); }
	}

	/// <summary>
	/// Should the data read for this iteration be cached in memory?
	/// Callers may wish to set this field to false for bulk scans.
	/// Default: true
	/// </summary>
	public bool FillCache {
		set { LevelDBInterop.leveldb_readoptions_set_fill_cache(Handle, value ? (byte)1 : (byte)0); }
	}

	/// <summary>
	/// If "snapshot" is provides, read as of the supplied snapshot
	/// (which must belong to the DB that is being read and which must
	/// not have been released).  
	/// If "snapshot" is not set, use an implicit
	/// snapshot of the state at the beginning of this read operation.
	/// </summary>
	public SnapShot Snapshot {
		set { LevelDBInterop.leveldb_readoptions_set_snapshot(Handle, value.Handle); }
	}

	protected override void FreeUnManagedObjects() {
		LevelDBInterop.leveldb_readoptions_destroy(Handle);
	}
}
