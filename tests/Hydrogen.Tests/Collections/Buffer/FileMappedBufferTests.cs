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
using Hydrogen.NUnit;

namespace Hydrogen.Tests;

[TestFixture]
[Parallelizable(ParallelScope.Children)]
public class FileMappedBufferTests {

	[Test]
	public void SingleByteFile_Save() {
		var expected = new byte[] { 127 };
		var fileName = Tools.FileSystem.GetTempFileName(true);
		using (Tools.Scope.ExecuteOnDispose(() => File.Delete(fileName))) {
			using (var binaryFile = new FileMappedBuffer(fileName, 1, 1, false, autoLoad: true)) {

				Assert.AreEqual(0, binaryFile.Pages.Count());
				binaryFile.AddRange(expected);

				// Check page
				Assert.AreEqual(1, binaryFile.Pages.Count());
				Assert.AreEqual(0, binaryFile.Pages[0].StartIndex);
				Assert.AreEqual(1, binaryFile.Pages[0].Count);
				Assert.AreEqual(0, binaryFile.Pages[0].EndIndex);
				Assert.IsTrue(binaryFile.Pages[0].Dirty);
			}
			// Check saved
			Assert.AreEqual(expected, File.ReadAllBytes(fileName));
		}
	}

	[Test]
	public void SingleByteFile_Load() {
		var expected = new byte[] { 127 };
		var fileName = Tools.FileSystem.GetTempFileName(true);
		Tools.FileSystem.AppendAllBytes(fileName, expected);
		using (Tools.Scope.ExecuteOnDispose(() => File.Delete(fileName))) {
			using (var binaryFile = new FileMappedBuffer(fileName, 1, 1, true, autoLoad: true)) {

				// Check page
				Assert.AreEqual(1, binaryFile.Pages.Count());
				Assert.AreEqual(0, binaryFile.Pages[0].StartIndex);
				Assert.AreEqual(1, binaryFile.Pages[0].Count);
				Assert.AreEqual(0, binaryFile.Pages[0].EndIndex);
				Assert.IsFalse(binaryFile.Pages[0].Dirty);

				// Check value
				Assert.AreEqual(expected, binaryFile);
				Assert.AreEqual(1, binaryFile.Count);
				Assert.AreEqual(1, binaryFile.CalculateTotalSize());
			}
			// Check file unchanged
			Assert.AreEqual(expected, File.ReadAllBytes(fileName));
		}
	}

	[Test]
	public void Grow() {
		var preExistingBytes = new byte[] { 127 };
		var appendedBytes = new byte[] { 17 };
		var expected = Tools.Array.Concat<byte>(preExistingBytes, appendedBytes);
		var fileName = Tools.FileSystem.GetTempFileName(true);
		Tools.FileSystem.AppendAllBytes(fileName, preExistingBytes);
		using (Tools.Scope.ExecuteOnDispose(() => File.Delete(fileName))) {
			using (var binaryFile = new FileMappedBuffer(fileName, 1, 1, false, autoLoad: true)) {

				binaryFile.AddRange(appendedBytes);

				// Check pages 1 & 2
				Assert.AreEqual(2, binaryFile.Pages.Count());
				Assert.IsTrue(binaryFile.Pages[0].State == PageState.Unloaded);
				Assert.AreEqual(0, binaryFile.Pages[0].StartIndex);
				Assert.AreEqual(1, binaryFile.Pages[0].Count);
				Assert.AreEqual(0, binaryFile.Pages[0].EndIndex);
				Assert.IsFalse(binaryFile.Pages[0].Dirty);

				Assert.IsTrue(binaryFile.Pages[1].State == PageState.Loaded);
				Assert.AreEqual(1, binaryFile.Pages[1].StartIndex);
				Assert.AreEqual(1, binaryFile.Pages[1].Count);
				Assert.AreEqual(1, binaryFile.Pages[1].EndIndex);
				Assert.IsTrue(binaryFile.Pages[1].Dirty);

				// Check value
				Assert.AreEqual(expected, binaryFile);
				Assert.AreEqual(2, binaryFile.Count);
				Assert.AreEqual(2, binaryFile.CalculateTotalSize());
			}
			// Check file was saved appended
			Assert.AreEqual(expected, File.ReadAllBytes(fileName));
		}
	}

