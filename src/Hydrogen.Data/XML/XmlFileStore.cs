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


namespace Hydrogen.Data {

	public class XmlFileStore : Disposable {
		private const string DefaultFileStoreDictionaryFileName = "__FileStoreRegistry.xml";
		private readonly IPersistedDictionary<string, string> _fileDictionary;
		private readonly SynchronizedDictionary<string, string> _synchronizedFileDictionary;

		public XmlFileStore() : this(Path.GetTempPath()) {
		}

		public XmlFileStore(string baseDirectory) 
			: this (baseDirectory, XmlFileStorePersistencePolicy.Perist) {
		}

		public XmlFileStore(string baseDirectory, XmlFileStorePersistencePolicy persistencePolicy) 
			: this (baseDirectory, new XmlFileDictionary<string, string>(Path.Combine(baseDirectory, DefaultFileStoreDictionaryFileName), true), persistencePolicy) {
		}

		public XmlFileStore(string baseDirectory, IPersistedDictionary<string, string> fileRegistry, XmlFileStorePersistencePolicy persistencePolicy = XmlFileStorePersistencePolicy.Perist) {
			PersistencePolicy = persistencePolicy;
			if (!Directory.Exists(baseDirectory)) {
				Directory.CreateDirectory(baseDirectory);
			}
			_fileDictionary = fileRegistry;
			_synchronizedFileDictionary = new SynchronizedDictionary<string, string>(fileRegistry);
			using (_synchronizedFileDictionary.EnterWriteScope()) {
				_fileDictionary.Load();

				// Delete files not cleaned up since last use
				if (_synchronizedFileDictionary.Any() && persistencePolicy == XmlFileStorePersistencePolicy.DeleteOnDispose) {
					_synchronizedFileDictionary.Values.ForEach(file => Tools.Exceptions.ExecuteIgnoringException(() => File.Delete(file)));
					_synchronizedFileDictionary.Clear();
					_fileDictionary.Save();
				}
			}
			BaseDirectory = baseDirectory;
		}

		#region Properties

		public IEnumerable<string> FileAliases => _synchronizedFileDictionary.Values;

		public XmlFileStorePersistencePolicy PersistencePolicy { get; set; }

		protected string BaseDirectory { get; set; }

		#endregion

		#region Methods

		public virtual string GetFilePath(string fileAlias) {
			using (_synchronizedFileDictionary.EnterWriteScope()) {
				if (!ContainsAlias(fileAlias))
					throw new SoftwareException("No file '{0}' has been registered in the file store", fileAlias);
				return Path.Combine(BaseDirectory, _synchronizedFileDictionary[fileAlias]);
			}
		}

        public virtual bool ContainsAlias(string alias) {
			return _synchronizedFileDictionary.ContainsKey(alias);
		}

		public virtual string NewFile() {
			var fileAlias = Guid.NewGuid().ToStrictAlphaString();
			RegisterFile(fileAlias);
			return fileAlias;
		}

        public virtual void RegisterFile(string fileAlias) {
			RegisterMany(new[] { fileAlias });
		}

        public virtual void RegisterMany(IEnumerable<string> fileAliases) {
	        using (_synchronizedFileDictionary.EnterWriteScope()) {
		        if (fileAliases.Any(alias => alias == DefaultFileStoreDictionaryFileName)) {
			        throw new SoftwareException("Registering a file with alias '{0}' is prohibited", DefaultFileStoreDictionaryFileName);
		        }
		        foreach (var fileAlias in fileAliases) {
			        if (!ContainsAlias(fileAlias)) {
						_synchronizedFileDictionary[fileAlias] = GenerateInternalFilePath(fileAlias);
						File.WriteAllBytes(GetFilePath(fileAlias), new byte[0]);				        
			        }
		        }
				_fileDictionary.Save();
	        }
        }

        public virtual void Delete(string fileAlias) {
			DeleteMany(new[] { fileAlias });
		}

