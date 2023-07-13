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

namespace Hydrogen.Data;

/// <summary>
/// Stores files as GUID's with first two digits as a parent directory, and attaches a default file extension (if specified).
/// </summary>
/// <example>{504C1439-05D1-422A-906E-42FB8552FA5B} -> {BaseDir}/50/4c143905d1422a906e42fb8552fa5b.json</example>
public class GuidFileStore : FileStoreBase<Guid> {

	public GuidFileStore() : this(Path.GetTempPath()) {
	}

	public GuidFileStore(string baseDirectory) {
		BaseDirectory = baseDirectory;
	}

	public override IEnumerable<Guid> FileKeys =>
		Directory
			.EnumerateDirectories(BaseDirectory)
			.SelectMany(prefixDir => Directory.EnumerateFiles(prefixDir, $"*{FileExtension}"))
			.Select(x => Path.GetRelativePath(BaseDirectory, x)) // remove BaseDirectory
			.Select(x => x.TrimEnd(FileExtension).Split(Path.DirectorySeparatorChar, StringSplitOptions.RemoveEmptyEntries).Take(2).ToDelimittedString(string.Empty))
			.Select(x => (Guid.TryParse(x, out var val), val)) // ignores file system entries that don't parse as guid
			.Where(x => x.Item1)
			.Select(x => x.Item2);

	public FileStorePersistencePolicy PersistencePolicy { get; init; } = FileStorePersistencePolicy.Perist;

	public string BaseDirectory { get; }

	public string FileExtension { get; init; } = string.Empty;

	public override Guid RecommendFileKey(string externalFilePath)
		=> Guid.NewGuid();

	public override string GetFilePath(Guid fileKey) {
		var sanitizedGuid = ToGuidString(fileKey);
		return Path.Combine(BaseDirectory, sanitizedGuid.Substring(0, 2), sanitizedGuid.Substring(2) + FileExtension);
	}

	public override bool ContainsFile(Guid fileKey)
		=> File.Exists(GetFilePath(fileKey));

	public override Guid NewFile() {
		var fileKey = Guid.NewGuid();
		this.RegisterFile(fileKey);
		return fileKey;
	}

	public override string[] RegisterMany(IEnumerable<Guid> fileKeys) {
		var filePaths = new List<string>();
		foreach (var fileKey in fileKeys) {
			var filePath = GetFilePath(fileKey);
			Guard.Against(File.Exists(filePath), $"File '{fileKey}' already exists");
			Tools.FileSystem.CreateBlankFile(filePath, true);
			filePaths.Add(filePath);
		}
		return filePaths.ToArray();
	}

	public override void DeleteMany(IEnumerable<Guid> fileKeys) {
		foreach (var fileKey in fileKeys) {
			var filePath = GetFilePath(fileKey);
			Guard.Ensure(File.Exists(filePath), $"File '{fileKey}' not found");
			Tools.FileSystem.DeleteFile(filePath);
			var parent = Tools.FileSystem.GetParentDirectoryPath(filePath, 1);
			if (!Directory.EnumerateFileSystemEntries(parent).Any())
				Tools.FileSystem.DeleteDirectory(parent);
		}
	}

	public override void Clear() {
		Tools.FileSystem.DeleteAllFiles(BaseDirectory, true);
	}

	protected override void FreeManagedResources() {
		if (PersistencePolicy == FileStorePersistencePolicy.DeleteOnDispose) {
			Clear();
		}
	}

	private string ToGuidString(Guid guid)
		=> guid.ToStrictAlphaString().ToLowerInvariant();

}
