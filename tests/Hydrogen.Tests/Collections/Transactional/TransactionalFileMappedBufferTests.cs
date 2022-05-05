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
using System.CodeDom;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using NUnit.Framework;
using System.IO;
using NUnit.Framework.Constraints;
using Hydrogen;

namespace Hydrogen.Tests {

	[TestFixture]
	[Parallelizable(ParallelScope.Children)]
	public class TransactionalFileMappedBufferTests {
		private const int RandomSeed = 123292;

		private static void AssertEmptyDir(string dir) {
			Assert.IsTrue(Directory.Exists(dir));
			Assert.AreEqual(0, Directory.GetFiles(dir).Length);
		}

		private static void AssertSingleFile(string dir, string filename) {
			Assert.IsTrue(Directory.Exists(dir));
			var files = Directory.GetFiles(dir);
			Assert.AreEqual(1, files.Length);
			Assert.AreEqual(Path.GetFileName(filename), Path.GetFileName(files[0]));
		}

		private static void AssertFileCount(string dir, int expectedCount) {
			Assert.IsTrue(Directory.Exists(dir));
			var files = Directory.GetFiles(dir);
			Assert.AreEqual(expectedCount, files.Length);
		}

		#region Single Page

		[Test]
		public void ExistingFile_EditSinglePage_InMemoryUpdateOnly([Values(1, 17, 7173)] int pageSize) {
			var RNG = new Random(RandomSeed);
			var originalData = RNG.NextBytes(pageSize);
			var baseDir = Tools.FileSystem.GetTempEmptyDirectory(true);
			var fileName = Path.Combine(baseDir, "File.dat");

			Tools.FileSystem.AppendAllBytes(fileName, originalData);
			using (Tools.Scope.ExecuteOnDispose(() => Tools.FileSystem.DeleteDirectory(baseDir))) {
				using (var file = new TransactionalFileMappedBuffer(fileName, pageSize, 1*pageSize)) {
					if (file.RequiresLoad)
						file.Load();

					// flip all the data in the page
					for (var i = 0; i < pageSize; i++)
						file[i] ^= file[i];
					file[0] ^= file[0];  // page 0 should be changed, but in memory only

					var pageFile = TransactionalFileMappedBuffer.MarkerRepository.GeneratePageMarkerFileName(baseDir, file.FileID, TransactionalFileMappedBuffer.PageMarkerType.UncommittedPage, 0);

					// Check pagefile for page 0 doesn't exist yet
					Assert.IsTrue(!File.Exists(pageFile));
				}
			}
		}

		[Test]
		public void ExistingFile_EditSinglePage_UncommittedPageCreated([Values(1, 17, 7173)] int pageSize) {
			var RNG = new Random(RandomSeed);
			var originalData = RNG.NextBytes(pageSize);
			var baseDir = Tools.FileSystem.GetTempEmptyDirectory(true);
			var fileName = Path.Combine(baseDir, "File.dat");

			Tools.FileSystem.AppendAllBytes(fileName, originalData);
			using (Tools.Scope.ExecuteOnDispose(() => Tools.FileSystem.DeleteDirectory(baseDir))) {
				using (var file = new TransactionalFileMappedBuffer(fileName, pageSize, 1 * pageSize)) {
					if (file.RequiresLoad)
						file.Load();

					// flip all the data in the page
					for (var i = 0; i < pageSize; i++)
						file[i] ^= file[i];
					file[0] ^= file[0];  // page 0 should be changed, but in memory only

					var pageFile = TransactionalFileMappedBuffer.MarkerRepository.GeneratePageMarkerFileName(baseDir, file.FileID, TransactionalFileMappedBuffer.PageMarkerType.UncommittedPage, 0);

					// Cause page 0 to be flushed
					file.Flush();

					// Check page file for page 0 does exist yet
					Assert.IsTrue(File.Exists(pageFile));
				}
			}
		}

		[Test]
		public void ExistingFile_EditSinglePage_Rollback([Values(1, 17, 7173)] int pageSize) {
			var RNG = new Random(RandomSeed);
			var originalData = RNG.NextBytes(pageSize);
			var baseDir = Tools.FileSystem.GetTempEmptyDirectory(true);
			var fileName = Path.Combine(baseDir, "File.dat");

			Tools.FileSystem.AppendAllBytes(fileName, originalData);
			using (Tools.Scope.ExecuteOnDispose(() => Tools.FileSystem.DeleteDirectory(baseDir))) {
				using (var file = new TransactionalFileMappedBuffer(fileName, pageSize, 1 * pageSize)) {
					if (file.RequiresLoad)
						file.Load();

					// flip all the data in the page
					for (var i = 0; i < pageSize; i++)
						file[i] ^= file[i];
					file[0] ^= file[0];  // page 0 should be changed, but in memory only

					var pageFile = TransactionalFileMappedBuffer.MarkerRepository.GeneratePageMarkerFileName(baseDir, file.FileID, TransactionalFileMappedBuffer.PageMarkerType.UncommittedPage, 0);

					// Check pagefile for page 0 doesn't exist yet
					Assert.IsTrue(!File.Exists(pageFile));

					// Cause page 0 to be flushed
					file.Flush();

					// rollback transaction
					file.Rollback();
				}
				// Only original file should remain
				AssertSingleFile(baseDir, fileName);

				// Contains original data, no changes
				Assert.AreEqual(originalData, File.ReadAllBytes(fileName));
			}
		}

