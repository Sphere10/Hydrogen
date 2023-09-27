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
using System.Linq;
using System.Text;


namespace Hydrogen.Data;

public class KeyTransformedFileStore<TFromKey, TToKey> : IFileStore<TToKey> {
	protected readonly IFileStore<TFromKey> InternalFileStore;
	private readonly Func<TFromKey, TToKey> _fromTransformer;
	private readonly Func<TToKey, TFromKey> _toTransformer;

	public KeyTransformedFileStore(IFileStore<TFromKey> internalFileStore, Func<TFromKey, TToKey> fromTransform, Func<TToKey, TFromKey> toTransform) {
		InternalFileStore = internalFileStore;
		_fromTransformer = fromTransform;
		_toTransformer = toTransform;
	}

	public void Dispose() => InternalFileStore.Dispose();

	public IEnumerable<TToKey> FileKeys => InternalFileStore.FileKeys.Select(_fromTransformer);

	public TToKey RecommendFileKey(string externalFilePath) => _fromTransformer(InternalFileStore.RecommendFileKey(externalFilePath));

	public string GetFilePath(TToKey fileKey) => InternalFileStore.GetFilePath(_toTransformer(fileKey));

	public bool ContainsFile(TToKey fileKey) => InternalFileStore.ContainsFile(_toTransformer(fileKey));

	public TToKey NewFile() => _fromTransformer(InternalFileStore.NewFile());

	public string[] RegisterMany(IEnumerable<TToKey> fileKeys) => InternalFileStore.RegisterMany(fileKeys.Select(_toTransformer));

	public void DeleteMany(IEnumerable<TToKey> fileKeys) => InternalFileStore.DeleteMany(fileKeys.Select(_toTransformer));

	public void Clear() => InternalFileStore.Clear();

	public FileInfo GetInfo(TToKey fileKey) => InternalFileStore.GetInfo(_toTransformer(fileKey));

	public Stream Open(TToKey fileKey, FileMode mode) => InternalFileStore.Open(_toTransformer(fileKey), mode);

	public Stream Open(TToKey fileKey, FileMode mode, FileAccess access) => InternalFileStore.Open(_toTransformer(fileKey), mode, access);

	public Stream Open(TToKey fileKey, FileMode mode, FileAccess access, FileShare share) => InternalFileStore.Open(_toTransformer(fileKey), mode, access, share);

	public void SetCreationTime(TToKey fileKey, DateTime creationTime) => InternalFileStore.SetCreationTime(_toTransformer(fileKey), creationTime);

	public void SetCreationTimeUtc(TToKey fileKey, DateTime creationTimeUtc) => InternalFileStore.SetCreationTimeUtc(_toTransformer(fileKey), creationTimeUtc);

	public DateTime GetCreationTime(TToKey fileKey) => InternalFileStore.GetCreationTime(_toTransformer(fileKey));

	public DateTime GetCreationTimeUtc(TToKey fileKey) => InternalFileStore.GetCreationTimeUtc(_toTransformer(fileKey));

	public void SetLastAccessTime(TToKey fileKey, DateTime lastAccessTime) => InternalFileStore.SetLastAccessTime(_toTransformer(fileKey), lastAccessTime);

	public void SetLastAccessTimeUtc(TToKey fileKey, DateTime lastAccessTimeUtc) => InternalFileStore.SetLastAccessTime(_toTransformer(fileKey), lastAccessTimeUtc);

	public DateTime GetLastAccessTime(TToKey fileKey) => InternalFileStore.GetLastAccessTime(_toTransformer(fileKey));

	public DateTime GetLastAccessTimeUtc(TToKey fileKey) => InternalFileStore.GetLastAccessTimeUtc(_toTransformer(fileKey));

	public void SetLastWriteTime(TToKey fileKey, DateTime lastWriteTime) => InternalFileStore.SetLastWriteTime(_toTransformer(fileKey), lastWriteTime);

	public void SetLastWriteTimeUtc(TToKey fileKey, DateTime lastWriteTimeUtc) => InternalFileStore.SetLastWriteTimeUtc(_toTransformer(fileKey), lastWriteTimeUtc);

	public DateTime GetLastWriteTime(TToKey fileKey) => InternalFileStore.GetLastWriteTime(_toTransformer(fileKey));

	public DateTime GetLastWriteTimeUtc(TToKey fileKey) => InternalFileStore.GetLastWriteTimeUtc(_toTransformer(fileKey));

	public FileAttributes GetAttributes(TToKey fileKey) => InternalFileStore.GetAttributes(_toTransformer(fileKey));

	public void SetAttributes(TToKey fileKey, FileAttributes fileAttributes) => InternalFileStore.SetAttributes(_toTransformer(fileKey), fileAttributes);

	public Stream OpenRead(TToKey fileKey) => InternalFileStore.OpenRead(_toTransformer(fileKey));

	public Stream OpenWrite(TToKey fileKey) => InternalFileStore.OpenWrite(_toTransformer(fileKey));

	public string ReadAllText(TToKey fileKey) => InternalFileStore.ReadAllText(_toTransformer(fileKey));

	public string ReadAllText(TToKey fileKey, Encoding encoding) => InternalFileStore.ReadAllText(_toTransformer(fileKey), encoding);

	public void WriteAllText(TToKey fileKey, string contents) => InternalFileStore.WriteAllText(_toTransformer(fileKey), contents);

	public void WriteAllText(TToKey fileKey, string contents, Encoding encoding) => InternalFileStore.WriteAllText(_toTransformer(fileKey), contents, encoding);

	public byte[] ReadAllBytes(TToKey fileKey) => InternalFileStore.ReadAllBytes(_toTransformer(fileKey));

	public void WriteAllBytes(TToKey fileKey, byte[] bytes) => InternalFileStore.WriteAllBytes(_toTransformer(fileKey), bytes);

	public void AppendAllBytes(TToKey fileKey, byte[] bytes) => InternalFileStore.AppendAllBytes(_toTransformer(fileKey), bytes);

	public string[] ReadAllLines(TToKey fileKey) => InternalFileStore.ReadAllLines(_toTransformer(fileKey));

	public string[] ReadAllLines(TToKey fileKey, Encoding encoding) => InternalFileStore.ReadAllLines(_toTransformer(fileKey), encoding);

	public void WriteAllLines(TToKey fileKey, string[] contents) => InternalFileStore.WriteAllLines(_toTransformer(fileKey), contents);

	public void WriteAllLines(TToKey fileKey, string[] contents, Encoding encoding) => InternalFileStore.WriteAllLines(_toTransformer(fileKey), contents, encoding);

	public void AppendAllText(TToKey fileKey, string contents) => InternalFileStore.AppendAllText(_toTransformer(fileKey), contents);

	public void AppendAllText(TToKey fileKey, string contents, Encoding encoding) => InternalFileStore.AppendAllText(_toTransformer(fileKey), contents, encoding);

}
