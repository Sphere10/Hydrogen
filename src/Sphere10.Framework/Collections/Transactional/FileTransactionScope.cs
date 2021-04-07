using System;
using System.IO;

namespace Sphere10.Framework {

	public class FileTransactionScope : TransactionalScope<FileTransactionScope, FileTransaction> {
		private const string ContextIDPrefix = "FileTransactionContext:";

		public FileTransactionScope(string baseDir, bool removeme, ScopeContextPolicy policy = ScopeContextPolicy.None) 
		   : this(GenTransactionFileName(), baseDir, policy) {
			
		}

		public FileTransactionScope(string transactionFile, ScopeContextPolicy policy = ScopeContextPolicy.None)
			: this(transactionFile, Path.GetDirectoryName(transactionFile), policy) {
		}

		public FileTransactionScope(string transactionFile, string uncomittedPageDir, ScopeContextPolicy policy = ScopeContextPolicy.None)
			: base(policy,
				  ToContextName(transactionFile),
				  (scope) => new FileTransaction(transactionFile, uncomittedPageDir),
				  (scope, txn) => txn.Commit(),
				  (scope, txn) => txn.Rollback(),
				  (scope, txn) => txn.Dispose(),
				  TransactionAction.Rollback) {
		}

		public ITransactionalFile EnlistFile(string filename, int pageSize, int maxOpenPages) {
			CheckTransactionExists();
			return Transaction.EnlistFile(filename, pageSize, maxOpenPages);
		}

		public ITransactionalFile EnlistFile(string filename, Guid fileID, int pageSize, int maxOpenPages) {
			CheckTransactionExists();
			return Transaction.EnlistFile(filename, fileID, pageSize, maxOpenPages);
		}

		public ITransactionalFile EnlistFile(ITransactionalFile file) {
			Guard.Argument(
				Tools.FileSystem.GetCaseCorrectFilePath(file.AsBuffer.PageMarkerRepo.BaseDir) == Tools.FileSystem.GetCaseCorrectFilePath(Transaction.UncomittedPageFileDirectory), 
				nameof(file),
				"Enlisted file's transactional page directory did not match transactions page directory"
			);
			CheckTransactionExists();
			return Transaction.EnlistFile(file);
		}

		public void DelistFile(string filename) {
			CheckTransactionExists();
			Transaction.DelistFile(filename);
		}

		public void DelistFile(TransactionalFileMappedBuffer file) {
			CheckTransactionExists();
			Transaction.DelistFile(file);
		}

		public new static FileTransactionScope GetCurrent(string transactionFile) {
			return ScopeContext<FileTransactionScope>.GetCurrent(ToContextName(transactionFile));
		}

		public static void ProcessDanglingTransactions(string baseDir) {
			// this will load up all dangling transaction found in a directory, and complete them
			// TODO

		}

		private static string ToContextName(string transactionFile) {
			return $"{ContextIDPrefix}{transactionFile}";
		}

		private static string GenTransactionFileName() => $"{Guid.NewGuid().ToStrictAlphaString()}.txn";
	}

}