		[Test]
		public void ExistingFile_EditSinglePage_Commit([Values(1, 17, 7173)] int pageSize) {
			var RNG = new Random(RandomSeed);
			var originalData = RNG.NextBytes(pageSize);
			var baseDir = Tools.FileSystem.GetTempEmptyDirectory(true);
			var fileName = Path.Combine(baseDir, "File.dat");

			Tools.FileSystem.AppendAllBytes(fileName, originalData);
			using (Tools.Scope.ExecuteOnDispose(() => Tools.FileSystem.DeleteDirectory(baseDir))) {
				using (var file = new TransactionalFileMappedBuffer(fileName, pageSize, 1 * pageSize)) {
					if (file.RequiresLoad)
						file.Load();

					// flip all the data in the page
					for (var i = 0; i < pageSize; i++)
						file[i] ^= file[i];
					file[0] ^= file[0];  // page 0 should be changed, but in memory only

					var pageFile = TransactionalFileMappedBuffer.MarkerRepository.GeneratePageMarkerFileName(baseDir, file.FileID, TransactionalFileMappedBuffer.PageMarkerType.UncommittedPage, 0);

					// Check pagefile for page 0 doesn't exist yet
					Assert.IsTrue(!File.Exists(pageFile));

					// Cause page 0 to be flushed
					file.Flush();

					// Commit transaction
					file.Commit();
				}
				// Only original file should remain
				AssertSingleFile(baseDir, fileName);

				// Contains transformed data, with changes
				Assert.AreEqual(originalData.Select(b => b ^ b).ToArray(), File.ReadAllBytes(fileName));
			}
		}

		[Test]
		public void ExistingFile_EditSinglePage_Resume_Commit([Values(1, 17, 7173)] int pageSize) {
			var RNG = new Random(RandomSeed);
			var originalData = RNG.NextBytes(pageSize);
			var baseDir = Tools.FileSystem.GetTempEmptyDirectory(true);
			var fileName = Path.Combine(baseDir, "File.dat");

			Tools.FileSystem.AppendAllBytes(fileName, originalData);
			using (Tools.Scope.ExecuteOnDispose(() => Tools.FileSystem.DeleteDirectory(baseDir))) {
				// Write a commit header and a deleted page
				var fileID = TransactionalFileMappedBuffer.ComputeFileID(fileName);
				var commitFile = TransactionalFileMappedBuffer.MarkerRepository.GenerateFileMarkerFileName(baseDir, fileID, TransactionalFileMappedBuffer.FileMarkerType.Committing);
				var pageFile = TransactionalFileMappedBuffer.MarkerRepository.GeneratePageMarkerFileName(baseDir, fileID, TransactionalFileMappedBuffer.PageMarkerType.UncommittedPage, 0);
				Tools.FileSystem.CreateBlankFile(commitFile);
				File.WriteAllBytes(pageFile, originalData.Select(b => (byte)(b ^ b)).ToArray());
				using (var file = new TransactionalFileMappedBuffer(fileName, pageSize, 1 * pageSize)) {
					file.Load(); // resumes commit
				}

				// Only original file should remain
				AssertSingleFile(baseDir, fileName);

				// Contains transformed data, with changes
				Assert.AreEqual(originalData.Select(b => b ^ b).ToArray(), File.ReadAllBytes(fileName));
			}
		}

		[Test]
		public void ExistingFile_EditSinglePage_Resume_Rollback([Values(1, 17, 7173)] int pageSize) {
			var RNG = new Random(RandomSeed);
			var originalData = RNG.NextBytes(pageSize);
			var baseDir = Tools.FileSystem.GetTempEmptyDirectory(true);
			var fileName = Path.Combine(baseDir, "File.dat");

			Tools.FileSystem.AppendAllBytes(fileName, originalData);
			using (Tools.Scope.ExecuteOnDispose(() => Tools.FileSystem.DeleteDirectory(baseDir))) {
				// Write a commit header and a deleted page
				var fileID = TransactionalFileMappedBuffer.ComputeFileID(fileName);
				var rollbackFile = TransactionalFileMappedBuffer.MarkerRepository.GenerateFileMarkerFileName(baseDir, fileID, TransactionalFileMappedBuffer.FileMarkerType.RollingBack);
				var pageFile = TransactionalFileMappedBuffer.MarkerRepository.GeneratePageMarkerFileName(baseDir, fileID, TransactionalFileMappedBuffer.PageMarkerType.UncommittedPage, 0);
				Tools.FileSystem.CreateBlankFile(rollbackFile);
				File.WriteAllBytes(pageFile, originalData.Select(b => (byte)(b ^ b)).ToArray());
				using (var file = new TransactionalFileMappedBuffer(fileName, pageSize, 1 * pageSize)) {
					file.Load();
				}

				// Only original file should remain
				AssertSingleFile(baseDir, fileName);

				// Contains original data, no changes
				Assert.AreEqual(originalData, File.ReadAllBytes(fileName));
			}
		}

		[Test]
		public void ExistingFile_DeleteSinglePage_MarkerCreated([Values(1, 17, 7173)] int pageSize) {
			var RNG = new Random(RandomSeed);
			var originalData = RNG.NextBytes(pageSize);
			var baseDir = Tools.FileSystem.GetTempEmptyDirectory(true);
			var fileName = Path.Combine(baseDir, "File.dat");

			Tools.FileSystem.AppendAllBytes(fileName, originalData);
			using (Tools.Scope.ExecuteOnDispose(() => Tools.FileSystem.DeleteDirectory(baseDir))) {
				using (var file = new TransactionalFileMappedBuffer(fileName, pageSize, 1 * pageSize)) {
					if (file.RequiresLoad)
						file.Load();

					// delete data
					file.RemoveRange(0, file.Count);
					var markerFile = TransactionalFileMappedBuffer.MarkerRepository.GeneratePageMarkerFileName(baseDir, file.FileID, TransactionalFileMappedBuffer.PageMarkerType.DeletedMarker, 0);
					Assert.IsTrue(File.Exists(markerFile));
				}
			}
		}

