// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Linq;
using NUnit.Framework;
using System.IO;
using System.Threading.Tasks;
using NUnit.Framework.Legacy;

namespace Hydrogen.Tests;

[TestFixture]
[Parallelizable(ParallelScope.Children)]
public class FileTransactionScopeTests {

	[Test]
	public void EnlistedFilesAggregateInNestedScopes() {
		var RNG = new Random(1231);
		var fileBaseDir = Tools.FileSystem.GetTempEmptyDirectory(true);
		var txnBaseDir = Tools.FileSystem.GetTempEmptyDirectory(true);

		var filePath1 = Path.Combine(fileBaseDir, "file1.dat");
		var filePath2 = Path.Combine(fileBaseDir, "file2.dat");
		var filePath3 = Path.Combine(fileBaseDir, "file3.dat");
		File.WriteAllBytes(filePath1, RNG.NextBytes(111));
		File.WriteAllBytes(filePath2, RNG.NextBytes(111));
		File.WriteAllBytes(filePath3, RNG.NextBytes(111));
		using (Tools.Scope.ExecuteOnDispose(() => Tools.FileSystem.DeleteDirectories(fileBaseDir, txnBaseDir))) {
			using (var scope1 = new FileTransactionScope(txnBaseDir)) {
				scope1.BeginTransaction();
				scope1.EnlistFile(filePath1, 100, 1 * 100);
				using (var scope2 = new FileTransactionScope(txnBaseDir)) {
					scope2.BeginTransaction();
					scope2.EnlistFile(filePath2, 100, 1 * 100);
					using (var scope3 = new FileTransactionScope(txnBaseDir)) {
						scope3.BeginTransaction();
						scope3.EnlistFile(filePath3, 100, 1 * 100);

						// Ensures that scope3 refers to 3 enlisted files
						ClassicAssert.AreEqual(filePath1, scope3.Transaction.EnlistedFiles[0].FileDescriptor.CaseCorrectPath);
						ClassicAssert.AreEqual(filePath2, scope3.Transaction.EnlistedFiles[1].FileDescriptor.CaseCorrectPath);
						ClassicAssert.AreEqual(filePath3, scope3.Transaction.EnlistedFiles[2].FileDescriptor.CaseCorrectPath);
					}
					// Ensures that scope3 refers to 3 enlisted files
					ClassicAssert.AreEqual(filePath1, scope2.Transaction.EnlistedFiles[0].FileDescriptor.CaseCorrectPath);
					ClassicAssert.AreEqual(filePath2, scope2.Transaction.EnlistedFiles[1].FileDescriptor.CaseCorrectPath);
					ClassicAssert.AreEqual(filePath3, scope2.Transaction.EnlistedFiles[2].FileDescriptor.CaseCorrectPath);
				}
				// Ensures that scope3 refers to 3 enlisted files
				ClassicAssert.AreEqual(filePath1, scope1.Transaction.EnlistedFiles[0].FileDescriptor.CaseCorrectPath);
				ClassicAssert.AreEqual(filePath2, scope1.Transaction.EnlistedFiles[1].FileDescriptor.CaseCorrectPath);
				ClassicAssert.AreEqual(filePath3, scope1.Transaction.EnlistedFiles[2].FileDescriptor.CaseCorrectPath);
			}
		}
	}

	[Test]
	public void CannotEnlistFileWithDifferentPageDir() {
		var RNG = new Random(1231);
		var fileBaseDir = Tools.FileSystem.GetTempEmptyDirectory(true);
		var txnBaseDir = Tools.FileSystem.GetTempEmptyDirectory(true);
		var filePageDir = Tools.FileSystem.GetTempEmptyDirectory(true);

		var filePath = Path.Combine(fileBaseDir, "file.dat");
		File.WriteAllBytes(filePath, RNG.NextBytes(111));
		using (Tools.Scope.ExecuteOnDispose(() => Tools.FileSystem.DeleteDirectories(fileBaseDir, txnBaseDir, filePageDir))) {
			using (var scope = new FileTransactionScope(txnBaseDir)) {
				scope.BeginTransaction();
				var file = new TransactionalFileMappedBuffer(
					TransactionalFileDescriptor.From(
						filePath,
						filePageDir,
						100,
						1 * 100
					)
				); // note: filePageDir != txnBaseDir
				Assert.Throws<ArgumentException>(() => scope.EnlistFile(file, true));
			}
		}
	}


