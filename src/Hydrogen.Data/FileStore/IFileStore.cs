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

public interface IFileStore<TFileKeyType> : IDisposable {

	IEnumerable<TFileKeyType> FileKeys { get; }

	TFileKeyType RecommendFileKey(string externalFilePath);

	string GetFilePath(TFileKeyType fileKey);

	bool ContainsFile(TFileKeyType fileKey);

	TFileKeyType NewFile();

	string[] RegisterMany(IEnumerable<TFileKeyType> fileKeys);

	void DeleteMany(IEnumerable<TFileKeyType> fileKeys);

	void Clear();

	public FileInfo GetInfo(TFileKeyType fileKey);

	public Stream Open(TFileKeyType fileKey, FileMode mode);

	public Stream Open(TFileKeyType fileKey, FileMode mode, FileAccess access);

	public Stream Open(TFileKeyType fileKey, FileMode mode, FileAccess access, FileShare share);

	public void SetCreationTime(TFileKeyType fileKey, DateTime creationTime);

	public void SetCreationTimeUtc(TFileKeyType fileKey, DateTime creationTimeUtc);

	public DateTime GetCreationTime(TFileKeyType fileKey);

	public DateTime GetCreationTimeUtc(TFileKeyType fileKey);

	public void SetLastAccessTime(TFileKeyType fileKey, DateTime lastAccessTime);

	public void SetLastAccessTimeUtc(TFileKeyType fileKey, DateTime lastAccessTimeUtc);

	public DateTime GetLastAccessTime(TFileKeyType fileKey);

	public DateTime GetLastAccessTimeUtc(TFileKeyType fileKey);

	public void SetLastWriteTime(TFileKeyType fileKey, DateTime lastWriteTime);

	public void SetLastWriteTimeUtc(TFileKeyType fileKey, DateTime lastWriteTimeUtc);

	public DateTime GetLastWriteTime(TFileKeyType fileKey);

	public DateTime GetLastWriteTimeUtc(TFileKeyType fileKey);

	public FileAttributes GetAttributes(TFileKeyType fileKey);

	public void SetAttributes(TFileKeyType fileKey, FileAttributes fileAttributes);

	public Stream OpenRead(TFileKeyType fileKey);

	public Stream OpenWrite(TFileKeyType fileKey);

	public string ReadAllText(TFileKeyType fileKey);

	public string ReadAllText(TFileKeyType fileKey, Encoding encoding);

	public void WriteAllText(TFileKeyType fileKey, string contents);

	public void WriteAllText(TFileKeyType fileKey, string contents, Encoding encoding);

	public byte[] ReadAllBytes(TFileKeyType fileKey);

	public void WriteAllBytes(TFileKeyType fileKey, byte[] bytes);

	public void AppendAllBytes(TFileKeyType fileKey, byte[] bytes);

	public string[] ReadAllLines(TFileKeyType fileKey);

	public string[] ReadAllLines(TFileKeyType fileKey, Encoding encoding);

	public void WriteAllLines(TFileKeyType fileKey, string[] contents);

	public void WriteAllLines(TFileKeyType fileKey, string[] contents, Encoding encoding);

	public void AppendAllText(TFileKeyType fileKey, string contents);

	public void AppendAllText(TFileKeyType fileKey, string contents, Encoding encoding);

}
