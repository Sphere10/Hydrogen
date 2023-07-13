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
/// Stores files in a directory with a unique GUID filename with keys being unique strings. An XML file is stored to track all the files.
/// </summary>
public class SimpleFileStore : FileStoreBase<string> {
	private const string DefaultFilename = "Untitled";
	private const string DefaultFileStoreDictionaryFileName = "__FileStoreRegistry.xml";
	private readonly IPersistedDictionary<string, string> _fileDictionary;
	private readonly SynchronizedDictionary<string, string> _synchronizedFileDictionary;

	public SimpleFileStore() : this(Path.GetTempPath()) {
	}

	public SimpleFileStore(string baseDirectory)
		: this(baseDirectory, FileStorePersistencePolicy.Perist) {
	}

	public SimpleFileStore(string baseDirectory, FileStorePersistencePolicy persistencePolicy)
		: this(baseDirectory, new XmlFileDictionary<string, string>(Path.Combine(baseDirectory, DefaultFileStoreDictionaryFileName), true), persistencePolicy) {
	}

	public SimpleFileStore(string baseDirectory, IPersistedDictionary<string, string> fileRegistry, FileStorePersistencePolicy persistencePolicy = FileStorePersistencePolicy.Perist) {
		PersistencePolicy = persistencePolicy;
		if (!Directory.Exists(baseDirectory)) {
			Directory.CreateDirectory(baseDirectory);
		}
		_fileDictionary = fileRegistry;
		_synchronizedFileDictionary = new SynchronizedDictionary<string, string>(fileRegistry);
		using (_synchronizedFileDictionary.EnterWriteScope()) {
			_fileDictionary.Load();

			// Delete files not cleaned up since last use
			if (_synchronizedFileDictionary.Any() && persistencePolicy == FileStorePersistencePolicy.DeleteOnDispose) {
				_synchronizedFileDictionary.Values.ForEach(file => Tools.Exceptions.ExecuteIgnoringException(() => File.Delete(file)));
				_synchronizedFileDictionary.Clear();
				_fileDictionary.Save();
			}
		}
		BaseDirectory = baseDirectory;
	}

	public override IEnumerable<string> FileKeys => _synchronizedFileDictionary.Values;

	public FileStorePersistencePolicy PersistencePolicy { get; set; }

	protected string BaseDirectory { get; set; }

	public override string RecommendFileKey(string externalFilePath) {
		var i = 0;
		string proposedKey;
		do {
			proposedKey = $"{DefaultFilename}_{++i}";
		} while (_fileDictionary.ContainsKey(proposedKey));
		return proposedKey;
	}

	public override string GetFilePath(string fileKey) {
		using (_synchronizedFileDictionary.EnterWriteScope()) {
			if (!ContainsFile(fileKey))
				throw new SoftwareException("No file '{0}' has been registered in the file store", fileKey);
			return Path.Combine(BaseDirectory, _synchronizedFileDictionary[fileKey]);
		}
	}

	public override bool ContainsFile(string alias) {
		return _synchronizedFileDictionary.ContainsKey(alias);
	}

	public override string NewFile() {
		var fileKey = Guid.NewGuid().ToStrictAlphaString();
		this.RegisterFile(fileKey);
		return fileKey;
	}

	public override string[] RegisterMany(IEnumerable<string> fileKeys) {
		var files = new List<string>();
		using (_synchronizedFileDictionary.EnterWriteScope()) {
			if (fileKeys.Any(alias => alias == DefaultFileStoreDictionaryFileName)) {
				throw new SoftwareException("Registering a file with alias '{0}' is prohibited", DefaultFileStoreDictionaryFileName);
			}
			foreach (var fileKey in fileKeys) {
				if (!ContainsFile(fileKey)) {
					_synchronizedFileDictionary[fileKey] = GenerateInternalRelFilePath(fileKey);
					var filePath = GetFilePath(fileKey);
					File.WriteAllBytes(filePath, Array.Empty<byte>());
					files.Add(filePath);
				}
			}
			_fileDictionary.Save();
		}
		return files.ToArray();
	}

	public override void DeleteMany(IEnumerable<string> fileKeys) {
		using (_synchronizedFileDictionary.EnterWriteScope()) {
			var exceptions = new List<Exception>();
			foreach (var file in fileKeys) {
				try {
					File.Delete(GetFilePath(file));
					if (_synchronizedFileDictionary.ContainsKey(file))
						_synchronizedFileDictionary.Remove(_synchronizedFileDictionary[file]);
				} catch (Exception error) {
					exceptions.Add(error);
				}
			}
			_fileDictionary.Save();
			if (exceptions.Count > 0)
				throw new AggregateException(exceptions);
		}
	}

	public override void Clear() {
		using (_synchronizedFileDictionary.EnterWriteScope()) {
			DeleteMany(_synchronizedFileDictionary.Keys);
			_fileDictionary.Delete();
		}
	}

	protected virtual string GenerateInternalRelFilePath(string fileKey) {
		return Guid.NewGuid().ToStrictAlphaString().ToLowerInvariant() + ".file";
	}

	protected override void FreeManagedResources() {
		if (PersistencePolicy == FileStorePersistencePolicy.DeleteOnDispose) {
			Clear();
		}
	}

}