	[Test]
	public void CanEnlistFileWithSamePageDir() {
		var RNG = new Random(1231);
		var fileBaseDir = Tools.FileSystem.GetTempEmptyDirectory(true);
		var txnBaseDir = Tools.FileSystem.GetTempEmptyDirectory(true);
		var filePageDir = txnBaseDir;

		var filePath = Path.Combine(fileBaseDir, "file.dat");
		File.WriteAllBytes(filePath, RNG.NextBytes(111));
		using (Tools.Scope.ExecuteOnDispose(() => Tools.FileSystem.DeleteDirectories(fileBaseDir, txnBaseDir))) {
			using (var scope = new FileTransactionScope(txnBaseDir)) {
				scope.BeginTransaction();
				var file = new TransactionalFileMappedBuffer(
					TransactionalFileDescriptor.From(
						filePath,
						filePageDir,
						100,
						1 * 100
					) // note: filePageDir == txnBaseDir
				);

				Assert.DoesNotThrow(() => scope.EnlistFile(file, true));
			}
		}
	}


	[Test]
	public void Nested_CommitCommit() {
		var RNG = new Random(1231);
		var fileBaseDir = Tools.FileSystem.GetTempEmptyDirectory(true);
		var txnBaseDir = Tools.FileSystem.GetTempEmptyDirectory(true);

		var original = RNG.NextBytes(111);

		var filePath = Path.Combine(fileBaseDir, "file.dat");
		File.WriteAllBytes(filePath, original);
		string txnFile;
		using (Tools.Scope.ExecuteOnDispose(() => Tools.FileSystem.DeleteDirectories(fileBaseDir, txnBaseDir))) {
			var chunk1 = new byte[] { 1, 2, 3 };
			var chunk2 = new byte[] { 4, 5, 6 };
			var chunk3 = new byte[] { 7, 8, 9 };
			using (var scope1 = new FileTransactionScope(txnBaseDir)) {
				txnFile = scope1.TransactionFile;
				scope1.BeginTransaction();
				var file = scope1.EnlistFile(filePath, 100, 1 * 100) as TransactionalFileMappedBuffer;
				file.AddRange(chunk1);

				using (var scope2 = new FileTransactionScope(txnBaseDir)) {
					// child scope should get absorb parent transaction automatically
					ClassicAssert.AreEqual(scope1.Transaction, scope2.Transaction);
					ClassicAssert.AreEqual(scope1.TransactionFile, scope2.TransactionFile);

					// Should have enlisted file
					ClassicAssert.AreEqual(file, scope2.Transaction.EnlistedFiles[0]);
					scope2.Transaction.EnlistedFiles[0].AddRange(chunk2);

					// child commit doesn't commit
					scope2.Commit();

				}

				// Add a third chunk
				file.AddRange(chunk3);

				scope1.Commit();
			}
			ClassicAssert.AreEqual(original.Concat(chunk1).Concat(chunk2).Concat(chunk3), File.ReadAllBytes(filePath));
		}
		// check no transaction files
		ClassicAssert.IsTrue(!File.Exists(txnFile));
	}

	[Test]
	public void Nested_CommitRollback() {
		var RNG = new Random(1231);
		var fileBaseDir = Tools.FileSystem.GetTempEmptyDirectory(true);
		var txnBaseDir = Tools.FileSystem.GetTempEmptyDirectory(true);
		var original = RNG.NextBytes(111);

		var filePath = Path.Combine(fileBaseDir, "file.dat");
		File.WriteAllBytes(filePath, original);

		using (Tools.Scope.ExecuteOnDispose(() => Tools.FileSystem.DeleteDirectories(fileBaseDir, txnBaseDir))) {
			var chunk1 = new byte[] { 1, 2, 3 };
			var chunk2 = new byte[] { 4, 5, 6 };
			var chunk3 = new byte[] { 7, 8, 9 };
			string txnFile;
			using (var scope1 = new FileTransactionScope(txnBaseDir)) {
				txnFile = scope1.TransactionFile;
				scope1.BeginTransaction();
				var file = scope1.EnlistFile(filePath, 100, 1 * 100) as TransactionalFileMappedBuffer;
				file.AddRange(chunk1);

				using (var scope2 = new FileTransactionScope(txnBaseDir)) {
					// child scope should get absorb parent transaction automatically
					ClassicAssert.AreEqual(scope1.Transaction, scope2.Transaction);

					// Should have enlisted file
					ClassicAssert.AreEqual(file, scope2.Transaction.EnlistedFiles[0]);
					scope2.Transaction.EnlistedFiles[0].AddRange(chunk2);

					// child commit doesn't commit
					scope2.Rollback();

				}

				// Add a third chunk
				file.AddRange(chunk3);

				scope1.Commit();
			}
			ClassicAssert.AreEqual(original, File.ReadAllBytes(filePath));

			// check no transaction files
			ClassicAssert.IsTrue(!File.Exists(txnFile));
		}
	}

