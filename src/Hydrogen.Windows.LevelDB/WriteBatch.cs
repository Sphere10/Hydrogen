// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Text;

namespace Hydrogen.Windows.LevelDB;

/// <summary>
/// WriteBatch holds a collection of updates to apply atomically to a DB.
///
/// The updates are applied in the order in which they are added
/// to the WriteBatch.  For example, the value of "key" will be "v3"
/// after the following batch is written:
///
///    batch.Put("key", "v1");
///    batch.Delete("key");
///    batch.Put("key", "v2");
///    batch.Put("key", "v3");
/// </summary>
public class WriteBatch : LevelDBHandle {
	public WriteBatch() {
		Handle = LevelDBInterop.leveldb_writebatch_create();
	}

	/// <summary>
	/// Clear all updates buffered in this batch.
	/// </summary>
	public void Clear() {
		LevelDBInterop.leveldb_writebatch_clear(Handle);
	}

	/// <summary>
	/// Store the mapping "key->value" in the database.
	/// </summary>
	public WriteBatch Put(string key, string value) {
		return Put(Encoding.UTF8.GetBytes(key), Encoding.UTF8.GetBytes(value));
	}

	/// <summary>
	/// Store the mapping "key->value" in the database.
	/// </summary>
	public WriteBatch Put(byte[] key, byte[] value) {
		LevelDBInterop.leveldb_writebatch_put(Handle, key, (IntPtr)key.Length, value, (IntPtr)value.Length);
		return this;
	}

	/// <summary>
	/// If the database contains a mapping for "key", erase it.  
	/// Else do nothing.
	/// </summary>
	public WriteBatch Delete(string key) {
		return Delete(Encoding.UTF8.GetBytes(key));
	}

	/// <summary>
	/// If the database contains a mapping for "key", erase it.  
	/// Else do nothing.
	/// </summary>
	public WriteBatch Delete(byte[] key) {
		LevelDBInterop.leveldb_writebatch_delete(Handle, key, (IntPtr)key.Length);
		return this;
	}

	/// <summary>
	/// Support for iterating over a batch.
	/// </summary>
	public void Iterate(IntPtr state, Action<IntPtr, IntPtr, IntPtr, IntPtr, IntPtr> put, Action<IntPtr, IntPtr, IntPtr> deleted) {
		LevelDBInterop.leveldb_writebatch_iterate(Handle, state, put, deleted);
	}

	protected override void FreeUnManagedObjects() {
		LevelDBInterop.leveldb_writebatch_destroy(Handle);
	}
}
