// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;

namespace Hydrogen.Windows.LevelDB;

public static class LevelDBInterop {
	static LevelDBInterop() {
		var assembly = Assembly.GetAssembly(typeof(LevelDBInterop)).GetName();
		var directory = Path.Combine(
			Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData, Environment.SpecialFolderOption.Create),
			$"Hydrogen Framework{Path.DirectorySeparatorChar}{assembly.Name}-{assembly.Version}{Path.DirectorySeparatorChar}{(IntPtr.Size == 4 ? "x32" : "x64")}"
		);
		if (!Directory.Exists(directory))
			Directory.CreateDirectory(directory);
		var nativeLib = Path.Combine(directory, "LevelDB.dll");
		if (!File.Exists(nativeLib)) {
			File.WriteAllBytes(nativeLib, IntPtr.Size == 4 ? Resources.LevelDB32_DLL : Resources.LevelDB64_DLL);
		}
		LoadLibrary(nativeLib);
	}


	static bool BuffersEqual(byte[] left, byte[] right) {
		if (left.Length != right.Length)
			return false;
		for (int i = 0; i < left.Length; ++i)
			if (left[i] != right[i])
				return false;
		return true;
	}

	[DllImport("kernel32", SetLastError = true, CharSet = CharSet.Unicode)]
	static extern IntPtr LoadLibrary(string lpFileName);

	#region DB

	[DllImport("LevelDB.dll", CallingConvention = CallingConvention.Cdecl)]
	public static extern IntPtr leveldb_open(IntPtr /* Options*/ options, string name, out IntPtr error);

	[DllImport("LevelDB.dll", CallingConvention = CallingConvention.Cdecl)]
	public static extern void leveldb_close(IntPtr /*DB */ db);

	[DllImport("LevelDB.dll", CallingConvention = CallingConvention.Cdecl)]
	public static extern void leveldb_put(IntPtr /* DB */ db, IntPtr /* WriteOptions*/ options, byte[] key, IntPtr keylen, byte[] val, IntPtr vallen, out IntPtr errptr);

	[DllImport("LevelDB.dll", CallingConvention = CallingConvention.Cdecl)]
	public static extern void leveldb_delete(IntPtr /* DB */ db, IntPtr /* WriteOptions*/ options, byte[] key, IntPtr keylen, out IntPtr errptr);

	[DllImport("LevelDB.dll", CallingConvention = CallingConvention.Cdecl)]
	public static extern void leveldb_write(IntPtr /* DB */ db, IntPtr /* WriteOptions*/ options, IntPtr /* WriteBatch */ batch, out IntPtr errptr);

	[DllImport("LevelDB.dll", CallingConvention = CallingConvention.Cdecl)]
	public static extern IntPtr leveldb_get(IntPtr /* DB */ db, IntPtr /* ReadOptions*/ options, byte[] key, IntPtr keylen, out IntPtr vallen, out IntPtr errptr);

	[DllImport("LevelDB.dll", CallingConvention = CallingConvention.Cdecl)]
	public static extern IntPtr leveldb_get(IntPtr /* DB */ db, IntPtr /* ReadOptions*/ options, IntPtr key, IntPtr keylen, out IntPtr vallen, out IntPtr errptr);

	//[DllImport("LevelDB.dll", CallingConvention = CallingConvention.Cdecl)]
	//static extern void leveldb_approximate_sizes(IntPtr /* DB */ db, int num_ranges, byte[] range_start_key, long range_start_key_len, byte[] range_limit_key, long range_limit_key_len, out long sizes);

	[DllImport("LevelDB.dll", CallingConvention = CallingConvention.Cdecl)]
	public static extern IntPtr leveldb_create_iterator(IntPtr /* DB */ db, IntPtr /* ReadOption */ options);

	[DllImport("LevelDB.dll", CallingConvention = CallingConvention.Cdecl)]
	public static extern IntPtr leveldb_create_snapshot(IntPtr /* DB */ db);

	[DllImport("LevelDB.dll", CallingConvention = CallingConvention.Cdecl)]
	public static extern void leveldb_release_snapshot(IntPtr /* DB */ db, IntPtr /* SnapShot*/ snapshot);

	[DllImport("LevelDB.dll", CallingConvention = CallingConvention.Cdecl)]
	public static extern IntPtr leveldb_property_value(IntPtr /* DB */ db, string propname);

	[DllImport("LevelDB.dll", CallingConvention = CallingConvention.Cdecl)]
	public static extern void leveldb_repair_db(IntPtr /* Options*/ options, string name, out IntPtr error);

	[DllImport("LevelDB.dll", CallingConvention = CallingConvention.Cdecl)]
	public static extern void leveldb_destroy_db(IntPtr /* Options*/ options, string name, out IntPtr error);

	#region extensions

	[DllImport("LevelDB.dll", CallingConvention = CallingConvention.Cdecl)]
	public static extern void leveldb_free(IntPtr /* void */ ptr);

	#endregion

	#endregion

	#region Env

	[DllImport("LevelDB.dll", CallingConvention = CallingConvention.Cdecl)]
	public static extern IntPtr leveldb_create_default_env();

	[DllImport("LevelDB.dll", CallingConvention = CallingConvention.Cdecl)]
	public static extern void leveldb_env_destroy(IntPtr /*Env*/ cache);

	#endregion

	#region Iterator

	[DllImport("LevelDB.dll", CallingConvention = CallingConvention.Cdecl)]
	public static extern void leveldb_iter_destroy(IntPtr /*Iterator*/ iterator);

	[DllImport("LevelDB.dll", CallingConvention = CallingConvention.Cdecl)]
	public static extern byte leveldb_iter_valid(IntPtr /*Iterator*/ iterator);

	[DllImport("LevelDB.dll", CallingConvention = CallingConvention.Cdecl)]
	public static extern void leveldb_iter_seek_to_first(IntPtr /*Iterator*/ iterator);

	[DllImport("LevelDB.dll", CallingConvention = CallingConvention.Cdecl)]
	public static extern void leveldb_iter_seek_to_last(IntPtr /*Iterator*/ iterator);

	[DllImport("LevelDB.dll", CallingConvention = CallingConvention.Cdecl)]
	public static extern void leveldb_iter_seek(IntPtr /*Iterator*/ iterator, byte[] key, IntPtr length);

	[DllImport("LevelDB.dll", CallingConvention = CallingConvention.Cdecl)]
	public static extern void leveldb_iter_next(IntPtr /*Iterator*/ iterator);

	[DllImport("LevelDB.dll", CallingConvention = CallingConvention.Cdecl)]
	public static extern void leveldb_iter_prev(IntPtr /*Iterator*/ iterator);

	[DllImport("LevelDB.dll", CallingConvention = CallingConvention.Cdecl)]
	public static extern IntPtr leveldb_iter_key(IntPtr /*Iterator*/ iterator, out IntPtr length);

	[DllImport("LevelDB.dll", CallingConvention = CallingConvention.Cdecl)]
	public static extern IntPtr leveldb_iter_value(IntPtr /*Iterator*/ iterator, out IntPtr length);

	[DllImport("LevelDB.dll", CallingConvention = CallingConvention.Cdecl)]
	public static extern void leveldb_iter_get_error(IntPtr /*Iterator*/ iterator, out IntPtr error);

	#endregion

	#region Options

	[DllImport("LevelDB.dll", CallingConvention = CallingConvention.Cdecl)]
	public static extern IntPtr leveldb_options_create();

	[DllImport("LevelDB.dll", CallingConvention = CallingConvention.Cdecl)]
	public static extern void leveldb_options_destroy(IntPtr /*Options*/ options);

	[DllImport("LevelDB.dll", CallingConvention = CallingConvention.Cdecl)]
	public static extern void leveldb_options_set_create_if_missing(IntPtr /*Options*/ options, byte o);

	[DllImport("LevelDB.dll", CallingConvention = CallingConvention.Cdecl)]
	public static extern void leveldb_options_set_error_if_exists(IntPtr /*Options*/ options, byte o);

	[DllImport("LevelDB.dll", CallingConvention = CallingConvention.Cdecl)]
	public static extern void leveldb_options_set_info_log(IntPtr /*Options*/ options, IntPtr /* Logger */ logger);

	[DllImport("LevelDB.dll", CallingConvention = CallingConvention.Cdecl)]
	public static extern void leveldb_options_set_paranoid_checks(IntPtr /*Options*/ options, byte o);

	[DllImport("LevelDB.dll", CallingConvention = CallingConvention.Cdecl)]
	public static extern void leveldb_options_set_env(IntPtr /*Options*/ options, IntPtr /*Env*/ env);

	[DllImport("LevelDB.dll", CallingConvention = CallingConvention.Cdecl)]
	public static extern void leveldb_options_set_write_buffer_size(IntPtr /*Options*/ options, long size);

	[DllImport("LevelDB.dll", CallingConvention = CallingConvention.Cdecl)]
	public static extern void leveldb_options_set_max_open_files(IntPtr /*Options*/ options, int max);

	[DllImport("LevelDB.dll", CallingConvention = CallingConvention.Cdecl)]
	public static extern void leveldb_options_set_cache(IntPtr /*Options*/ options, IntPtr /*Cache*/ cache);

	[DllImport("LevelDB.dll", CallingConvention = CallingConvention.Cdecl)]
	public static extern void leveldb_options_set_block_size(IntPtr /*Options*/ options, long size);

	[DllImport("LevelDB.dll", CallingConvention = CallingConvention.Cdecl)]
	public static extern void leveldb_options_set_block_restart_interval(IntPtr /*Options*/ options, int interval);

	[DllImport("LevelDB.dll", CallingConvention = CallingConvention.Cdecl)]
	public static extern void leveldb_options_set_compression(IntPtr /*Options*/ options, int level);

	[DllImport("LevelDB.dll", CallingConvention = CallingConvention.Cdecl)]
	public static extern void leveldb_options_set_comparator(IntPtr /*Options*/ options, IntPtr /*Comparator*/ comparer);

	#endregion

	#region ReadOptions

	[DllImport("LevelDB.dll", CallingConvention = CallingConvention.Cdecl)]
	public static extern IntPtr leveldb_readoptions_create();

	[DllImport("LevelDB.dll", CallingConvention = CallingConvention.Cdecl)]
	public static extern void leveldb_readoptions_destroy(IntPtr /*ReadOptions*/ options);

	[DllImport("LevelDB.dll", CallingConvention = CallingConvention.Cdecl)]
	public static extern void leveldb_readoptions_set_verify_checksums(IntPtr /*ReadOptions*/ options, byte o);

	[DllImport("LevelDB.dll", CallingConvention = CallingConvention.Cdecl)]
	public static extern void leveldb_readoptions_set_fill_cache(IntPtr /*ReadOptions*/ options, byte o);

	[DllImport("LevelDB.dll", CallingConvention = CallingConvention.Cdecl)]
	public static extern void leveldb_readoptions_set_snapshot(IntPtr /*ReadOptions*/ options, IntPtr /*SnapShot*/ snapshot);

	#endregion

	#region WriteBatch

	[DllImport("LevelDB.dll", CallingConvention = CallingConvention.Cdecl)]
	public static extern IntPtr leveldb_writebatch_create();

	[DllImport("LevelDB.dll", CallingConvention = CallingConvention.Cdecl)]
	public static extern void leveldb_writebatch_destroy(IntPtr /* WriteBatch */ batch);

	[DllImport("LevelDB.dll", CallingConvention = CallingConvention.Cdecl)]
	public static extern void leveldb_writebatch_clear(IntPtr /* WriteBatch */ batch);

	[DllImport("LevelDB.dll", CallingConvention = CallingConvention.Cdecl)]
	public static extern void leveldb_writebatch_put(IntPtr /* WriteBatch */ batch, byte[] key, IntPtr keylen, byte[] val, IntPtr vallen);

	[DllImport("LevelDB.dll", CallingConvention = CallingConvention.Cdecl)]
	public static extern void leveldb_writebatch_delete(IntPtr /* WriteBatch */ batch, byte[] key, IntPtr keylen);

	[DllImport("LevelDB.dll", CallingConvention = CallingConvention.Cdecl)]
	public static extern void leveldb_writebatch_iterate(IntPtr /* WriteBatch */ batch, IntPtr state, Action<IntPtr, IntPtr, IntPtr, IntPtr, IntPtr> put, Action<IntPtr, IntPtr, IntPtr> deleted);

	#endregion

	#region WriteOptions

	[DllImport("LevelDB.dll", CallingConvention = CallingConvention.Cdecl)]
	public static extern IntPtr leveldb_writeoptions_create();

	[DllImport("LevelDB.dll", CallingConvention = CallingConvention.Cdecl)]
	public static extern void leveldb_writeoptions_destroy(IntPtr /*WriteOptions*/ options);

	[DllImport("LevelDB.dll", CallingConvention = CallingConvention.Cdecl)]
	public static extern void leveldb_writeoptions_set_sync(IntPtr /*WriteOptions*/ options, byte o);

	#endregion

	#region Cache

	[DllImport("LevelDB.dll", CallingConvention = CallingConvention.Cdecl)]
	public static extern IntPtr leveldb_cache_create_lru(IntPtr capacity);

	[DllImport("LevelDB.dll", CallingConvention = CallingConvention.Cdecl)]
	public static extern void leveldb_cache_destroy(IntPtr /*Cache*/ cache);

	#endregion

	#region Comparator

	[DllImport("LevelDB.dll", CallingConvention = CallingConvention.Cdecl)]
	public static extern IntPtr /* leveldb_comparator_t* */
		leveldb_comparator_create(
			IntPtr /* void* */ state,
			IntPtr /* void (*)(void*) */ destructor,
			IntPtr
				/* int (*compare)(void*,
				                  const char* a, size_t alen,
				                  const char* b, size_t blen) */
				compare,
			IntPtr /* const char* (*)(void*) */ name);

	[DllImport("LevelDB.dll", CallingConvention = CallingConvention.Cdecl)]
	public static extern void leveldb_comparator_destroy(IntPtr /* leveldb_comparator_t* */ cmp);

	#endregion

}