	[Test]
	public void NibbleFile_Save() {
		var expected = new byte[] { 127, 17 };
		var fileName = Tools.FileSystem.GetTempFileName(true);
		using (Tools.Scope.ExecuteOnDispose(() => File.Delete(fileName))) {
			using (var binaryFile = new FileMappedBuffer(fileName, 1, 1, false, autoLoad: true)) {

				binaryFile.Add(expected[0]);

				// Check Page 1
				Assert.AreEqual(1, binaryFile.Pages.Count());
				Assert.IsTrue(binaryFile.Pages[0].State == PageState.Loaded);
				Assert.AreEqual(0, binaryFile.Pages[0].StartIndex);
				Assert.AreEqual(1, binaryFile.Pages[0].Count);
				Assert.AreEqual(0, binaryFile.Pages[0].EndIndex);
				Assert.IsTrue(binaryFile.Pages[0].Dirty);

				// Add new page
				binaryFile.Add(expected[1]);

				// Check pages 1 & 2
				Assert.AreEqual(2, binaryFile.Pages.Count());
				Assert.IsTrue(binaryFile.Pages[0].State == PageState.Unloaded);
				Assert.AreEqual(0, binaryFile.Pages[0].StartIndex);
				Assert.AreEqual(1, binaryFile.Pages[0].Count);
				Assert.AreEqual(0, binaryFile.Pages[0].EndIndex);
				Assert.IsFalse(binaryFile.Pages[0].Dirty);

				Assert.IsTrue(binaryFile.Pages[1].State == PageState.Loaded);
				Assert.AreEqual(1, binaryFile.Pages[1].StartIndex);
				Assert.AreEqual(1, binaryFile.Pages[1].Count);
				Assert.AreEqual(1, binaryFile.Pages[1].EndIndex);
				Assert.IsTrue(binaryFile.Pages[1].Dirty);
			}

			// Check saved
			Assert.AreEqual(expected, File.ReadAllBytes(fileName));
		}
	}

	[Test]
	public void Rewind() {
		var expected = new byte[] { 127, 17, 18, 19 };
		var fileName = Tools.FileSystem.GetTempFileName(true);
		using (Tools.Scope.ExecuteOnDispose(() => File.Delete(fileName))) {
			using (var binaryFile = new FileMappedBuffer(fileName, 1, 1, false, autoLoad: true)) {

				binaryFile.AddRange<byte>(127, 16, 15, 14, 13);
				binaryFile.RemoveRange(1, 4);
				Assert.AreEqual(1, binaryFile.Count);
				Assert.AreEqual(127, binaryFile[0]);
				binaryFile.AddRange<byte>(17, 18, 19);
				Assert.AreEqual(4, binaryFile.Count);
				Assert.AreEqual(expected, binaryFile);
			}
			// Check saved
			Assert.AreEqual(expected, File.ReadAllBytes(fileName));
		}
	}

	[Test]
	public void NibbleFile_Load() {
		var expected = new byte[] { 127, 17 };
		var fileName = Tools.FileSystem.GetTempFileName(true);
		Tools.FileSystem.AppendAllBytes(fileName, expected);
		using (Tools.Scope.ExecuteOnDispose(() => File.Delete(fileName))) {
			using (var binaryFile = new FileMappedBuffer(fileName, 1, 1, true, autoLoad: true)) {

				// Check pages 1 & 2
				Assert.AreEqual(2, binaryFile.Pages.Count());
				Assert.IsTrue(binaryFile.Pages[0].State == PageState.Unloaded);
				Assert.AreEqual(0, binaryFile.Pages[0].StartIndex);
				Assert.AreEqual(1, binaryFile.Pages[0].Count);
				Assert.AreEqual(0, binaryFile.Pages[0].EndIndex);
				Assert.IsFalse(binaryFile.Pages[0].Dirty);

				Assert.IsTrue(binaryFile.Pages[1].State == PageState.Unloaded);
				Assert.AreEqual(1, binaryFile.Pages[1].StartIndex);
				Assert.AreEqual(1, binaryFile.Pages[1].Count);
				Assert.AreEqual(1, binaryFile.Pages[1].EndIndex);

				// Check values
				Assert.AreEqual(expected, binaryFile);
			}
			// Check file unchanged
			Assert.AreEqual(expected, File.ReadAllBytes(fileName));
		}
	}

