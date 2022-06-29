//-----------------------------------------------------------------------
// <copyright file="FileStore.cs" company="Sphere 10 Software">
//
// Copyright (c) Sphere 10 Software. All rights reserved. (http://www.sphere10.com)
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// <author>Herman Schoenfeld</author>
// <date>2018</date>
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;


namespace Hydrogen.Data;

public interface IFileStoreBase<TfileKeyType> : IDisposable {

	IEnumerable<TfileKeyType> FileKeys { get; }

	string GetFilePath(TfileKeyType fileKey);

	bool ContainsFile(TfileKeyType fileKey);

	TfileKeyType NewFile();

	void RegisterMany(IEnumerable<TfileKeyType> fileKeys);

	void DeleteMany(IEnumerable<TfileKeyType> fileKeys);

	void Clear();

	public FileInfo GetInfo(TfileKeyType fileKey);

	public Stream Open(TfileKeyType fileKey, FileMode mode);

	public Stream Open(TfileKeyType fileKey, FileMode mode, FileAccess access);

	public Stream Open(TfileKeyType fileKey, FileMode mode, FileAccess access, FileShare share);

	public void SetCreationTime(TfileKeyType fileKey, DateTime creationTime);

	public void SetCreationTimeUtc(TfileKeyType fileKey, DateTime creationTimeUtc);

	public DateTime GetCreationTime(TfileKeyType fileKey);

	public DateTime GetCreationTimeUtc(TfileKeyType fileKey);

	public void SetLastAccessTime(TfileKeyType fileKey, DateTime lastAccessTime);

	public void SetLastAccessTimeUtc(TfileKeyType fileKey, DateTime lastAccessTimeUtc);

	public DateTime GetLastAccessTime(TfileKeyType fileKey);

	public DateTime GetLastAccessTimeUtc(TfileKeyType fileKey);

	public void SetLastWriteTime(TfileKeyType fileKey, DateTime lastWriteTime);

	public void SetLastWriteTimeUtc(TfileKeyType fileKey, DateTime lastWriteTimeUtc);

	public DateTime GetLastWriteTime(TfileKeyType fileKey);

	public DateTime GetLastWriteTimeUtc(TfileKeyType fileKey);

	public FileAttributes GetAttributes(TfileKeyType fileKey);

	public void SetAttributes(TfileKeyType fileKey, FileAttributes fileAttributes);

	public Stream OpenRead(TfileKeyType fileKey);

	public Stream OpenWrite(TfileKeyType fileKey);

	public string ReadAllText(TfileKeyType fileKey);

	public string ReadAllText(TfileKeyType fileKey, Encoding encoding);

	public void WriteAllText(TfileKeyType fileKey, string contents);

	public void WriteAllText(TfileKeyType fileKey, string contents, Encoding encoding);

	public byte[] ReadAllBytes(TfileKeyType fileKey);

	public void WriteAllBytes(TfileKeyType fileKey, byte[] bytes);

	public void AppendAllBytes(TfileKeyType fileKey, byte[] bytes);

	public string[] ReadAllLines(TfileKeyType fileKey);

	public string[] ReadAllLines(TfileKeyType fileKey, Encoding encoding);

	public void WriteAllLines(TfileKeyType fileKey, string[] contents);

	public void WriteAllLines(TfileKeyType fileKey, string[] contents, Encoding encoding);

	public void AppendAllText(TfileKeyType fileKey, string contents);

	public void AppendAllText(TfileKeyType fileKey, string contents, Encoding encoding);

}