		[Test]
		public void ExistingFile_DeleteSinglePage_MarkerNotCreated([Values(1, 17, 7173)] int pageSize) {
			var RNG = new Random(RandomSeed);
			var originalData = RNG.NextBytes(pageSize);
			var baseDir = Tools.FileSystem.GetTempEmptyDirectory(true);
			var fileName = Path.Combine(baseDir, "File.dat");

			Tools.FileSystem.AppendAllBytes(fileName, originalData);
			using (Tools.Scope.ExecuteOnDispose(() => Tools.FileSystem.DeleteDirectory(baseDir))) {
				using (var file = new TransactionalFileMappedBuffer(fileName, pageSize, 1 * pageSize)) {
					if (file.RequiresLoad)
						file.Load();

					// Add a new page
					file.AddRange(RNG.NextBytes(Math.Max(pageSize, pageSize / 2)));

					// Delete last page (shouldn't leave marker since was only in memory)
					var lastPage = file.Pages.Last();
					file.RemoveRange(lastPage.StartIndex, lastPage.Count);
					
					var markerFile = TransactionalFileMappedBuffer.MarkerRepository.GeneratePageMarkerFileName(baseDir, file.FileID, TransactionalFileMappedBuffer.PageMarkerType.DeletedMarker, lastPage.Number);
					Assert.IsFalse(File.Exists(markerFile));
				}
			}
		}

		[Test]
		public void ExistingFile_DeleteSinglePage_Rollback([Values(1, 17, 7173)] int pageSize) {
			var RNG = new Random(RandomSeed);
			var originalData = RNG.NextBytes(pageSize);
			var baseDir = Tools.FileSystem.GetTempEmptyDirectory(true);
			var fileName = Path.Combine(baseDir, "File.dat");

			Tools.FileSystem.AppendAllBytes(fileName, originalData);
			using (Tools.Scope.ExecuteOnDispose(() => Tools.FileSystem.DeleteDirectory(baseDir))) {
				using (var file = new TransactionalFileMappedBuffer(fileName, pageSize, 1 * pageSize)) {
					if (file.RequiresLoad)
						file.Load();

					// delete data
					file.RemoveRange(0, file.Count);
					var markerFile = TransactionalFileMappedBuffer.MarkerRepository.GeneratePageMarkerFileName(baseDir, file.FileID, TransactionalFileMappedBuffer.PageMarkerType.DeletedMarker, 0);
					Assert.IsTrue(File.Exists(markerFile));

					// rollback transaction
					file.Rollback();
				}
				// Only original file should remain
				AssertSingleFile(baseDir, fileName);

				// Contains original data, no changes
				Assert.AreEqual(originalData, File.ReadAllBytes(fileName));
			}
		}

		[Test]
		public void ExistingFile_DeleteSinglePage_Commit([Values(1, 17, 7173)] int pageSize) {
			var RNG = new Random(RandomSeed);
			var originalData = RNG.NextBytes(pageSize);
			var baseDir = Tools.FileSystem.GetTempEmptyDirectory(true);
			var fileName = Path.Combine(baseDir, "File.dat");

			Tools.FileSystem.AppendAllBytes(fileName, originalData);
			using (Tools.Scope.ExecuteOnDispose(() => Tools.FileSystem.DeleteDirectory(baseDir))) {
				using (var file = new TransactionalFileMappedBuffer(fileName, pageSize, 1 * pageSize)) {
					if (file.RequiresLoad)
						file.Load();

					// delete data
					file.RemoveRange(0, file.Count);
					var markerFile = TransactionalFileMappedBuffer.MarkerRepository.GeneratePageMarkerFileName(baseDir, file.FileID, TransactionalFileMappedBuffer.PageMarkerType.DeletedMarker, 0);
					Assert.IsTrue(File.Exists(markerFile));
					// Commit transaction
					file.Commit();
				}
				// Only original file should remain
				AssertSingleFile(baseDir, fileName);

				// Contains transformed data, with changes
				Assert.AreEqual(new byte[0], File.ReadAllBytes(fileName));
			}
		}

		[Test]
		public void ExistingFile_DeleteSinglePage_Resume_Commit([Values(1, 17, 7173)] int pageSize) {
			var RNG = new Random(RandomSeed);
			var originalData = RNG.NextBytes(pageSize);
			var baseDir = Tools.FileSystem.GetTempEmptyDirectory(true);
			var fileName = Path.Combine(baseDir, "File.dat");

			Tools.FileSystem.AppendAllBytes(fileName, originalData);
			using (Tools.Scope.ExecuteOnDispose(() => Tools.FileSystem.DeleteDirectory(baseDir))) {
				// Write a commit header and a deleted page
				var fileID = TransactionalFileMappedBuffer.ComputeFileID(fileName);
				var commitFile = TransactionalFileMappedBuffer.MarkerRepository.GenerateFileMarkerFileName(baseDir, fileID, TransactionalFileMappedBuffer.FileMarkerType.Committing);
				var markerFile = TransactionalFileMappedBuffer.MarkerRepository.GeneratePageMarkerFileName(baseDir, fileID, TransactionalFileMappedBuffer.PageMarkerType.DeletedMarker, 0);
				Tools.FileSystem.CreateBlankFile(commitFile);
				Tools.FileSystem.CreateBlankFile(markerFile);
				using (var file = new TransactionalFileMappedBuffer(fileName, pageSize, 1 * pageSize)) {
					file.Load(); // resumes commit
				}

				// Only original file should remain
				AssertSingleFile(baseDir, fileName);

				// Contains transformed data, with changes
				Assert.AreEqual(new byte[0], File.ReadAllBytes(fileName));
			}
		}

