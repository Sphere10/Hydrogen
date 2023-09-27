// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;


namespace Hydrogen.Data;

public abstract class FileStoreBase<TFileKey> : SyncDisposable, IFileStore<TFileKey> {

	#region Properties

	public abstract IEnumerable<TFileKey> FileKeys { get; }

	#endregion

	#region Methods

	public abstract TFileKey RecommendFileKey(string externalFilePath);

	public abstract string GetFilePath(TFileKey fileKey);

	public abstract bool ContainsFile(TFileKey fileKey);

	public abstract TFileKey NewFile();

	public abstract string[] RegisterMany(IEnumerable<TFileKey> fileKeys);

	public abstract void DeleteMany(IEnumerable<TFileKey> fileKeys);

	public abstract void Clear();

	#endregion

	#region File Access Methods

	public FileInfo GetInfo(TFileKey fileKey) {
		return new FileInfo(GetFilePath(fileKey));
	}

	public Stream Open(TFileKey fileKey, FileMode mode) {
		return File.Open(GetFilePath(fileKey), mode);
	}

	public Stream Open(TFileKey fileKey, FileMode mode, FileAccess access) {
		return File.Open(GetFilePath(fileKey), mode, access);
	}

	public Stream Open(TFileKey fileKey, FileMode mode, FileAccess access, FileShare share) {
		return File.Open(GetFilePath(fileKey), mode, access, share);
	}

	public void SetCreationTime(TFileKey fileKey, DateTime creationTime) {
		File.SetCreationTime(GetFilePath(fileKey), creationTime);
	}

	public void SetCreationTimeUtc(TFileKey fileKey, DateTime creationTimeUtc) {
		File.SetCreationTimeUtc(GetFilePath(fileKey), creationTimeUtc);
	}

	public DateTime GetCreationTime(TFileKey fileKey) {
		return File.GetCreationTime(GetFilePath(fileKey));
	}

	public DateTime GetCreationTimeUtc(TFileKey fileKey) {
		return File.GetCreationTimeUtc(GetFilePath(fileKey));
	}

	public void SetLastAccessTime(TFileKey fileKey, DateTime lastAccessTime) {
		File.SetLastAccessTime(GetFilePath(fileKey), lastAccessTime);
	}

	public void SetLastAccessTimeUtc(TFileKey fileKey, DateTime lastAccessTimeUtc) {
		File.SetLastAccessTimeUtc(GetFilePath(fileKey), lastAccessTimeUtc);
	}

	public DateTime GetLastAccessTime(TFileKey fileKey) {
		return File.GetLastAccessTime(GetFilePath(fileKey));
	}

	public DateTime GetLastAccessTimeUtc(TFileKey fileKey) {
		return File.GetLastAccessTimeUtc(GetFilePath(fileKey));
	}

	public void SetLastWriteTime(TFileKey fileKey, DateTime lastWriteTime) {
		File.SetLastAccessTime(GetFilePath(fileKey), lastWriteTime);
	}

	public void SetLastWriteTimeUtc(TFileKey fileKey, DateTime lastWriteTimeUtc) {
		File.SetLastWriteTimeUtc(GetFilePath(fileKey), lastWriteTimeUtc);
	}

	public DateTime GetLastWriteTime(TFileKey fileKey) {
		return File.GetLastAccessTime(GetFilePath(fileKey));
	}

	public DateTime GetLastWriteTimeUtc(TFileKey fileKey) {
		return File.GetLastWriteTimeUtc(GetFilePath(fileKey));
	}

	public FileAttributes GetAttributes(TFileKey fileKey) {
		return File.GetAttributes(GetFilePath(fileKey));
	}

	public void SetAttributes(TFileKey fileKey, FileAttributes fileAttributes) {
		File.SetAttributes(GetFilePath(fileKey), fileAttributes);
	}

	public Stream OpenRead(TFileKey fileKey) {
		return File.OpenRead(GetFilePath(fileKey));
	}

	public Stream OpenWrite(TFileKey fileKey) {
		return File.OpenWrite(GetFilePath(fileKey));
	}

	public string ReadAllText(TFileKey fileKey) {
		return File.ReadAllText(GetFilePath(fileKey));
	}

	public string ReadAllText(TFileKey fileKey, Encoding encoding) {
		return File.ReadAllText(GetFilePath(fileKey), encoding);
	}

	public void WriteAllText(TFileKey fileKey, string contents) {
		File.WriteAllText(GetFilePath(fileKey), contents);
	}

	public void WriteAllText(TFileKey fileKey, string contents, Encoding encoding) {
		File.WriteAllText(GetFilePath(fileKey), contents, encoding);
	}

	public byte[] ReadAllBytes(TFileKey fileKey) {
		return File.ReadAllBytes(GetFilePath(fileKey));
	}

	public void WriteAllBytes(TFileKey fileKey, byte[] bytes) {
		Tools.FileSystem.WriteAllBytes(GetFilePath(fileKey), bytes);
	}

	public void AppendAllBytes(TFileKey fileKey, byte[] bytes) {
		Tools.FileSystem.AppendAllBytes(GetFilePath(fileKey), bytes);
	}

	public string[] ReadAllLines(TFileKey fileKey) {
		return File.ReadAllLines(GetFilePath(fileKey));
	}

	public string[] ReadAllLines(TFileKey fileKey, Encoding encoding) {
		return File.ReadAllLines(GetFilePath(fileKey), encoding);
	}

	public void WriteAllLines(TFileKey fileKey, string[] contents) {
		File.WriteAllLines(GetFilePath(fileKey), contents);
	}

	public void WriteAllLines(TFileKey fileKey, string[] contents, Encoding encoding) {
		File.WriteAllLines(GetFilePath(fileKey), contents, encoding);
	}

	public void AppendAllText(TFileKey fileKey, string contents) {
		File.AppendAllText(GetFilePath(fileKey), contents);
	}

	public void AppendAllText(TFileKey fileKey, string contents, Encoding encoding) {
		File.AppendAllText(GetFilePath(fileKey), contents, encoding);
	}

	#endregion

}
