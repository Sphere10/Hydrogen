// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

namespace Hydrogen.Windows.LevelDB;

/// <summary>
/// Options that control write operations.
/// </summary>
public class WriteOptions : LevelDBHandle {
	public WriteOptions() {
		Handle = LevelDBInterop.leveldb_writeoptions_create();
	}

	/// <summary>
	/// If true, the write will be flushed from the operating system
	/// buffer cache (by calling WritableFile::Sync()) before the write
	/// is considered complete.  If this flag is true, writes will be
	/// slower.
	///
	/// If this flag is false, and the machine crashes, some recent
	/// writes may be lost.  Note that if it is just the process that
	/// crashes (i.e., the machine does not reboot), no writes will be
	/// lost even if sync==false.
	///
	/// In other words, a DB write with sync==false has similar
	/// crash semantics as the "write()" system call.  A DB write
	/// with sync==true has similar crash semantics to a "write()"
	/// system call followed by "fsync()".
	/// </summary>
	public bool Sync {
		set { LevelDBInterop.leveldb_writeoptions_set_sync(Handle, value ? (byte)1 : (byte)0); }
	}

	protected override void FreeUnManagedObjects() {
		LevelDBInterop.leveldb_writeoptions_destroy(Handle);
	}
}