		[Test]
		public void ExistingFile_DeleteSinglePage_Resume_Rollback([Values(1, 17, 7173)] int pageSize) {
			var RNG = new Random(RandomSeed);
			var originalData = RNG.NextBytes(pageSize);
			var baseDir = Tools.FileSystem.GetTempEmptyDirectory(true);
			var fileName = Path.Combine(baseDir, "File.dat");

			Tools.FileSystem.AppendAllBytes(fileName, originalData);
			using (Tools.Scope.ExecuteOnDispose(() => Tools.FileSystem.DeleteDirectory(baseDir))) {
				// Write a commit header and a deleted page
				var fileID = TransactionalFileMappedBuffer.ComputeFileID(fileName);
				var commitFile = TransactionalFileMappedBuffer.MarkerRepository.GenerateFileMarkerFileName(baseDir, fileID, TransactionalFileMappedBuffer.FileMarkerType.RollingBack);
				var markerFile = TransactionalFileMappedBuffer.MarkerRepository.GeneratePageMarkerFileName(baseDir, fileID, TransactionalFileMappedBuffer.PageMarkerType.DeletedMarker, 0);
				Tools.FileSystem.CreateBlankFile(commitFile);
				Tools.FileSystem.CreateBlankFile(markerFile);
				using (var file = new TransactionalFileMappedBuffer(fileName, pageSize, 1 * pageSize)) {
					file.Load(); // resumes rollback
				}

				// Only original file should remain
				AssertSingleFile(baseDir, fileName);

				// Contains original data, no changes
				Assert.AreEqual(originalData, File.ReadAllBytes(fileName));
			}
		}

		#endregion

		#region TwoPage

		[Test]
		public void ExistingFile_DeleteTwoPage_Commit([Values(1, 17, 7173)] int pageSize) {
			var RNG = new Random(RandomSeed);
			var originalData = RNG.NextBytes(pageSize * 2);
			var baseDir = Tools.FileSystem.GetTempEmptyDirectory(true);
			var fileName = Path.Combine(baseDir, "File.dat");

			Tools.FileSystem.AppendAllBytes(fileName, originalData);
			using (Tools.Scope.ExecuteOnDispose(() => Tools.FileSystem.DeleteDirectory(baseDir))) {
				using (var file = new TransactionalFileMappedBuffer(fileName, pageSize, 1 * pageSize)) {
					if (file.RequiresLoad)
						file.Load();

					// delete data
					file.RemoveRange(file.Pages[1].StartIndex, file.Pages[1].Count);
					var markerFile = TransactionalFileMappedBuffer.MarkerRepository.GeneratePageMarkerFileName(baseDir, file.FileID, TransactionalFileMappedBuffer.PageMarkerType.DeletedMarker, 1);
					Assert.IsTrue(File.Exists(markerFile));
					// Commit transaction
					file.Commit();
				}
				// Only original file should remain
				AssertSingleFile(baseDir, fileName);

				// Contains transformed data, with changes
				Assert.AreEqual(originalData.Take(pageSize).ToArray(), File.ReadAllBytes(fileName));
			}
		}

		[Test]
		public void ExistingFile_DeleteTwoPage_MarkerCreated([Values(1, 17, 7173)] int pageSize) {
			var RNG = new Random(RandomSeed);
			var originalData = RNG.NextBytes(pageSize + pageSize*2);
			var baseDir = Tools.FileSystem.GetTempEmptyDirectory(true);
			var fileName = Path.Combine(baseDir, "File.dat");

			Tools.FileSystem.AppendAllBytes(fileName, originalData);
			using (Tools.Scope.ExecuteOnDispose(() => Tools.FileSystem.DeleteDirectory(baseDir))) {
				using (var file = new TransactionalFileMappedBuffer(fileName, pageSize, 1 * pageSize)) {
					if (file.RequiresLoad)
						file.Load();

					// Delete last page (shouldn't leave marker since was only in memory)
					var lastPages = file.Pages.Skip(file.Pages.Count() - 2).ToArray();
					file.RemoveRange(lastPages[0].StartIndex, lastPages[0].Count + lastPages[1].Count);

					var markerFile1 = TransactionalFileMappedBuffer.MarkerRepository.GeneratePageMarkerFileName(baseDir, file.FileID, TransactionalFileMappedBuffer.PageMarkerType.DeletedMarker, lastPages[0].Number);
					var markerFile2 = TransactionalFileMappedBuffer.MarkerRepository.GeneratePageMarkerFileName(baseDir, file.FileID, TransactionalFileMappedBuffer.PageMarkerType.DeletedMarker, lastPages[1].Number);
					Assert.IsTrue(File.Exists(markerFile1));
					Assert.IsTrue(File.Exists(markerFile2));
				}
			}
		}