	[Test]
	public void Nested_RollbackCommit() {
		var RNG = new Random(1231);
		var fileBaseDir = Tools.FileSystem.GetTempEmptyDirectory(true);
		var txnBaseDir = Tools.FileSystem.GetTempEmptyDirectory(true);

		var original = RNG.NextBytes(111);

		var filePath = Path.Combine(fileBaseDir, "file.dat");
		File.WriteAllBytes(filePath, original);

		using (Tools.Scope.ExecuteOnDispose(() => Tools.FileSystem.DeleteDirectories(fileBaseDir, txnBaseDir))) {
			var chunk1 = new byte[] { 1, 2, 3 };
			var chunk2 = new byte[] { 4, 5, 6 };
			var chunk3 = new byte[] { 7, 8, 9 };
			string txnFile;
			using (var scope1 = new FileTransactionScope(txnBaseDir)) {
				txnFile = scope1.TransactionFile;
				scope1.BeginTransaction();
				var file = scope1.EnlistFile(filePath, 100, 1 * 100) as TransactionalFileMappedBuffer;
				file.AddRange(chunk1);

				using (var scope2 = new FileTransactionScope(txnBaseDir)) {
					// child scope should get absorb parent transaction automatically
					ClassicAssert.AreEqual(scope1.Transaction, scope2.Transaction);

					// Should have enlisted file
					ClassicAssert.AreEqual(file, scope2.Transaction.EnlistedFiles[0]);
					scope2.Transaction.EnlistedFiles[0].AddRange(chunk2);

					// child commit doesn't commit
					scope2.Commit();
					// check no transaction files
					ClassicAssert.IsTrue(File.Exists(txnFile));
				}

				// Add a third chunk
				file.AddRange(chunk3);

				scope1.Rollback();
			}
			ClassicAssert.AreEqual(original, File.ReadAllBytes(filePath));

			// check no transaction files
			ClassicAssert.IsTrue(!File.Exists(txnFile));

		}
	}

	[Test]
	public void Nested_RollbackRollback() {
		var RNG = new Random(1231);
		var fileBaseDir = Tools.FileSystem.GetTempEmptyDirectory(true);
		var txnBaseDir = Tools.FileSystem.GetTempEmptyDirectory(true);

		var original = RNG.NextBytes(111);

		var filePath = Path.Combine(fileBaseDir, "file.dat");
		File.WriteAllBytes(filePath, original);

		using (Tools.Scope.ExecuteOnDispose(() => Tools.FileSystem.DeleteDirectories(fileBaseDir, txnBaseDir))) {
			var chunk1 = new byte[] { 1, 2, 3 };
			var chunk2 = new byte[] { 4, 5, 6 };
			var chunk3 = new byte[] { 7, 8, 9 };
			string txnFile;
			using (var scope1 = new FileTransactionScope(txnBaseDir)) {
				txnFile = scope1.TransactionFile;
				scope1.BeginTransaction();
				var file = scope1.EnlistFile(filePath, 100, 1 * 100) as TransactionalFileMappedBuffer;
				file.AddRange(chunk1);

				using (var scope2 = new FileTransactionScope(txnBaseDir)) {
					// child scope should get absorb parent transaction automatically
					ClassicAssert.AreEqual(scope1.Transaction, scope2.Transaction);

					// Should have enlisted file
					ClassicAssert.AreEqual(file, scope2.Transaction.EnlistedFiles[0]);
					scope2.Transaction.EnlistedFiles[0].AddRange(chunk2);

					// child commit doesn't commit
					scope2.Rollback();
				}

				// Add a third chunk
				file.AddRange(chunk3);

				scope1.Rollback();
			}
			ClassicAssert.AreEqual(original, File.ReadAllBytes(filePath));

			// check no transaction files
			ClassicAssert.IsTrue(!File.Exists(txnFile));
		}
	}

	[Test]
	public void CannotDirectCommitEnlistedFile() {
		var RNG = new Random(1231);
		var fileBaseDir = Tools.FileSystem.GetTempEmptyDirectory(true);
		var txnBaseDir = Tools.FileSystem.GetTempEmptyDirectory(true);

		var filePath = Path.Combine(fileBaseDir, "file.dat");
		File.WriteAllBytes(filePath, RNG.NextBytes(512));

		using (Tools.Scope.ExecuteOnDispose(() => Tools.FileSystem.DeleteDirectories(fileBaseDir, txnBaseDir))) {
			using (var scope1 = new FileTransactionScope(txnBaseDir)) {
				scope1.BeginTransaction();
				var file = scope1.EnlistFile(filePath, 100, 1 * 100) as TransactionalFileMappedBuffer;
				Assert.Throws<InvalidOperationException>(() => file.Commit());
				scope1.Rollback();
			}
		}
	}

