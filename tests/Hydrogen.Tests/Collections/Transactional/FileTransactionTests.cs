// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using System.IO;
using NUnit.Framework.Legacy;

namespace Hydrogen.Tests;

[TestFixture]
[Parallelizable(ParallelScope.Children)]
public class FileTransactionTests {

	[Test]
	public void Sequential_CommitCommit() {
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
			using (var transaction = new FileTransaction(txnFile)) {
				var file = transaction.EnlistFile(filePath, 100, 1 * 100);

				file.AddRange(chunk1);
				transaction.Commit();
				// check no transaction files
				ClassicAssert.AreEqual(0, Tools.FileSystem.GetFiles(txnBaseDir).Count());
				file.AddRange(chunk2);
				transaction.Commit();
			}
			ClassicAssert.AreEqual(original.Concat(chunk1).Concat(chunk2), File.ReadAllBytes(filePath));
		}
	}

	[Test]
	public void Sequential_CommitRollback() {
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
			using (var transaction = new FileTransaction(txnFile)) {
				var file = transaction.EnlistFile(filePath, 100, 1 * 100);

				file.AddRange(chunk1);
				transaction.Commit();
				// check no transaction files
				ClassicAssert.AreEqual(0, Tools.FileSystem.GetFiles(txnBaseDir).Count());
				file.AddRange(chunk2);
				transaction.Rollback();
			}
			ClassicAssert.AreEqual(original.Concat(chunk1), File.ReadAllBytes(filePath));
		}
	}


	[Test]
	public void Sequential_RollbackCommit() {
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
			using (var transaction = new FileTransaction(txnFile)) {
				var file = transaction.EnlistFile(filePath, 100, 1 * 100);

				file.AddRange(chunk1);
				transaction.Rollback();
				// check no transaction files
				ClassicAssert.AreEqual(0, Tools.FileSystem.GetFiles(txnBaseDir).Count());
				file.AddRange(chunk2);
				transaction.Commit();
			}
			ClassicAssert.AreEqual(original.Concat(chunk2), File.ReadAllBytes(filePath));
		}
	}


	[Test]
	public void Sequential_RollbackRollback() {
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
			using (var transaction = new FileTransaction(txnFile)) {
				var file = transaction.EnlistFile(filePath, 100, 1 * 100);

				file.AddRange(chunk1);
				transaction.Rollback();
				// check no transaction files
				ClassicAssert.AreEqual(0, Tools.FileSystem.GetFiles(txnBaseDir).Count());
				file.AddRange(chunk2);
				transaction.Rollback();
			}
			ClassicAssert.AreEqual(original, File.ReadAllBytes(filePath));
		}
	}


	[Test]
	public void NoDuplicateEnlistment() {
		var RNG = new Random(1231);
		var fileBaseDir = Tools.FileSystem.GetTempEmptyDirectory(true);
		var txnBaseDir = Tools.FileSystem.GetTempEmptyDirectory(true);
		var txnFile = Path.Combine(txnBaseDir, "Test.txn");

		var file = Path.Combine(fileBaseDir, "file.dat");
		File.WriteAllBytes(file, RNG.NextBytes(111));

		using (Tools.Scope.ExecuteOnDispose(() => Tools.FileSystem.DeleteDirectories(fileBaseDir, txnBaseDir))) {
			using (var transaction = new FileTransaction(txnFile)) {
				transaction.EnlistFile(file, 100, 1 * 100);
				Assert.Throws<InvalidOperationException>(() => transaction.EnlistFile(file, 10, 11));
			}
		}
	}

	[Test]
	public void NoDuplicateGlobalEnlistment() {
		var RNG = new Random(1231);
		var fileBaseDir = Tools.FileSystem.GetTempEmptyDirectory(true);
		var txnBaseDir = Tools.FileSystem.GetTempEmptyDirectory(true);
		var txnFile1 = Path.Combine(txnBaseDir, "Test1.txn");
		var txnFile2 = Path.Combine(txnBaseDir, "Test2.txn");

		var file = Path.Combine(fileBaseDir, "file.dat");
		File.WriteAllBytes(file, RNG.NextBytes(111));

		using (Tools.Scope.ExecuteOnDispose(() => Tools.FileSystem.DeleteDirectories(fileBaseDir, txnBaseDir))) {
			using (var transaction1 = new FileTransaction(txnFile1))
			using (var transaction2 = new FileTransaction(txnFile2)) {
				transaction1.EnlistFile(file, 100, 1 * 100);
				Assert.Throws<InvalidOperationException>(() => transaction2.EnlistFile(file, 10, 11));
			}
		}
	}

	[Test]
	[Sequential]
	public void Integration_Commit([Values(1, 2, 7, 11)] int numFiles) {
		var expected = new List<List<byte>>();
		var files = new List<TransactionalFileMappedBuffer>();
		var RNG = new Random(1231);

		var fileBaseDir = Tools.FileSystem.GetTempEmptyDirectory(true);
		var txnBaseDir = Tools.FileSystem.GetTempEmptyDirectory(true);
		var txnFile = Path.Combine(txnBaseDir, "Test.txn");

		using (Tools.Scope.ExecuteOnDispose(() => Tools.FileSystem.DeleteDirectories(fileBaseDir, txnBaseDir))) {
			using (var transaction = new FileTransaction(txnFile, txnBaseDir)) {
				// create arrays
				expected = new List<List<byte>>();
				for (var i = 0; i < numFiles; i++) {
					var filename = Path.Combine(fileBaseDir, $"File{i}.dat");
					var pageSize = i * i + 1;
					var maxOpenPages = i / 2 + 1;
					var maxCapacity = pageSize * (i + 1);

					// every odd file is new, even is pre-created
					expected.Add(new List<byte>());
					if (i % 2 == 0) {
						var startBytes = RNG.NextBytes(maxCapacity / 2);
						File.WriteAllBytes(filename, startBytes);
						expected[i].AddRange(startBytes);
					}
					files.Add(transaction.EnlistFile(filename, pageSize, maxOpenPages * pageSize));
				}

				// Mutate all the files 100 times, commit every 10 times
				for (var j = 0; j < 100; j++) {
					for (var i = 0; i < numFiles; i++) {
						var pageSize = i * i + 1;
						var maxCapacity = pageSize * (i + 1);
						var file = files[i];
						var expectedArr = expected[i];
						MutateLists(expectedArr, file, maxCapacity, RNG);
					}
				}

				// final commit
				transaction.Commit();
			}

			// check no transaction junk
			ClassicAssert.AreEqual(0, Tools.FileSystem.GetFiles(txnBaseDir).Count());

			// check files match expected
			for (var i = 0; i < numFiles; i++) {
				var filename = Path.Combine(fileBaseDir, $"File{i}.dat");
				ClassicAssert.AreEqual(expected[i], File.ReadAllBytes(filename));
			}
		}
	}

	[Test]
	[Sequential]
	public void Integration_ResumeCommit([Values(1, 2, 7, 11)] int numFiles) {
		var expected = new List<List<byte>>();
		var files = new List<TransactionalFileMappedBuffer>();
		var RNG = new Random(1231);

		var fileBaseDir = Tools.FileSystem.GetTempEmptyDirectory(true);
		var txnBaseDir = Tools.FileSystem.GetTempEmptyDirectory(true);
		var tmpDir = Tools.FileSystem.GetTempEmptyDirectory(true);
		var txnFile = Path.Combine(txnBaseDir, "Test.txn");

		using (Tools.Scope.ExecuteOnDispose(() => Tools.FileSystem.DeleteDirectories(fileBaseDir, txnBaseDir, tmpDir))) {
			using (var transaction = new FileTransaction(txnFile, txnBaseDir)) {
				// create arrays
				expected = new List<List<byte>>();
				for (var i = 0; i < numFiles; i++) {
					var filename = Path.Combine(fileBaseDir, $"File{i}.dat");
					var pageSize = i * i + 1;
					var maxOpenPages = i / 2 + 1;
					var maxCapacity = pageSize * (i + 1);

					// every odd file is new, even is pre-created
					expected.Add(new List<byte>());
					if (i % 2 == 0) {
						var startBytes = RNG.NextBytes(maxCapacity / 2);
						File.WriteAllBytes(filename, startBytes);
						expected[i].AddRange(startBytes);
					}
					files.Add(transaction.EnlistFile(filename, pageSize, maxOpenPages * pageSize));
				}

				// Mutate all the files 100 times, commit every 10 times
				for (var j = 0; j < 100; j++) {
					for (var i = 0; i < numFiles; i++) {
						var pageSize = i * i + 1;
						var maxCapacity = pageSize * (i + 1);
						var file = files[i];
						var expectedArr = expected[i];
						MutateLists(expectedArr, file, maxCapacity, RNG);
					}
				}
				// Flush all in-mem updates to disk
				transaction.Flush();

				// Hack: Set transaction to committing state for simulating a power outtage
				Tools.Reflection.SetPropertyValue(transaction, "Status", FileTransactionState.Committing);
				Tools.Reflection.InvokeMethod(transaction, "SaveHeader");

				// Backup current "committing" txn state to tmp folder 
				Tools.FileSystem.CopyDirectory(txnBaseDir, tmpDir);

				// Rollback everything 
				Tools.Reflection.SetPropertyValue(transaction, "Status", FileTransactionState.HasChanges);
				Tools.Reflection.InvokeMethod(transaction, "SaveHeader");
				transaction.Rollback();
			}
			// Now we restore the backed up "comitting" folder to
			// simulate a power outage in middle of commit.
			Tools.FileSystem.CopyDirectory(tmpDir, txnBaseDir);

			// Reload transaction file should resume prior commit
			using (var transaction = new FileTransaction(txnFile, txnBaseDir)) {
				// Checks transaction is empty and usable (resumption was done in constructor)
				ClassicAssert.AreEqual(FileTransactionState.Unchanged, transaction.Status);
				ClassicAssert.IsEmpty(transaction.EnlistedFiles);
			}

			// check no transaction junk
			ClassicAssert.AreEqual(0, Tools.FileSystem.GetFiles(txnBaseDir).Count());

			// check files match expected
			for (var i = 0; i < numFiles; i++) {
				var filename = Path.Combine(fileBaseDir, $"File{i}.dat");
				var fileData = File.ReadAllBytes(filename);
				ClassicAssert.AreEqual(expected[i], fileData);
			}
		}
	}

	[Test]
	[Sequential]
	public void Integration_Rollback([Values(1, 2, 7, 11)] int numFiles) {
		List<List<byte>> original;
		var expected = new List<List<byte>>();
		var files = new List<TransactionalFileMappedBuffer>();
		var RNG = new Random(1231);

		var fileBaseDir = Tools.FileSystem.GetTempEmptyDirectory(true);
		var txnBaseDir = Tools.FileSystem.GetTempEmptyDirectory(true);
		var txnFile = Path.Combine(txnBaseDir, "Test.txn");

		using (Tools.Scope.ExecuteOnDispose(() => Tools.FileSystem.DeleteDirectories(fileBaseDir, txnBaseDir))) {
			using (var transaction = new FileTransaction(txnFile, txnBaseDir)) {
				// create arrays
				expected = new List<List<byte>>();
				for (var i = 0; i < numFiles; i++) {
					var filename = Path.Combine(fileBaseDir, $"File{i}.dat");
					var pageSize = i * i + 1;
					var maxOpenPages = i / 2 + 1;
					var maxCapacity = pageSize * (i + 1);

					// every odd file is new, even is pre-created
					expected.Add(new List<byte>());
					if (i % 2 == 0) {
						var startBytes = RNG.NextBytes(maxCapacity / 2);
						if (startBytes.Any()) {
							File.WriteAllBytes(filename, startBytes);
							expected[i].AddRange(startBytes);
						}
					}
					files.Add(transaction.EnlistFile(filename, pageSize, maxOpenPages * pageSize));
				}

				// Original is a clone of expected before mutations
				original = expected.Select(l => l.Select(b => b).ToList()).ToList();

				// Mutate all the files 100 times, commit every 10 times
				for (var j = 0; j < 100; j++) {
					for (var i = 0; i < numFiles; i++) {
						var pageSize = i * i + 1;
						var maxCapacity = pageSize * (i + 1);
						var file = files[i];
						var expectedArr = expected[i];
						MutateLists(expectedArr, file, maxCapacity, RNG);
					}
				}

				// final commit
				transaction.Rollback();
			}

			// check no transaction junk
			ClassicAssert.AreEqual(0, Tools.FileSystem.GetFiles(txnBaseDir).Count());

			// check files match expected
			for (var i = 0; i < numFiles; i++) {
				var filename = Path.Combine(fileBaseDir, $"File{i}.dat");
				ClassicAssert.AreEqual(original[i], File.ReadAllBytes(filename));
			}
		}
	}

	[Test]
	[Sequential]
	public void Integration_ResumeRollback([Values(1, 2, 7, 11)] int numFiles) {
		List<List<byte>> original;
		var expected = new List<List<byte>>();
		var files = new List<TransactionalFileMappedBuffer>();
		var RNG = new Random(1231);

		var fileBaseDir = Tools.FileSystem.GetTempEmptyDirectory(true);
		var txnBaseDir = Tools.FileSystem.GetTempEmptyDirectory(true);
		var tmpDir = Tools.FileSystem.GetTempEmptyDirectory(true);
		var txnFile = Path.Combine(txnBaseDir, "Test.txn");

		using (Tools.Scope.ExecuteOnDispose(() => Tools.FileSystem.DeleteDirectories(fileBaseDir, txnBaseDir, tmpDir))) {
			using (var transaction = new FileTransaction(txnFile, txnBaseDir)) {
				// create arrays
				expected = new List<List<byte>>();
				for (var i = 0; i < numFiles; i++) {
					var filename = Path.Combine(fileBaseDir, $"File{i}.dat");
					var pageSize = i * i + 1;
					var maxOpenPages = i / 2 + 1;
					var maxCapacity = pageSize * (i + 1);

					// every odd file is new, even is pre-created
					expected.Add(new List<byte>());
					if (i % 2 == 0) {
						var startBytes = RNG.NextBytes(maxCapacity / 2);
						File.WriteAllBytes(filename, startBytes);
						expected[i].AddRange(startBytes);
					}
					files.Add(transaction.EnlistFile(filename, pageSize, maxOpenPages * pageSize));
				}

				// Original is a clone of expected before mutations
				original = expected.Select(l => l.Select(b => b).ToList()).ToList();

				// Mutate all the files 100 times, commit every 10 times
				for (var j = 0; j < 100; j++) {
					for (var i = 0; i < numFiles; i++) {
						var pageSize = i * i + 1;
						var maxCapacity = pageSize * (i + 1);
						var file = files[i];
						var expectedArr = expected[i];
						MutateLists(expectedArr, file, maxCapacity, RNG);
					}
				}
				// Flush all in-mem updates to disk
				transaction.Flush();

				// Hack: Set transaction to RollingBack state for simulating a power outage
				Tools.Reflection.SetPropertyValue(transaction, "Status", FileTransactionState.RollingBack);
				Tools.Reflection.InvokeMethod(transaction, "SaveHeader");

				// Backup current "committing" txn state to tmp folder 
				Tools.FileSystem.CopyDirectory(txnBaseDir, tmpDir);

				// Rollback everything 
				Tools.Reflection.SetPropertyValue(transaction, "Status", FileTransactionState.HasChanges);
				Tools.Reflection.InvokeMethod(transaction, "SaveHeader");
				transaction.Rollback();
			}
			// Now we restore the backed up "comitting" folder to
			// simulate a power outage in middle of commit.
			Tools.FileSystem.CopyDirectory(tmpDir, txnBaseDir);

			// Reload transaction file should resume prior rollback
			using (var transaction = new FileTransaction(txnFile, txnBaseDir)) {
				// Checks transaction is empty and usable (resumption was done in constructor)
				ClassicAssert.AreEqual(FileTransactionState.Unchanged, transaction.Status);
				ClassicAssert.IsEmpty(transaction.EnlistedFiles);
			}

			// check no transaction junk
			ClassicAssert.AreEqual(0, Tools.FileSystem.GetFiles(txnBaseDir).Count());

			// check files are unchanged
			for (var i = 0; i < numFiles; i++) {
				var filename = Path.Combine(fileBaseDir, $"File{i}.dat");
				var fileData = File.ReadAllBytes(filename);
				ClassicAssert.AreEqual(original[i], fileData);
			}
		}
	}

	private void MutateLists(List<byte> expected, TransactionalFileMappedBuffer file, int maxCapacity, Random RNG) {
		// add a random amount
		var remainingCapacity = maxCapacity - file.Count;
		var newItemsCount = RNG.Next(0, (int)remainingCapacity + 1);
		IEnumerable<byte> newItems = RNG.NextBytes(newItemsCount);
		file.AddRange(newItems);
		expected.AddRange(newItems);

		// update a random amount
		if (file.Count > 0) {
			var range = RNG.NextRange((int)file.Count);
			newItems = RNG.NextBytes(range.End - range.Start + 1);
			expected.UpdateRangeSequentially(range.Start, newItems);
			file.UpdateRange(range.Start, newItems);

			// shuffle a random amount
			range = RNG.NextRange((int)file.Count);
			newItems = file.ReadRange(range.Start, range.End - range.Start + 1);
			var expectedNewItems = expected.GetRange(range.Start, range.End - range.Start + 1);

			range = RNG.NextRange((int)file.Count, rangeLength: newItems.Count());
			expected.UpdateRangeSequentially(range.Start, expectedNewItems);
			file.UpdateRange(range.Start, newItems);

			// remove a random amount (FROM END OF LIST)
			range = new ValueRange<int>(RNG.Next(0, (int)file.Count), (int)file.Count - 1);
			file.RemoveRange(range.Start, range.End - range.Start + 1);
			expected.RemoveRange(range.Start, range.End - range.Start + 1);
		}
	}

}
