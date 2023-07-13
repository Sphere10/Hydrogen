// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System.IO;
using System.IO.Compression;
using System.Linq;


namespace Hydrogen.DApp.Core.Storage;

public class ZipPackage : KeyValueStoreBase<string> {
	protected FileStream _stream;
	protected ZipArchive _archive;

	public ZipPackage(string filename) {
		FilePath = filename;
	}

	public virtual string Name {
		get => Path.GetFileName(FilePath);
		set => Tools.FileSystem.RenameFile(FilePath, value);
	}

	public string FilePath { get; set; }

	public void ExtractTo(string directory, bool overwrite = false) {
		EnsureReadable();
		if (overwrite && Directory.Exists(directory))
			Tools.FileSystem.DeleteAllFiles(directory, true);
		_archive.ExtractToDirectory(directory);
	}

	protected override Stream OpenReadInternal(string key) {
		var entry = _archive.GetEntry(key);
		return entry.Open();
	}

	protected override IQueryable<string> GetKeysInternal() {
		return _archive.Entries.Select(x => x.FullName).AsQueryable();
	}

	protected override Stream OpenWriteInternal(string key) {
		ZipArchiveEntry entry;
		switch (_archive.Mode) {
			case ZipArchiveMode.Create:
				entry = _archive.CreateEntry(key);
				break;
			case ZipArchiveMode.Update:
				entry = _archive.GetEntry(key);
				if (entry == null)
					entry = _archive.CreateEntry(key);
				break;
			default:
				entry = _archive.GetEntry(key);
				break;
		}
		return entry.Open();
	}

	protected override void InitializeReadScope() {
		if (!File.Exists(FilePath))
			throw new FileNotFoundException("Package not found", FilePath);
		_stream = File.OpenRead(FilePath);
		_archive = new ZipArchive(_stream, ZipArchiveMode.Read);
	}

	protected override void InitializeWriteScope() {
		var exists = File.Exists(FilePath);
		_stream = File.Open(FilePath, FileMode.OpenOrCreate);
		_archive = new ZipArchive(_stream, exists ? ZipArchiveMode.Update : ZipArchiveMode.Create);
	}

	protected override void FinalizeReadScope() {
		_archive.Dispose();
		_stream.Dispose();
	}

	protected override void FinalizeWriteScope() {
		_stream.Flush(true);
		_archive.Dispose();
		_stream.Dispose();
	}

	public static ZipPackage Create(string filename) {
		var package = new ZipPackage(filename);
		using (package.EnterWriteScope()) {
			// file should be created
		}
		return package;
	}

}