		[Test]
		public void ExistingFile_DeleteTwoPage_MarkerNotCreated([Values(1, 17, 7173)] int pageSize) {
			var RNG = new Random(RandomSeed);
			var originalData = RNG.NextBytes(pageSize);
			var baseDir = Tools.FileSystem.GetTempEmptyDirectory(true);
			var fileName = Path.Combine(baseDir, "File.dat");

			Tools.FileSystem.AppendAllBytes(fileName, originalData);
			using (Tools.Scope.ExecuteOnDispose(() => Tools.FileSystem.DeleteDirectory(baseDir))) {
				using (var file = new TransactionalFileMappedBuffer(fileName, pageSize, 1 * pageSize)) {
					if (file.RequiresLoad)
						file.Load();

					// Add a new page
					file.AddRange(RNG.NextBytes(pageSize * 2));

					// Delete last page (shouldn't leave marker since was only in memory)
					var lastPages = file.Pages.Skip(file.Pages.Count() - 2).ToArray();
					file.RemoveRange(lastPages[0].StartIndex, lastPages[0].Count + lastPages[1].Count);

					var markerFile1 = TransactionalFileMappedBuffer.MarkerRepository.GeneratePageMarkerFileName(baseDir, file.FileID, TransactionalFileMappedBuffer.PageMarkerType.DeletedMarker, lastPages[0].Number);
					var markerFile2 = TransactionalFileMappedBuffer.MarkerRepository.GeneratePageMarkerFileName(baseDir, file.FileID, TransactionalFileMappedBuffer.PageMarkerType.DeletedMarker, lastPages[1].Number);
					Assert.IsFalse(File.Exists(markerFile1));
					Assert.IsFalse(File.Exists(markerFile2));
				}
			}
		}


		#endregion

		#region Special tests

		[Test]
		public void Special_DeletePartialPage_Rollback() {
			const int pageSize = 100;
			const int lastPageRemainingData = 17;
			var RNG = new Random(RandomSeed);
			var originalData = RNG.NextBytes(pageSize * 2);
			var baseDir = Tools.FileSystem.GetTempEmptyDirectory(true);
			var fileName = Path.Combine(baseDir, "File.dat");

			Tools.FileSystem.AppendAllBytes(fileName, originalData);
			using (Tools.Scope.ExecuteOnDispose(() => Tools.FileSystem.DeleteDirectory(baseDir))) {
				using (var file = new TransactionalFileMappedBuffer(fileName, pageSize, 1 * pageSize)) {
					if (file.RequiresLoad)
						file.Load();

					file.RemoveRange(pageSize + lastPageRemainingData, pageSize - lastPageRemainingData);
					file.Flush();
					var markerFile = TransactionalFileMappedBuffer.MarkerRepository.GeneratePageMarkerFileName(baseDir, file.FileID, TransactionalFileMappedBuffer.PageMarkerType.UncommittedPage, 1);
					Assert.IsTrue(File.Exists(markerFile));
					Assert.AreEqual(lastPageRemainingData, Tools.FileSystem.GetFileSize(markerFile));

					// Commit transaction
					file.Rollback();
				}
				// Only original file should remain
				AssertSingleFile(baseDir, fileName);

				// Contains transformed data, with changes
				Assert.AreEqual(originalData, File.ReadAllBytes(fileName));
			}
		}

		[Test]
		public void Special_DeletePartialPage_Commit() {
			const int pageSize = 100;
			const int lastPageRemainingData = 11;
			var RNG = new Random(RandomSeed);
			var originalData = RNG.NextBytes(pageSize * 2);
			var baseDir = Tools.FileSystem.GetTempEmptyDirectory(true);
			var fileName = Path.Combine(baseDir, "File.dat");

			Tools.FileSystem.AppendAllBytes(fileName, originalData);
			using (Tools.Scope.ExecuteOnDispose(() => Tools.FileSystem.DeleteDirectory(baseDir))) {
				using (var file = new TransactionalFileMappedBuffer(fileName, pageSize, 1 * pageSize)) {
					if (file.RequiresLoad)
						file.Load();

					file.RemoveRange(pageSize + lastPageRemainingData, pageSize - lastPageRemainingData);
					file.Flush();
					var markerFile = TransactionalFileMappedBuffer.MarkerRepository.GeneratePageMarkerFileName(baseDir, file.FileID, TransactionalFileMappedBuffer.PageMarkerType.UncommittedPage, 1);
					Assert.IsTrue(File.Exists(markerFile));
					Assert.AreEqual(lastPageRemainingData, Tools.FileSystem.GetFileSize(markerFile));

					// Commit transaction
					file.Commit();
				}
				// Only original file should remain
				AssertSingleFile(baseDir, fileName);

				// Contains transformed data, with changes
				var expected = originalData.Take(pageSize + lastPageRemainingData).ToArray();
				var actual = File.ReadAllBytes(fileName);
				Assert.AreEqual(expected, actual);
			}
		}