	[Test]
	public void ReadOnly_SinglePage() {
		var expected = Enumerable.Range(0, 8).Select(x => (byte)x).ToArray();
		var fileName = Tools.FileSystem.GetTempFileName(true);
		Tools.FileSystem.AppendAllBytes(fileName, expected);
		using (Tools.Scope.ExecuteOnDispose(() => File.Delete(fileName)))
		using (var binaryFile = new FileMappedBuffer(fileName, 8, 4 * 8, true, autoLoad: true)) {

			for (var i = 0; i < 8; i++)
				Assert.AreEqual(expected[i], binaryFile[i]);
		}
	}

	[Test]
	public void ReadOnly_MultiPage() {
		var expected = Enumerable.Range(0, 256).Select(x => (byte)x).ToArray();
		var fileName = Tools.FileSystem.GetTempFileName(true);
		Tools.FileSystem.AppendAllBytes(fileName, expected);
		using (Tools.Scope.ExecuteOnDispose(() => File.Delete(fileName)))
		using (var binaryFile = new FileMappedBuffer(fileName, 8, 4 * 8, true, autoLoad: true)) {

			for (var i = 0; i < 256; i++)
				Assert.AreEqual(expected[i], binaryFile[i]);
		}
	}

	[Test]
	public void ReadOnly_Update() {
		var expected = Enumerable.Range(0, 256).Select(x => (byte)x).ToArray();
		var fileName = Tools.FileSystem.GetTempFileName(true);
		Tools.FileSystem.AppendAllBytes(fileName, expected.Reverse().ToArray());
		using (Tools.Scope.ExecuteOnDispose(() => File.Delete(fileName))) {
			// first load the file and sort them
			using (var binaryFile = new FileMappedBuffer(fileName, 8, 4 * 8, false, autoLoad: true)) {

				QuickSorter.Sort(binaryFile, Comparer<byte>.Default);
				for (var i = 0; i < 256; i++)
					Assert.AreEqual(expected[i], binaryFile[i]);
			}

			// check file is as expected
			using (var binaryFile = new FileMappedBuffer(fileName, 8, 4 * 8, true, autoLoad: true)) {

				for (var i = 0; i < 256; i++)
					Assert.AreEqual(expected[i], binaryFile[i]);
			}
		}
	}

	public void GetRandomRange(Random rng, int count, out int startIX, out int endIX) {
		var index1 = rng.Next(0, count);
		var index2 = rng.Next(0, count);
		startIX = Math.Min(index1, index2);
		endIX = Math.Max(index1, index2);
	}

	[Test]
	[Sequential]
	public void IntegrationTests(
		[Values(1, 10, 57, 173, 1111)] int maxCapacity,
		[Values(1, 1, 3, 31, 13)] int pageSize,
		[Values(1, 1, 7, 2, 19)] int maxOpenPages) {
		var fileName = Tools.FileSystem.GetTempFileName(true);
		using (Tools.Scope.ExecuteOnDispose(() => File.Delete(fileName))) {
			using (var binaryFile = new FileMappedBuffer(fileName, pageSize, maxOpenPages * pageSize, false, autoLoad: true)) {
				AssertEx.ListIntegrationTest<byte>(binaryFile, maxCapacity, (rng, i) => rng.NextBytes(i), mutateFromEndOnly: true);
			}
		}
	}
}
