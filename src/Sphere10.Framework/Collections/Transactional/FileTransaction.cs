using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace Sphere10.Framework {

	public class FileTransaction : IDisposable {
		private static readonly SynchronizedDictionary<string, TransactionalFileMappedBuffer> GloballyEnlistedFiles;
		private readonly IDictionary<string, TransactionalFileMappedBuffer> _enlistedFiles;

		static FileTransaction() {
			GloballyEnlistedFiles = new SynchronizedDictionary<string, TransactionalFileMappedBuffer>();
		}

		public FileTransaction(string transactionHeaderFilePath)
			: this(transactionHeaderFilePath, Path.GetDirectoryName(transactionHeaderFilePath)) {
		}

		public FileTransaction(string transactionHeaderFilePath, string uncomittedPageFileDirectory) {
			_enlistedFiles = new Dictionary<string, TransactionalFileMappedBuffer>();

			// Resume prior transaction if already exists, resume
			if (File.Exists(transactionHeaderFilePath)) {
				LoadHeader(transactionHeaderFilePath);
				switch (Status) {
					case FileTransactionState.Unchanged:
					case FileTransactionState.HasChanges:
					case FileTransactionState.RollingBack:
						ApplyRollback();
						DelistAll();
						break;
					case FileTransactionState.Committing:
						ApplyCommit();
						DelistAll();
						break;
					default:
						throw new ArgumentOutOfRangeException();
				}
			}
			Debug.Assert(!_enlistedFiles.Any());
			Status = FileTransactionState.Unchanged;
			TransactionHeaderFile = transactionHeaderFilePath;
			UncomittedPageFileDirectory = uncomittedPageFileDirectory;
		}

		public string TransactionHeaderFile { get; private set; }

		public TransactionalFileMappedBuffer[] EnlistedFiles => _enlistedFiles.Values.ToArray();

		public FileTransactionState Status { get; private set; }

		public string UncomittedPageFileDirectory { get; private set; }

		public void Flush() {
			foreach (var file in _enlistedFiles.Values) {
				file.Flush();
			}
		}

		public void Commit() {
			// Todo event
			switch (Status) {
				case FileTransactionState.Unchanged:
					break;
				case FileTransactionState.HasChanges:
					ApplyCommit();
					break;
				case FileTransactionState.Committing:
				case FileTransactionState.RollingBack:
					throw new InvalidOperationException("Illegal state");
				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		public void Rollback() {
			// Todo event
			switch (Status) {
				case FileTransactionState.Unchanged:
					break;
				case FileTransactionState.HasChanges:
					ApplyRollback();
					break;
				case FileTransactionState.Committing:
				case FileTransactionState.RollingBack:
					throw new InvalidOperationException("Illegal state");
				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		public TransactionalFileMappedBuffer EnlistFile(string filename, int pageSize, int maxOpenPages) {
			return EnlistFile(filename, Guid.NewGuid(), pageSize, maxOpenPages);
		}

		public TransactionalFileMappedBuffer EnlistFile(string filename, Guid fileID, int pageSize, int maxOpenPages) {
			Guard.ArgumentNotNullOrEmpty(filename, nameof(filename));

			if (File.Exists(filename)) {
				// Normalize filename
				filename = Tools.FileSystem.GetCaseCorrectFilePath(filename);
			}

			using (GloballyEnlistedFiles.EnterWriteScope()) {
				// validate not already enlisted here
				if (_enlistedFiles.ContainsKey(filename))
					throw new InvalidOperationException($"File already enlisted: {filename})");

				// check not globally enlisted
				if (GloballyEnlistedFiles.ContainsKey(filename))
					throw new InvalidOperationException($"File already enlisted in other transaction: {filename})");

				return EnlistFile(new TransactionalFileMappedBuffer(filename, UncomittedPageFileDirectory, fileID, pageSize, maxOpenPages, false));
			}
		}

		public TransactionalFileMappedBuffer EnlistFile(ITransactionalFile transactionalFile) {
			TransactionalFileMappedBuffer file;
			using (GloballyEnlistedFiles.EnterWriteScope()) {
				// validate not already enlisted here
				if (_enlistedFiles.ContainsKey(transactionalFile.Path))
					throw new InvalidOperationException($"File already enlisted: {transactionalFile.Path})");

				// check not globally enlisted
				if (GloballyEnlistedFiles.ContainsKey(transactionalFile.Path))
					throw new InvalidOperationException($"File already enlisted in other transaction: {transactionalFile.Path})");

				// Open the file
				file = transactionalFile.AsBuffer;
				if (file.RequiresLoad)
					file.Load();
				file.PageWrite += (o, page) => {
					if (Status == FileTransactionState.Unchanged) {
						Status = FileTransactionState.HasChanges;
						SaveHeader();
					}
				};

				// Register file
				_enlistedFiles.Add(transactionalFile.Path, file);
				GloballyEnlistedFiles.Add(transactionalFile.Path, file);
			}
			// Save txn update
			SaveHeader();
			return file;
		}

		public void DelistFile(string filename) {
			Guard.ArgumentNotNullOrEmpty(filename, nameof(filename));
			Guard.FileExists(filename);
			filename = Tools.FileSystem.GetCaseCorrectFilePath(filename);
			Guard.Argument(_enlistedFiles.ContainsKey(filename), nameof(filename), $"File not enlisted: {filename}");
			DelistFile(_enlistedFiles[filename]);
		}

		public void DelistFile(TransactionalFileMappedBuffer file) {
			Guard.ArgumentNotNull(file, nameof(file));
			GloballyEnlistedFiles.Remove(file.Path);
			_enlistedFiles.Remove(file.Path);
			file.Dispose();
		}

		public void Dispose() {
			DelistAll();
			File.Delete(TransactionHeaderFile);
		}

		protected virtual void ApplyCommit() {
			Status = FileTransactionState.Committing;
			SaveHeader();
			foreach (var file in _enlistedFiles.Values) {
				file.Commit();
			}
			Status = FileTransactionState.Unchanged;
			SaveHeader();
		}

		protected virtual void ApplyRollback() {
			Status = FileTransactionState.RollingBack;
			SaveHeader();
			foreach (var file in _enlistedFiles.Values) {
				file.Rollback();
			}
			Status = FileTransactionState.Unchanged;
			SaveHeader();
		}

		private void DelistAll() {
			foreach (var file in _enlistedFiles.Keys.ToArray())
				DelistFile(file);
		}

		private void SaveHeader() {
			// Only save header file when there is a change, delete header file when no changes
			if (Status != FileTransactionState.Unchanged) {
				File.WriteAllBytes(TransactionHeaderFile, Tools.Object.SerializeToByteArray(FileTransactionSerializableSurrogate.Dehydrate(this)));
			} else {
				File.Delete(TransactionHeaderFile);
			}
		}

		private void LoadHeader(string transactionHeaderFile) {
			if (!File.Exists(transactionHeaderFile))
				throw new FileNotFoundException("File not found.", transactionHeaderFile);

			var surrogate = Tools.Object.DeserializeFromByteArray(File.ReadAllBytes(transactionHeaderFile)) as FileTransactionSerializableSurrogate;
			this.Status = surrogate.Status;
			this.TransactionHeaderFile = transactionHeaderFile;
			this.UncomittedPageFileDirectory = surrogate.UncomittedPageFileDirectory;
			foreach (var enlistedSurrogate in surrogate.EnlistedFiles) {
				EnlistFile(enlistedSurrogate.Filename, enlistedSurrogate.FileID, enlistedSurrogate.PageSize, enlistedSurrogate.MaxOpenPages);
			}
		}

		private void CheckWritable() {
			switch (Status) {
				case FileTransactionState.Unchanged:
				case FileTransactionState.HasChanges:
					break;
				case FileTransactionState.Committing:
				case FileTransactionState.RollingBack:
				default:
					throw new InvalidOperationException($"Cannot write when file transaction is {Status}");
			}
		}

		public static bool IsEnlisted(string path) {
			Guard.ArgumentNotNull(path, nameof(path));
			Guard.FileExists(path);
			path = Tools.FileSystem.GetCaseCorrectFilePath(path);
			return GloballyEnlistedFiles.ContainsKey(path);
		}

		#region Serializable Surrogates

		[Serializable]
		public class TransactionalFileSerializableSurrogate {
			public string Filename { get; set; }
			public Guid FileID { get; set; }
			public int PageSize { get; set; }
			public int MaxOpenPages { get; set; }

			public TransactionalFileSerializableSurrogate() {
			}

			public TransactionalFileSerializableSurrogate(TransactionalFileMappedBuffer file) {
				Dehydrate(file, this);
			}

			public static void Dehydrate(TransactionalFileMappedBuffer @from, TransactionalFileSerializableSurrogate to) {
				to.Filename = from.Path;
				to.FileID = from.FileID;
				to.PageSize = from.PageSize;
				to.MaxOpenPages = from.MaxOpenPages;
			}

			public static TransactionalFileSerializableSurrogate Dehydrate(TransactionalFileMappedBuffer transaction) {
				var surrogate = new TransactionalFileSerializableSurrogate();
				Dehydrate(transaction, surrogate);
				return surrogate;
			}

		}

		[Serializable]
		public class FileTransactionSerializableSurrogate {
			public TransactionalFileSerializableSurrogate[] EnlistedFiles { get; set; }
			public FileTransactionState Status { get; set; }
			public string UncomittedPageFileDirectory { get; set; }

			public FileTransactionSerializableSurrogate() {
			}

			public FileTransactionSerializableSurrogate(FileTransaction transaction) {
				Dehydrate(transaction, this);
			}

			public static void Dehydrate(FileTransaction @from, FileTransactionSerializableSurrogate to) {
				to.UncomittedPageFileDirectory = @from.UncomittedPageFileDirectory;
				to.Status = @from.Status;
				to.EnlistedFiles = @from.EnlistedFiles.Select(TransactionalFileSerializableSurrogate.Dehydrate).ToArray();
			}

			public static FileTransactionSerializableSurrogate Dehydrate(FileTransaction transaction) {
				var surrogate = new FileTransactionSerializableSurrogate();
				Dehydrate(transaction, surrogate);
				return surrogate;
			}

		}

		#endregion
	}

}