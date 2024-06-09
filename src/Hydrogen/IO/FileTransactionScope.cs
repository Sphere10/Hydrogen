// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT NON-AI software license, see the accompanying file
// LICENSE or visit https://sphere10.com/legal/NON-AI-MIT.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;

namespace Hydrogen;

public class FileTransactionScope : SyncTransactionalScope<FileTransaction> {
	private const string ContextIDPrefix = "FileTransactionContext:71C280A0-7DEA-41C0-BCE6-CC34DD99BD64";

	public FileTransactionScope(string baseDir, ContextScopePolicy policy = ContextScopePolicy.None)
		: this(GenTransactionFileName(), baseDir, policy) {
	}

	private FileTransactionScope(string transactionFile, string uncommittedPageDir, ContextScopePolicy policy)
		: base(policy, ContextIDPrefix) {
		TransactionFile = this.IsRootScope ? transactionFile : RootScope.TransactionFile;
		PagePath = uncommittedPageDir;
	}

	public string TransactionFile { get; }

	public string PagePath { get; }

	public new FileTransactionScope RootScope => (FileTransactionScope)base.RootScope;

	protected override FileTransaction BeginTransactionInternal() => new(TransactionFile, PagePath);

	protected override void CloseTransactionInternal(FileTransaction transaction) => transaction.Dispose();

	protected override void CommitInternal(FileTransaction transaction) => transaction.Commit();

	protected override void RollbackInternal(FileTransaction transaction) => transaction.Rollback();

	public ITransactionalFile EnlistFile(string filename, int pageSize, long maxMemory) {
		CheckTransactionExists();
		return Transaction.EnlistFile(filename, pageSize, maxMemory);
	}

	public ITransactionalFile EnlistFile(ITransactionalFile file, bool ownsFile) {
		CheckTransactionExists();
		Guard.Argument(
			Tools.FileSystem.GetCaseCorrectFilePath(file.AsBuffer.PageMarkerRepo.BaseDir) == Tools.FileSystem.GetCaseCorrectFilePath(Transaction.UncommittedPageFileDirectory),
			nameof(file),
			"Enlisted file's transactional page directory did not match transactions page directory"
		);
		return Transaction.EnlistFile(file, ownsFile);
	}

	public void DelistFile(string filename) {
		CheckTransactionExists();
		Transaction.DelistFile(filename);
	}

	public void DelistFile(TransactionalFileMappedBuffer file) {
		CheckTransactionExists();
		Transaction.DelistFile(file);
	}

	public static FileTransactionScope GetCurrent() {
		//return ContextScope<FileTransactionScope>.GetCurrent(ToContextName(transactionFile));
		return FileTransactionScope.GetCurrent(ContextIDPrefix);
	}

	public static void ProcessDanglingTransactions(string baseDir) {
		// this will load up all dangling transaction found in a directory, and complete them
		// TODO

	}

	protected new static FileTransactionScope GetCurrent(string contextID)
		=> ActionTransactionalScope<FileTransaction>.GetCurrent(contextID) as FileTransactionScope;

	private static string GenTransactionFileName() => $"FTXN_{Guid.NewGuid().ToStrictAlphaString()}.txn";
}