	[Test]
	public void CanCommitUnenlistedFile() {
		var RNG = new Random(1231);
		var fileBaseDir = Tools.FileSystem.GetTempEmptyDirectory(true);
		var txnBaseDir = Tools.FileSystem.GetTempEmptyDirectory(true);

		var filePath = Path.Combine(fileBaseDir, "file.dat");
		File.WriteAllBytes(filePath, RNG.NextBytes(512));

		using (Tools.Scope.ExecuteOnDispose(() => Tools.FileSystem.DeleteDirectories(fileBaseDir, txnBaseDir))) {
			using (var scope1 = new FileTransactionScope(txnBaseDir)) {
				scope1.BeginTransaction();
				var file = new TransactionalFileMappedBuffer(TransactionalFileDescriptor.From( filePath, txnBaseDir, 100, 1 * 100)); // note: not enlisted in scope
				if (file.RequiresLoad)
					file.Load();

				Assert.DoesNotThrow(() => file.Commit());
				scope1.Rollback();
			}
		}
	}

	[Test]
	public void CannotDirectRollbackEnlistedFile() {
		var RNG = new Random(1231);
		var fileBaseDir = Tools.FileSystem.GetTempEmptyDirectory(true);
		var txnBaseDir = Tools.FileSystem.GetTempEmptyDirectory(true);

		var filePath = Path.Combine(fileBaseDir, "file.dat");
		File.WriteAllBytes(filePath, RNG.NextBytes(512));

		using (Tools.Scope.ExecuteOnDispose(() => Tools.FileSystem.DeleteDirectories(fileBaseDir, txnBaseDir))) {
			using (var scope1 = new FileTransactionScope(txnBaseDir)) {
				scope1.BeginTransaction();
				var file = scope1.EnlistFile(filePath, 100, 1 * 100) as TransactionalFileMappedBuffer;
				Assert.Throws<InvalidOperationException>(() => file.Rollback());
				scope1.Rollback();
			}
		}
	}

	[Test]
	public void CanRollbackUnenlistedFile() {
		var RNG = new Random(1231);
		var fileBaseDir = Tools.FileSystem.GetTempEmptyDirectory(true);
		var txnBaseDir = Tools.FileSystem.GetTempEmptyDirectory(true);
		var original = RNG.NextBytes(111);

		var filePath = Path.Combine(fileBaseDir, "file.dat");
		File.WriteAllBytes(filePath, original);

		using (Tools.Scope.ExecuteOnDispose(() => Tools.FileSystem.DeleteDirectories(fileBaseDir, txnBaseDir))) {
			using (var scope1 = new FileTransactionScope(txnBaseDir)) {
				scope1.BeginTransaction();
				// note: file is not enlisted
				var file = new TransactionalFileMappedBuffer(TransactionalFileDescriptor.From(filePath, txnBaseDir, 100, 1 * 100)); // note: not enlisted in scope
				if (file.RequiresLoad)
					file.Load();
				Assert.DoesNotThrow(() => file.Rollback());
				scope1.Rollback();
			}
		}
	}


	[Test]
	public void TestSimltaneousFileTransactions() {
		var RNG = new Random(1231);
		var fileBaseDir = Tools.FileSystem.GetTempEmptyDirectory(true);
		var txnBaseDir = Tools.FileSystem.GetTempEmptyDirectory(true);

		var filePath1 = Path.Combine(fileBaseDir, "file1.dat");
		var filePath2 = Path.Combine(fileBaseDir, "file2.dat");
		var filePath3 = Path.Combine(fileBaseDir, "file3.dat");
		var filePath4 = Path.Combine(fileBaseDir, "file4.dat");

		File.WriteAllBytes(filePath1, RNG.NextBytes(512));
		File.WriteAllBytes(filePath2, RNG.NextBytes(512));
		File.WriteAllBytes(filePath3, RNG.NextBytes(512));
		File.WriteAllBytes(filePath4, RNG.NextBytes(512));

		using (Tools.Scope.ExecuteOnDispose(() => Tools.FileSystem.DeleteDirectories(fileBaseDir, txnBaseDir))) {
			var task1 = Task.Run(() => LoopTransactions(filePath1, 100));
			var task2 = Task.Run(() => LoopTransactions(filePath2, 100));
			var task3 = Task.Run(() => LoopTransactions(filePath3, 100));
			var task4 = Task.Run(() => LoopTransactions(filePath4, 100));
			Task.WaitAll(task1, task2, task3, task4);
		}

		void LoopTransactions(string filepath, int loops) {
			for (var i = 0; i < loops; i++) {
				using (var scope = new FileTransactionScope(txnBaseDir, ContextScopePolicy.MustBeRoot)) {
					scope.BeginTransaction();
					var file = scope.EnlistFile(filepath, 100, 1 * 100);
					file.AsBuffer.AddRange(RNG.NextBytes(100));
					scope.Commit();
				}
			}
		}

	}
}