		[Test]
		public void Special_HasDeletePageMarkers([Values(1, 10, 100)] int maxOpenPages) {
			const int pageSize = 10;
			var RNG = new Random(RandomSeed);
			var originalData = RNG.NextBytes(pageSize * 10);
			var baseDir = Tools.FileSystem.GetTempEmptyDirectory(true);
			var fileName = Path.Combine(baseDir, "File.dat");

			Tools.FileSystem.AppendAllBytes(fileName, originalData);
			using (Tools.Scope.ExecuteOnDispose(() => Tools.FileSystem.DeleteDirectory(baseDir))) {
				using (var file = new TransactionalFileMappedBuffer(fileName, pageSize, maxOpenPages*pageSize)) {
					if (file.RequiresLoad)
						file.Load();

					// Delete pages 3..10
					file.RemoveRange(file.Pages[2].StartIndex, file.Pages.Skip(2).Aggregate(0, (c, p) => c + p.Count));
					file.Flush();

					// check deleted pages 3..10 exist
					for (var i = 2; i < 10; i++) {
						var markerFile = TransactionalFileMappedBuffer.MarkerRepository.GeneratePageMarkerFileName(baseDir, file.FileID, TransactionalFileMappedBuffer.PageMarkerType.DeletedMarker, i);
						Assert.IsTrue(File.Exists(markerFile));
					}

					// Commit transaction
					file.Rollback();
				}
				// Only original file should remain
				AssertSingleFile(baseDir, fileName);

				// Contains transformed data, with changes
				var expected = originalData;
				var actual = File.ReadAllBytes(fileName);
				Assert.AreEqual(expected, actual);
			}
		}

		[Test]
		public void Special_NoDeletePageMarkers([Values(1, 10, 100)] int maxOpenPages) {
			const int pageSize = 10;
			var RNG = new Random(RandomSeed);
			var originalData = RNG.NextBytes(pageSize * 2); // create 2 pages of default data
			var baseDir = Tools.FileSystem.GetTempEmptyDirectory(true);
			var fileName = Path.Combine(baseDir, "File.dat");

			Tools.FileSystem.AppendAllBytes(fileName, originalData);
			using (Tools.Scope.ExecuteOnDispose(() => Tools.FileSystem.DeleteDirectory(baseDir))) {
				using (var file = new TransactionalFileMappedBuffer(fileName, pageSize, maxOpenPages*pageSize)) {
					Assert.IsTrue(file.RequiresLoad);
					file.Load();

					// No pages should exist after load
					AssertSingleFile(baseDir, fileName);

					file.AddRange(RNG.NextBytes(pageSize * 10)); // add 10 pages
					file.Flush();

					// check deleted pages 3..10 exist
					for (var i = 2; i < 10; i++) {
						var markerFile = TransactionalFileMappedBuffer.MarkerRepository.GeneratePageMarkerFileName(baseDir, file.FileID, TransactionalFileMappedBuffer.PageMarkerType.UncommittedPage, i);
						Assert.IsTrue(File.Exists(markerFile));
					}

					// Delete pages 3..10
					file.RemoveRange(file.Pages[2].StartIndex, file.Pages.Skip(2).Aggregate(0, (c, p) => c + p.Count));
					file.Flush();

					// No pages should exist
					AssertSingleFile(baseDir, fileName);

					// Commit transaction
					file.Rollback();
				}
				// Only original file should remain
				AssertSingleFile(baseDir, fileName);

				// Contains transformed data, with changes
				var expected = originalData;
				var actual = File.ReadAllBytes(fileName);
				Assert.AreEqual(expected, actual);
			}
		}

		[Test]
		public void Special_NoDanglingUncommittedMarkersAfterUncommittedReload() {
			const int pageSize = 3;
			const int maxOpenPages = 2;
			var RNG = new Random(RandomSeed);
			var originalData = RNG.NextBytes(pageSize * 2); // create 2 pages of default data
			var baseDir = Tools.FileSystem.GetTempEmptyDirectory(true);
			var pageDir1 = Tools.FileSystem.GetTempEmptyDirectory(true);
			var pageDir2 = Tools.FileSystem.GetTempEmptyDirectory(true);
			var fileName = Path.Combine(baseDir, "File.dat");

			Tools.FileSystem.AppendAllBytes(fileName, originalData);
			using (Tools.Scope.ExecuteOnDispose(() => Tools.FileSystem.DeleteDirectories(baseDir, pageDir1, pageDir2))) {
				using (var file = new TransactionalFileMappedBuffer(fileName, pageDir1, pageSize, maxOpenPages*pageSize)) {
					file.Load();
					AssertFileCount(pageDir1, 0);

					file.AddRange(RNG.NextBytes(pageSize * 10)); // add 10 pages
					file.Flush();

					Tools.FileSystem.CopyDirectory(pageDir1, pageDir2);
					AssertFileCount(pageDir1, 10); // duplicate markers 
 				}

				using (var file = new TransactionalFileMappedBuffer(fileName, pageDir2, pageSize, maxOpenPages*pageSize)) {
					file.Load();
					AssertFileCount(pageDir1, 0);
				}
			}
		}

		[Test]
		public void Special_NoDanglingDeleteMarkersAfterUncommittedReload() {
			const int pageSize = 3;
			const int maxOpenPages = 2;
			var RNG = new Random(RandomSeed);
			var originalData = RNG.NextBytes(pageSize * 10); // create 2 pages of default data
			var baseDir = Tools.FileSystem.GetTempEmptyDirectory(true);
			var pageDir1 = Tools.FileSystem.GetTempEmptyDirectory(true);
			var pageDir2 = Tools.FileSystem.GetTempEmptyDirectory(true);
			var fileName = Path.Combine(baseDir, "File.dat");

			Tools.FileSystem.AppendAllBytes(fileName, originalData);
			using (Tools.Scope.ExecuteOnDispose(() => Tools.FileSystem.DeleteDirectories(baseDir, pageDir1, pageDir2))) {
				var fileID = Guid.NewGuid();
				using (var file = new TransactionalFileMappedBuffer(fileName, pageDir1, pageSize, maxOpenPages*pageSize)) {
					file.Load();
					AssertFileCount(pageDir1, 0);

					file.RemoveRange(file.Count - pageSize * 2, pageSize * 2);
					file.Flush();
					AssertFileCount(pageDir1, 2);

					Tools.FileSystem.CopyDirectory(pageDir1, pageDir2);
					AssertFileCount(pageDir1, 2); // duplicate markers 
				}

				using (var file = new TransactionalFileMappedBuffer(fileName, pageDir2, pageSize, maxOpenPages*pageSize)) {
					file.Load();
					AssertFileCount(pageDir1, 0);
				}
			}
		}


