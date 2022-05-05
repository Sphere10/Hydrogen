using System;

namespace Hydrogen {

	public class FileTransactionScope : TransactionalScope<FileTransactionScope, FileTransaction> {
		private const string ContextIDPrefix = "FileTransactionContext:71C280A0-7DEA-41C0-BCE6-CC34DD99BD64";

		public FileTransactionScope(string baseDir, ScopeContextPolicy policy = ScopeContextPolicy.None) 
		   : this(GenTransactionFileName(), baseDir, policy) {
		}

		private FileTransactionScope(string transactionFile, string uncommittedPageDir, ScopeContextPolicy policy)
			: base(policy,
				ContextIDPrefix,
				  (scope) => new FileTransaction(transactionFile, uncommittedPageDir),
				  (scope, txn) => txn.Commit(),
				  (scope, txn) => txn.Rollback(),
				  (scope, txn) => txn.Dispose(),
				  TransactionAction.Rollback) {
			TransactionFile = this.IsRootScope ? transactionFile : RootScope.TransactionFile;
		}

		public string TransactionFile { get; }

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
			//return ScopeContext<FileTransactionScope>.GetCurrent(ToContextName(transactionFile));
			return ScopeContext<FileTransactionScope>.GetCurrent(ContextIDPrefix);
		}

		public static void ProcessDanglingTransactions(string baseDir) {
			// this will load up all dangling transaction found in a directory, and complete them
			// TODO
			
		}

		//private static string ToContextName(Guid guid) => $"{ContextIDPrefix}{guid.ToStrictAlphaString()}";

		private static string GenTransactionFileName() => $"FTXN_{Guid.NewGuid().ToStrictAlphaString()}.txn";
	}

}