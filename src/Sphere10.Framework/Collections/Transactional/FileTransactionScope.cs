using System;
using System.IO;

namespace Sphere10.Framework {

	public class FileTransactionScope : TransactionalScope<FileTransactionScope, FileTransaction> {
		private const string ContextIDPrefix = "FileTransactionContext:";

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
				  TransactionAction.Rollback				  ) {
		}

		public TransactionalFileMappedBuffer EnlistFile(string filename, int pageSize, int maxOpenPages) {
			CheckTransactionExists();
			return Transaction.EnlistFile(filename, pageSize, maxOpenPages);
		}

		public TransactionalFileMappedBuffer EnlistFile(string filename, Guid fileID, int pageSize, int maxOpenPages) {
			CheckTransactionExists();
			return Transaction.EnlistFile(filename, fileID, pageSize, maxOpenPages);
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

		private static string ToContextName(string transactionFile) {
			return $"{ContextIDPrefix}{transactionFile}";
		}
	}

}