		[Test]
		public void DiposeBug() {
			var RNG = new Random(RandomSeed);
			var baseDir = Tools.FileSystem.GetTempEmptyDirectory(true);
			var fileName = Path.Combine(baseDir, "File.dat");
			using (Tools.Scope.ExecuteOnDispose(() => Tools.FileSystem.DeleteDirectory(baseDir))) {
				var file = new TransactionalFileMappedBuffer(fileName, 10, 100) { FlushOnDispose = false };
				if (file.RequiresLoad)
					file.Load();
				file.Add(1);
				Assert.That(() => file.Dispose(), Throws.Nothing);
			}
		}

		#endregion

		#region Event Tests

		[Test]
		public void CommitEvents() {
			var baseDir = Tools.FileSystem.GetTempEmptyDirectory(true);
			var fileName = Path.Combine(baseDir, "File.dat");
			using (Tools.Scope.ExecuteOnDispose(() => Tools.FileSystem.DeleteDirectory(baseDir))) {
				using (var file = new TransactionalFileMappedBuffer(fileName, 100, 1*100, autoLoad: true)) {
					var committingCount = 0;
					var committedCount = 0;
					var rollingBackCount = 0;
					var rolledBackCount = 0;

					file.Committing += _ => committingCount++;
					file.Committed += _ => committedCount++;
					file.AddRange(new Random(31337).NextBytes(100));
					Assert.AreEqual(0, committingCount);
					Assert.AreEqual(0, committedCount);
					Assert.AreEqual(0, rollingBackCount);
					Assert.AreEqual(0, rolledBackCount);

					// Commit transaction
					file.Commit();
					Assert.AreEqual(1, committingCount);
					Assert.AreEqual(1, committedCount);
					Assert.AreEqual(0, rollingBackCount);
					Assert.AreEqual(0, rolledBackCount);

				}
			}
		}

		[Test]
		public void RollbackEvents() {
			var baseDir = Tools.FileSystem.GetTempEmptyDirectory(true);
			var fileName = Path.Combine(baseDir, "File.dat");
			using (Tools.Scope.ExecuteOnDispose(() => Tools.FileSystem.DeleteDirectory(baseDir))) {
				using (var file = new TransactionalFileMappedBuffer(fileName, 100, 1*100, autoLoad: true)) {
					var committingCount = 0;
					var committedCount = 0;

					var rollingBackCount = 0;
					var rolledBackCount = 0;
					file.RollingBack += _ => rollingBackCount++;
					file.RolledBack += _ => rolledBackCount++;
					file.AddRange(new Random(31337).NextBytes(100));
					Assert.AreEqual(0, committingCount);
					Assert.AreEqual(0, committedCount);

					Assert.AreEqual(0, rollingBackCount);
					Assert.AreEqual(0, rolledBackCount);
					// Commit transaction
					file.Rollback();
					Assert.AreEqual(0, committingCount);
					Assert.AreEqual(0, committedCount);
					Assert.AreEqual(1, rollingBackCount);
					Assert.AreEqual(1, rolledBackCount);
				}
			}
		}

		#endregion

		#region Integration Tests
		[Test]
		[Sequential]
		public void Integration_Commit(
		[Values(1, 10, 57, 173, 1111)] int maxCapacity,
		[Values(1, 1, 3, 31, 13)] int pageSize,
		[Values(1, 1, 7, 2, 10000)] int maxOpenPages) {
			var expected = new List<byte>();
			var RNG = new Random(1231);
			var fileBaseDir = Tools.FileSystem.GetTempEmptyDirectory(true);
			var pageBaseDir = Tools.FileSystem.GetTempEmptyDirectory(true);
			var fileName = Path.Combine(fileBaseDir, "File.dat");
			var fileID = Guid.NewGuid();
			var startBytes = RNG.NextBytes(maxCapacity / 2);
			expected.AddRange(startBytes);
			File.WriteAllBytes(fileName, startBytes);
			using (Tools.Scope.ExecuteOnDispose(() => Tools.FileSystem.DeleteDirectories(fileBaseDir, pageBaseDir))) {
				using (var file = new TransactionalFileMappedBuffer(fileName, pageBaseDir, pageSize, maxOpenPages*pageSize)) {
					if (file.RequiresLoad)
						file.Load();

					for (var i = 0; i < 100; i++) {

						// add a random amount
						var remainingCapacity = maxCapacity - file.Count;
						var newItemsCount = RNG.Next(0, remainingCapacity + 1);
						IEnumerable<byte> newItems = RNG.NextBytes(newItemsCount);
						file.AddRange(newItems);
						expected.AddRange(newItems);
						Assert.AreEqual(expected, file);

						// update a random amount
						if (file.Count > 0) {
							var range = RNG.NextRange(file.Count);
							newItems = RNG.NextBytes(range.End - range.Start + 1);
							expected.UpdateRangeSequentially(range.Start, newItems);
							file.UpdateRange(range.Start, newItems);
							Assert.AreEqual(expected, file);

							// shuffle a random amount
							range = RNG.NextRange(file.Count);
							newItems = file.ReadRange(range.Start, range.End - range.Start + 1);
							var expectedNewItems = expected.GetRange(range.Start, range.End - range.Start + 1);

							range = RNG.NextRange(file.Count, rangeLength: newItems.Count());
							expected.UpdateRangeSequentially(range.Start, expectedNewItems);
							file.UpdateRange(range.Start, newItems);

							Assert.AreEqual(expected.Count, file.Count);
							Assert.AreEqual(expected, file);

							// remove a random amount (FROM END OF LIST)
							range = new ValueRange<int>(RNG.Next(0, file.Count), file.Count - 1);
							file.RemoveRange(range.Start, range.End - range.Start + 1);
							expected.RemoveRange(range.Start, range.End - range.Start + 1);
							Assert.AreEqual(expected, file);
						}
					}
					file.Commit();
				}
				var fileBytes = File.ReadAllBytes(fileName);
				Assert.AreEqual(expected, fileBytes);
			}
		}


