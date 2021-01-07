//-----------------------------------------------------------------------
// <copyright file="LargeCollectionTests.cs" company="Sphere 10 Software">
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
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using NUnit.Framework;
using System.IO;
using NUnit.Framework.Constraints;
using Sphere10.Framework;
using Sphere10.Framework.FastReflection;

namespace Sphere10.Framework.Tests {

	[TestFixture]
	[Parallelizable(ParallelScope.Children)]
	public class FileTransactionScopeTests {

		[Test]
		public void Nested_CommitCommit() {
			var RNG = new Random(1231);
			var fileBaseDir = Tools.FileSystem.GetTempEmptyDirectory(true);
			var txnBaseDir = Tools.FileSystem.GetTempEmptyDirectory(true);
			var txnFile = Path.Combine(txnBaseDir, "Test.txn");

			var original = RNG.NextBytes(111);

			var filePath = Path.Combine(fileBaseDir, "file.dat");
			File.WriteAllBytes(filePath, original);

			using (Tools.Scope.ExecuteOnDispose(() => Tools.FileSystem.DeleteDirectories(fileBaseDir, txnBaseDir))) {
				var chunk1 = new byte[] { 1, 2, 3 };
				var chunk2 = new byte[] { 4, 5, 6 };
				var chunk3 = new byte[] { 7, 8, 9 };
				using (var scope1 = new FileTransactionScope(txnFile)) {
					scope1.BeginTransaction();
					var file = scope1.EnlistFile(filePath, 100, 1);
					file.AddRange(chunk1);

					using (var scope2 = new FileTransactionScope(txnFile)) {
						// child scope should get absorb parent transaction automatically
						Assert.AreEqual(scope1.Transaction, scope2.Transaction);

						// Should have enlisted file
						Assert.AreEqual(file, scope2.Transaction.EnlistedFiles[0]);
						scope2.Transaction.EnlistedFiles[0].AddRange(chunk2);

						// child commit doesn't commit
						scope2.Commit();
						// check no transaction files
						Assert.IsTrue(File.Exists(txnFile));
					}

					// Add a third chunk
					file.AddRange(chunk3);

					scope1.Commit();
				}
				Assert.AreEqual(original.Concat(chunk1).Concat(chunk2).Concat(chunk3), File.ReadAllBytes(filePath));
			}
		}

		[Test]
		public void Nested_CommitRollback() {
			var RNG = new Random(1231);
			var fileBaseDir = Tools.FileSystem.GetTempEmptyDirectory(true);
			var txnBaseDir = Tools.FileSystem.GetTempEmptyDirectory(true);
			var txnFile = Path.Combine(txnBaseDir, "Test.txn");

			var original = RNG.NextBytes(111);

			var filePath = Path.Combine(fileBaseDir, "file.dat");
			File.WriteAllBytes(filePath, original);

			using (Tools.Scope.ExecuteOnDispose(() => Tools.FileSystem.DeleteDirectories(fileBaseDir, txnBaseDir))) {
				var chunk1 = new byte[] { 1, 2, 3 };
				var chunk2 = new byte[] { 4, 5, 6 };
				var chunk3 = new byte[] { 7, 8, 9 };
				using (var scope1 = new FileTransactionScope(txnFile)) {
					scope1.BeginTransaction();
					var file = scope1.EnlistFile(filePath, 100, 1);
					file.AddRange(chunk1);

					using (var scope2 = new FileTransactionScope(txnFile)) {
						// child scope should get absorb parent transaction automatically
						Assert.AreEqual(scope1.Transaction, scope2.Transaction);

						// Should have enlisted file
						Assert.AreEqual(file, scope2.Transaction.EnlistedFiles[0]);
						scope2.Transaction.EnlistedFiles[0].AddRange(chunk2);

						// child commit doesn't commit
						scope2.Rollback();
						// check no transaction files
						Assert.IsTrue(File.Exists(txnFile));
					}

					// Add a third chunk
					file.AddRange(chunk3);

					scope1.Commit();
				}
				Assert.AreEqual(original, File.ReadAllBytes(filePath));
			}
		}


		[Test]
		public void Nested_RollbackCommit() {
			var RNG = new Random(1231);
			var fileBaseDir = Tools.FileSystem.GetTempEmptyDirectory(true);
			var txnBaseDir = Tools.FileSystem.GetTempEmptyDirectory(true);
			var txnFile = Path.Combine(txnBaseDir, "Test.txn");

			var original = RNG.NextBytes(111);

			var filePath = Path.Combine(fileBaseDir, "file.dat");
			File.WriteAllBytes(filePath, original);

			using (Tools.Scope.ExecuteOnDispose(() => Tools.FileSystem.DeleteDirectories(fileBaseDir, txnBaseDir))) {
				var chunk1 = new byte[] { 1, 2, 3 };
				var chunk2 = new byte[] { 4, 5, 6 };
				var chunk3 = new byte[] { 7, 8, 9 };
				using (var scope1 = new FileTransactionScope(txnFile)) {
					scope1.BeginTransaction();
					var file = scope1.EnlistFile(filePath, 100, 1);
					file.AddRange(chunk1);

					using (var scope2 = new FileTransactionScope(txnFile)) {
						// child scope should get absorb parent transaction automatically
						Assert.AreEqual(scope1.Transaction, scope2.Transaction);

						// Should have enlisted file
						Assert.AreEqual(file, scope2.Transaction.EnlistedFiles[0]);
						scope2.Transaction.EnlistedFiles[0].AddRange(chunk2);

						// child commit doesn't commit
						scope2.Commit();
						// check no transaction files
						Assert.IsTrue(File.Exists(txnFile));
					}

					// Add a third chunk
					file.AddRange(chunk3);

					scope1.Rollback();
				}
				Assert.AreEqual(original, File.ReadAllBytes(filePath));
			}
		}

		[Test]
		public void Nested_RollbackRollback() {
			var RNG = new Random(1231);
			var fileBaseDir = Tools.FileSystem.GetTempEmptyDirectory(true);
			var txnBaseDir = Tools.FileSystem.GetTempEmptyDirectory(true);
			var txnFile = Path.Combine(txnBaseDir, "Test.txn");

			var original = RNG.NextBytes(111);

			var filePath = Path.Combine(fileBaseDir, "file.dat");
			File.WriteAllBytes(filePath, original);

			using (Tools.Scope.ExecuteOnDispose(() => Tools.FileSystem.DeleteDirectories(fileBaseDir, txnBaseDir))) {
				var chunk1 = new byte[] { 1, 2, 3 };
				var chunk2 = new byte[] { 4, 5, 6 };
				var chunk3 = new byte[] { 7, 8, 9 };
				using (var scope1 = new FileTransactionScope(txnFile)) {
					scope1.BeginTransaction();
					var file = scope1.EnlistFile(filePath, 100, 1);
					file.AddRange(chunk1);

					using (var scope2 = new FileTransactionScope(txnFile)) {
						// child scope should get absorb parent transaction automatically
						Assert.AreEqual(scope1.Transaction, scope2.Transaction);

						// Should have enlisted file
						Assert.AreEqual(file, scope2.Transaction.EnlistedFiles[0]);
						scope2.Transaction.EnlistedFiles[0].AddRange(chunk2);

						// child commit doesn't commit
						scope2.Rollback();
						// check no transaction files
						Assert.IsTrue(File.Exists(txnFile));
					}

					// Add a third chunk
					file.AddRange(chunk3);

					scope1.Rollback();
				}
				Assert.AreEqual(original, File.ReadAllBytes(filePath));
			}
		}

	}
}
