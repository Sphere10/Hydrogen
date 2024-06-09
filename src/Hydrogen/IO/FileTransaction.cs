// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT NON-AI software license, see the accompanying file
// LICENSE or visit https://sphere10.com/legal/NON-AI-MIT.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace Hydrogen;

public class FileTransaction : IDisposable {
	private static readonly SynchronizedDictionary<string, TransactionalFileMappedBuffer> GloballyEnlistedFiles;
	private readonly IDictionary<string, FileDescriptor> _enlistedFiles;

	static FileTransaction() {
		GloballyEnlistedFiles = new SynchronizedDictionary<string, TransactionalFileMappedBuffer>();
	}

	public FileTransaction(string transactionHeaderFilePath)
		: this(transactionHeaderFilePath, Path.GetDirectoryName(transactionHeaderFilePath)) {
	}

	public FileTransaction(string transactionHeaderFilePath, string uncommittedPageFileDirectory) {
		_enlistedFiles = new Dictionary<string, FileDescriptor>();

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
		UncommittedPageFileDirectory = uncommittedPageFileDirectory;
	}

	public string TransactionHeaderFile { get; private set; }

	public TransactionalFileMappedBuffer[] EnlistedFiles => _enlistedFiles.Values.Select(x => x.Buffer).ToArray();

	public FileTransactionState Status { get; private set; }

	public string UncommittedPageFileDirectory { get; private set; }

	public void Flush() {
		foreach (var file in _enlistedFiles.Values) {
			file.Buffer.Flush();
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

	public TransactionalFileMappedBuffer EnlistFile(string filename, long pageSize, long maxMemory) {
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

			//return EnlistFile(new TransactionalFileMappedBuffer(TransactionalFileDescriptor.From(filename, UncommittedPageFileDirectory, pageSize, maxMemory, false), true);
			return EnlistFile(
				new TransactionalFileMappedBuffer(
					TransactionalFileDescriptor.From(filename, UncommittedPageFileDirectory, pageSize, maxMemory), 
					FileAccessMode.Append
				),
				true
			);
		}
	}

	public TransactionalFileMappedBuffer EnlistFile(ITransactionalFile transactionalFile, bool ownsFile) {
		TransactionalFileMappedBuffer transactionalBuffer;
		using (GloballyEnlistedFiles.EnterWriteScope()) {
			// validate not already enlisted here
			if (_enlistedFiles.ContainsKey(transactionalFile.FileDescriptor.Path))
				throw new InvalidOperationException($"File already enlisted: {transactionalFile.FileDescriptor.Path})");

			// check not globally enlisted
			if (GloballyEnlistedFiles.ContainsKey(transactionalFile.FileDescriptor.Path))
				throw new InvalidOperationException($"File already enlisted in other transaction: {transactionalFile.FileDescriptor.Path})");

			// Open the file
			transactionalBuffer = transactionalFile.AsBuffer;
			if (transactionalBuffer.RequiresLoad)
				transactionalBuffer.Load();
			transactionalBuffer.PageWrite += (o, page) => {
				if (Status == FileTransactionState.Unchanged) {
					Status = FileTransactionState.HasChanges;
					SaveHeader();
				}
			};

			// Register file
			_enlistedFiles.Add(transactionalFile.FileDescriptor.Path, new FileDescriptor { Buffer = transactionalBuffer, ShouldDispose = ownsFile });
			GloballyEnlistedFiles.Add(transactionalFile.FileDescriptor.Path, transactionalBuffer);
		}
		// Save txn update
		SaveHeader();
		return transactionalBuffer;
	}

	public void DelistFile(string filename) {
		Guard.ArgumentNotNullOrEmpty(filename, nameof(filename));
		Guard.FileExists(filename);
		filename = Tools.FileSystem.GetCaseCorrectFilePath(filename);
		Guard.Argument(_enlistedFiles.ContainsKey(filename), nameof(filename), $"File not enlisted: {filename}");
		DelistFile(_enlistedFiles[filename].Buffer);
	}

	public void DelistFile(TransactionalFileMappedBuffer file) {
		Guard.ArgumentNotNull(file, nameof(file));
		var shouldDispose = _enlistedFiles[file.FileDescriptor.Path].ShouldDispose;
		GloballyEnlistedFiles.Remove(file.FileDescriptor.Path);
		_enlistedFiles.Remove(file.FileDescriptor.Path);
		if (shouldDispose)
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
			file.Buffer.Commit();
		}
		Status = FileTransactionState.Unchanged;
		SaveHeader();
	}

	protected virtual void ApplyRollback() {
		Status = FileTransactionState.RollingBack;
		SaveHeader();
		foreach (var file in _enlistedFiles.Values) {
			file.Buffer.Rollback();
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
		this.UncommittedPageFileDirectory = surrogate.UncomittedPageFileDirectory;
		foreach (var enlistedSurrogate in surrogate.EnlistedFiles) {
			EnlistFile(enlistedSurrogate.Filename, enlistedSurrogate.PageSize, enlistedSurrogate.MaxMemory);
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

	#region Internal Classes

	private record FileDescriptor {
		public TransactionalFileMappedBuffer Buffer { get; init; }
		public bool ShouldDispose { get; init; }
	}

	#endregion

	#region Serializable Surrogates

	[Serializable]
	public class TransactionalFileSerializableSurrogate {
		public string Filename { get; set; }
		public long PageSize { get; set; }
		public long MaxMemory { get; set; }

		public TransactionalFileSerializableSurrogate() {
		}

		public TransactionalFileSerializableSurrogate(TransactionalFileMappedBuffer file) {
			Dehydrate(file, this);
		}

		public static void Dehydrate(TransactionalFileMappedBuffer @from, TransactionalFileSerializableSurrogate to) {
			to.Filename = from.FileDescriptor.Path;
			to.PageSize = from.PageSize;
			to.MaxMemory = from.MaxMemory;
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
			to.UncomittedPageFileDirectory = @from.UncommittedPageFileDirectory;
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