        public virtual void DeleteMany(IEnumerable<string> fileAliases) {
	        using (_synchronizedFileDictionary.EnterWriteScope()) {
		        var exceptions = new List<Exception>();
		        foreach (var file in fileAliases) {
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

        public virtual void Clear() {
	        using (_synchronizedFileDictionary.EnterWriteScope()) {
		        DeleteMany(_synchronizedFileDictionary.Keys);
				_fileDictionary.Delete();
	        }
        }

		#endregion

		#region File Access Methods

		public FileInfo GetInfo(string fileAlias) {
			return new FileInfo(GetFilePath(fileAlias));
		}

		public FileStream Open(string fileAlias, FileMode mode) {
			return File.Open(GetFilePath(fileAlias), mode);
		}

		public FileStream Open(string fileAlias, FileMode mode, FileAccess access) {
			return File.Open(GetFilePath(fileAlias), mode, access);
		}

		public FileStream Open(string fileAlias, FileMode mode, FileAccess access, FileShare share) {
			return File.Open(GetFilePath(fileAlias), mode, access, share);
		}

		public void SetCreationTime(string fileAlias, DateTime creationTime) {
			File.SetCreationTime(GetFilePath(fileAlias), creationTime);
		}

		public void SetCreationTimeUtc(string fileAlias, DateTime creationTimeUtc) {
			File.SetCreationTimeUtc(GetFilePath(fileAlias), creationTimeUtc);
		}

		public DateTime GetCreationTime(string fileAlias) {
			return File.GetCreationTime(GetFilePath(fileAlias));
		}

		public DateTime GetCreationTimeUtc(string fileAlias) {
			return File.GetCreationTimeUtc(GetFilePath(fileAlias));
		}

		public void SetLastAccessTime(string fileAlias, DateTime lastAccessTime) {
			File.SetLastAccessTime(GetFilePath(fileAlias), lastAccessTime);
		}

		public void SetLastAccessTimeUtc(string fileAlias, DateTime lastAccessTimeUtc) {
			File.SetLastAccessTimeUtc(GetFilePath(fileAlias), lastAccessTimeUtc);
		}

		public DateTime GetLastAccessTime(string fileAlias) {
			return File.GetLastAccessTime(GetFilePath(fileAlias));
		}

		public DateTime GetLastAccessTimeUtc(string fileAlias) {
			return File.GetLastAccessTimeUtc(GetFilePath(fileAlias));
		}

		public void SetLastWriteTime(string fileAlias, DateTime lastWriteTime) {
			File.SetLastAccessTime(GetFilePath(fileAlias), lastWriteTime);
		}

		public void SetLastWriteTimeUtc(string fileAlias, DateTime lastWriteTimeUtc) {
			File.SetLastWriteTimeUtc(GetFilePath(fileAlias), lastWriteTimeUtc);
		}

		public DateTime GetLastWriteTime(string fileAlias) {
			return File.GetLastAccessTime(GetFilePath(fileAlias));
		}

		public DateTime GetLastWriteTimeUtc(string fileAlias) {
			return File.GetLastWriteTimeUtc(GetFilePath(fileAlias));
		}

		public FileAttributes GetAttributes(string fileAlias) {
			return File.GetAttributes(GetFilePath(fileAlias));
		}

		public void SetAttributes(string fileAlias, FileAttributes fileAttributes) {
			File.SetAttributes(GetFilePath(fileAlias), fileAttributes);
		}

		public FileStream OpenRead(string fileAlias) {
			return File.OpenRead(GetFilePath(fileAlias));
		}

		public FileStream OpenWrite(string fileAlias) {
			return File.OpenWrite(GetFilePath(fileAlias));
		}

		public string ReadAllText(string fileAlias) {
			return File.ReadAllText(GetFilePath(fileAlias));
		}

		public string ReadAllText(string fileAlias, Encoding encoding) {
			return File.ReadAllText(GetFilePath(fileAlias), encoding);
		}

		public void WriteAllText(string fileAlias, string contents) {
			File.WriteAllText(GetFilePath(fileAlias), contents);
		}

		public void WriteAllText(string fileAlias, string contents, Encoding encoding) {
			File.WriteAllText(GetFilePath(fileAlias), contents, encoding);
		}

		public byte[] ReadAllBytes(string fileAlias) {
			return File.ReadAllBytes(GetFilePath(fileAlias));
		}

		public void WriteAllBytes(string fileAlias, byte[] bytes) {
			File.WriteAllBytes(GetFilePath(fileAlias), bytes);
		}

		public void AppendAllBytes(string fileAlias, byte[] bytes) {
			Tools.FileSystem.AppendAllBytes(GetFilePath(fileAlias), bytes);
		}

		public string[] ReadAllLines(string fileAlias) {
			return File.ReadAllLines(GetFilePath(fileAlias));
		}

		public string[] ReadAllLines(string fileAlias, Encoding encoding) {
			return File.ReadAllLines(GetFilePath(fileAlias), encoding);
		}

		public void WriteAllLines(string fileAlias, string[] contents) {
			File.WriteAllLines(GetFilePath(fileAlias), contents);
		}

		public void WriteAllLines(string fileAlias, string[] contents, Encoding encoding) {
			File.WriteAllLines(GetFilePath(fileAlias), contents, encoding);
		}

		public void AppendAllText(string fileAlias, string contents) {
			File.AppendAllText(GetFilePath(fileAlias), contents);
		}

		public void AppendAllText(string fileAlias, string contents, Encoding encoding) {
			File.AppendAllText(GetFilePath(fileAlias), contents, encoding);
		}

		#endregion

		#region Internal Methods

		protected virtual string GenerateInternalFilePath(string fileAlias) {
			return  Guid.NewGuid().ToStrictAlphaString().ToLowerInvariant() + ".file";
		}

		protected override void FreeManagedResources() {
			if (PersistencePolicy == XmlFileStorePersistencePolicy.DeleteOnDispose) {
				Clear();
			}
		}

		#endregion

	}
}