		[Test]
		[Sequential]
		public void Intergration_ResumeCommit(
			[Values(1, 10, 57, 173, 1111)] int maxCapacity,
			[Values(1, 1, 3, 31, 13)] int pageSize,
			[Values(1, 1, 7, 2, 10000)] int maxOpenPages) {
			var expected = new List<byte>();
			var RNG = new Random(1231);
			var fileBaseDir = Tools.FileSystem.GetTempEmptyDirectory(true);
			var pageBaseDir = Tools.FileSystem.GetTempEmptyDirectory(true);
			var fileName = Path.Combine(fileBaseDir, "File.dat");
			var fileID = Guid.NewGuid();
			var startBytes = RNG.NextBytes(maxCapacity / 2);
			expected.AddRange(startBytes);
			File.WriteAllBytes(fileName, startBytes);

			using (Tools.Scope.ExecuteOnDispose(() => Tools.FileSystem.DeleteDirectories(fileBaseDir, pageBaseDir))) {
				// Do 10 transaction resumptions
				for (var j = 0; j < 10; j++) {
					string oldPageDir;
					// Do a bunch of operations on a transactional file and abort them, but copy the page files before abort
					using (var file = new TransactionalFileMappedBuffer(fileName, pageBaseDir, pageSize, maxOpenPages*pageSize)) {
						if (file.RequiresLoad)
							file.Load();

						// initial check
						Assert.AreEqual(expected, file);
						for (var i = 0; i < 10; i++) {

							// add a random amount
							var remainingCapacity = maxCapacity - file.Count;
							var newItemsCount = RNG.Next(0, remainingCapacity + 1);
							IEnumerable<byte> newItems = RNG.NextBytes(newItemsCount);
							file.AddRange(newItems);
							expected.AddRange(newItems);
							Assert.AreEqual(expected, file);

							// update a random amount
							if (file.Count > 0) {
								var range = RNG.NextRange(file.Count);
								newItems = RNG.NextBytes(range.End - range.Start + 1);
								expected.UpdateRangeSequentially(range.Start, newItems);
								file.UpdateRange(range.Start, newItems);
								Assert.AreEqual(expected, file);

								// shuffle a random amount
								range = RNG.NextRange(file.Count);
								newItems = file.ReadRange(range.Start, range.End - range.Start + 1);
								var expectedNewItems = expected.GetRange(range.Start, range.End - range.Start + 1);

								range = RNG.NextRange(file.Count, rangeLength: newItems.Count());
								expected.UpdateRangeSequentially(range.Start, expectedNewItems);
								file.UpdateRange(range.Start, newItems);

								Assert.AreEqual(expected.Count, file.Count);
								Assert.AreEqual(expected, file);

								// remove a random amount (FROM END OF LIST)
								range = new ValueRange<int>(RNG.Next(0, file.Count), file.Count - 1);
								file.RemoveRange(range.Start, range.End - range.Start + 1);
								expected.RemoveRange(range.Start, range.End - range.Start + 1);
								Assert.AreEqual(expected, file);
							}
						}
						file.Flush(); // write all pages to disk

						// Write a commiting page marker (next iteration will commit transaction when loading file)
						var markerFile = TransactionalFileMappedBuffer.MarkerRepository.GenerateFileMarkerFileName(pageBaseDir, file.FileID, TransactionalFileMappedBuffer.FileMarkerType.Committing);
						Tools.FileSystem.CreateBlankFile(markerFile);

						// Simulate a power-outage (copy all page markers to a new directory and abort transaction, load next
						// iteration from new directory
						oldPageDir = pageBaseDir;
						pageBaseDir = Tools.FileSystem.GetTempEmptyDirectory(true);  // next iteration will resume these pages
						Tools.FileSystem.CopyDirectory(oldPageDir, pageBaseDir);
						file.Rollback(); // delete page files
					}
					Tools.FileSystem.DeleteDirectory(oldPageDir);
				}

				// Do final commit
				using (var file = new TransactionalFileMappedBuffer(fileName, pageBaseDir, pageSize, maxOpenPages*pageSize)) {
					file.Load(); // should resume commit
				}
				Assert.AreEqual(expected, File.ReadAllBytes(fileName));
			}
		}


		#endregion

	}
}
