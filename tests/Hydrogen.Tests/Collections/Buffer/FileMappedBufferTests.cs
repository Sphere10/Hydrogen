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
using NUnit.Framework.Legacy;

namespace Hydrogen.Tests;

[TestFixture]
[Parallelizable(ParallelScope.Children)]
public class FileMappedBufferTests {

	[Test]
	public void SingleByteFile_Save() {
		var expected = new byte[] { 127 };
		var fileName = Tools.FileSystem.GetTempFileName(true);
		using (Tools.Scope.ExecuteOnDispose(() => File.Delete(fileName))) {
			using (var binaryFile = new FileMappedBuffer(PagedFileDescriptor.From( fileName, 1, 1))) {

				ClassicAssert.AreEqual(0, binaryFile.Pages.Count());
				binaryFile.AddRange(expected);

				// Check page
				ClassicAssert.AreEqual(1, binaryFile.Pages.Count());
				ClassicAssert.AreEqual(0, binaryFile.Pages[0].StartIndex);
				ClassicAssert.AreEqual(1, binaryFile.Pages[0].Count);
				ClassicAssert.AreEqual(0, binaryFile.Pages[0].EndIndex);
				ClassicAssert.IsTrue(binaryFile.Pages[0].Dirty);
			}
			// Check saved
			ClassicAssert.AreEqual(expected, File.ReadAllBytes(fileName));
		}
	}

	[Test]
	public void SingleByteFile_Load() {
		var expected = new byte[] { 127 };
		var fileName = Tools.FileSystem.GetTempFileName(true);
		Tools.FileSystem.AppendAllBytes(fileName, expected);
		using (Tools.Scope.ExecuteOnDispose(() => File.Delete(fileName))) {
			using (var binaryFile = new FileMappedBuffer(PagedFileDescriptor.From( fileName, 1, 1), FileAccessMode.Read | FileAccessMode.AutoLoad)) {

				// Check page
				ClassicAssert.AreEqual(1, binaryFile.Pages.Count());
				ClassicAssert.AreEqual(0, binaryFile.Pages[0].StartIndex);
				ClassicAssert.AreEqual(1, binaryFile.Pages[0].Count);
				ClassicAssert.AreEqual(0, binaryFile.Pages[0].EndIndex);
				ClassicAssert.IsFalse(binaryFile.Pages[0].Dirty);

				// Check value
				ClassicAssert.AreEqual(expected, binaryFile);
				ClassicAssert.AreEqual(1, binaryFile.Count);
				ClassicAssert.AreEqual(1, binaryFile.CalculateTotalSize());
			}
			// Check file unchanged
			ClassicAssert.AreEqual(expected, File.ReadAllBytes(fileName));
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
			using (var binaryFile = new FileMappedBuffer(PagedFileDescriptor.From (fileName, 1, 1))) {

				binaryFile.AddRange(appendedBytes);

				// Check pages 1 & 2
				ClassicAssert.AreEqual(2, binaryFile.Pages.Count());
				ClassicAssert.IsTrue(binaryFile.Pages[0].State == PageState.Unloaded);
				ClassicAssert.AreEqual(0, binaryFile.Pages[0].StartIndex);
				ClassicAssert.AreEqual(1, binaryFile.Pages[0].Count);
				ClassicAssert.AreEqual(0, binaryFile.Pages[0].EndIndex);
				ClassicAssert.IsFalse(binaryFile.Pages[0].Dirty);

				ClassicAssert.IsTrue(binaryFile.Pages[1].State == PageState.Loaded);
				ClassicAssert.AreEqual(1, binaryFile.Pages[1].StartIndex);
				ClassicAssert.AreEqual(1, binaryFile.Pages[1].Count);
				ClassicAssert.AreEqual(1, binaryFile.Pages[1].EndIndex);
				ClassicAssert.IsTrue(binaryFile.Pages[1].Dirty);

				// Check value
				ClassicAssert.AreEqual(expected, binaryFile);
				ClassicAssert.AreEqual(2, binaryFile.Count);
				ClassicAssert.AreEqual(2, binaryFile.CalculateTotalSize());
			}
			// Check file was saved appended
			ClassicAssert.AreEqual(expected, File.ReadAllBytes(fileName));
		}
	}

	[Test]
	public void NibbleFile_Save() {
		var expected = new byte[] { 127, 17 };
		var fileName = Tools.FileSystem.GetTempFileName(true);
		using (Tools.Scope.ExecuteOnDispose(() => File.Delete(fileName))) {
			using (var binaryFile = new FileMappedBuffer(PagedFileDescriptor.From(fileName, 1, 1))) {

				binaryFile.Add(expected[0]);

				// Check Page 1
				ClassicAssert.AreEqual(1, binaryFile.Pages.Count());
				ClassicAssert.IsTrue(binaryFile.Pages[0].State == PageState.Loaded);
				ClassicAssert.AreEqual(0, binaryFile.Pages[0].StartIndex);
				ClassicAssert.AreEqual(1, binaryFile.Pages[0].Count);
				ClassicAssert.AreEqual(0, binaryFile.Pages[0].EndIndex);
				ClassicAssert.IsTrue(binaryFile.Pages[0].Dirty);

				// Add new page
				binaryFile.Add(expected[1]);

				// Check pages 1 & 2
				ClassicAssert.AreEqual(2, binaryFile.Pages.Count());
				ClassicAssert.IsTrue(binaryFile.Pages[0].State == PageState.Unloaded);
				ClassicAssert.AreEqual(0, binaryFile.Pages[0].StartIndex);
				ClassicAssert.AreEqual(1, binaryFile.Pages[0].Count);
				ClassicAssert.AreEqual(0, binaryFile.Pages[0].EndIndex);
				ClassicAssert.IsFalse(binaryFile.Pages[0].Dirty);

				ClassicAssert.IsTrue(binaryFile.Pages[1].State == PageState.Loaded);
				ClassicAssert.AreEqual(1, binaryFile.Pages[1].StartIndex);
				ClassicAssert.AreEqual(1, binaryFile.Pages[1].Count);
				ClassicAssert.AreEqual(1, binaryFile.Pages[1].EndIndex);
				ClassicAssert.IsTrue(binaryFile.Pages[1].Dirty);
			}

			// Check saved
			ClassicAssert.AreEqual(expected, File.ReadAllBytes(fileName));
		}
	}

	[Test]
	public void Rewind() {
		var expected = new byte[] { 127, 17, 18, 19 };
		var fileName = Tools.FileSystem.GetTempFileName(true);
		using (Tools.Scope.ExecuteOnDispose(() => File.Delete(fileName))) {
			using (var binaryFile = new FileMappedBuffer(PagedFileDescriptor.From(fileName, 1, 1))) {

				binaryFile.AddRange<byte>(127, 16, 15, 14, 13);
				binaryFile.RemoveRange(1, 4);
				ClassicAssert.AreEqual(1, binaryFile.Count);
				ClassicAssert.AreEqual(127, binaryFile[0]);
				binaryFile.AddRange<byte>(17, 18, 19);
				ClassicAssert.AreEqual(4, binaryFile.Count);
				ClassicAssert.AreEqual(expected, binaryFile);
			}
			// Check saved
			ClassicAssert.AreEqual(expected, File.ReadAllBytes(fileName));
		}
	}

	[Test]
	public void NibbleFile_Load() {
		var expected = new byte[] { 127, 17 };
		var fileName = Tools.FileSystem.GetTempFileName(true);
		Tools.FileSystem.AppendAllBytes(fileName, expected);
		using (Tools.Scope.ExecuteOnDispose(() => File.Delete(fileName))) {
			using (var binaryFile = new FileMappedBuffer(PagedFileDescriptor.From(fileName, 1, 1))) {

				// Check pages 1 & 2
				ClassicAssert.AreEqual(2, binaryFile.Pages.Count());
				ClassicAssert.IsTrue(binaryFile.Pages[0].State == PageState.Unloaded);
				ClassicAssert.AreEqual(0, binaryFile.Pages[0].StartIndex);
				ClassicAssert.AreEqual(1, binaryFile.Pages[0].Count);
				ClassicAssert.AreEqual(0, binaryFile.Pages[0].EndIndex);
				ClassicAssert.IsFalse(binaryFile.Pages[0].Dirty);

				ClassicAssert.IsTrue(binaryFile.Pages[1].State == PageState.Unloaded);
				ClassicAssert.AreEqual(1, binaryFile.Pages[1].StartIndex);
				ClassicAssert.AreEqual(1, binaryFile.Pages[1].Count);
				ClassicAssert.AreEqual(1, binaryFile.Pages[1].EndIndex);

				// Check values
				ClassicAssert.AreEqual(expected, binaryFile);
			}
			// Check file unchanged
			ClassicAssert.AreEqual(expected, File.ReadAllBytes(fileName));
		}
	}

	[Test]
	public void ReadOnly_SinglePage() {
		var expected = Enumerable.Range(0, 8).Select(x => (byte)x).ToArray();
		var fileName = Tools.FileSystem.GetTempFileName(true);
		Tools.FileSystem.AppendAllBytes(fileName, expected);
		using (Tools.Scope.ExecuteOnDispose(() => File.Delete(fileName)))
		using (var binaryFile = new FileMappedBuffer(PagedFileDescriptor.From (fileName, 8, 4*8))) {

			for (var i = 0; i < 8; i++)
				ClassicAssert.AreEqual(expected[i], binaryFile[i]);
		}
	}

	[Test]
	public void ReadOnly_MultiPage() {
		var expected = Enumerable.Range(0, 256).Select(x => (byte)x).ToArray();
		var fileName = Tools.FileSystem.GetTempFileName(true);
		Tools.FileSystem.AppendAllBytes(fileName, expected);
		using (Tools.Scope.ExecuteOnDispose(() => File.Delete(fileName)))
		using (var binaryFile = new FileMappedBuffer(PagedFileDescriptor.From(fileName, 8, 4 * 8))) {

			for (var i = 0; i < 256; i++)
				ClassicAssert.AreEqual(expected[i], binaryFile[i]);
		}
	}

	[Test]
	public void ReadOnly_Update() {
		var expected = Enumerable.Range(0, 256).Select(x => (byte)x).ToArray();
		var fileName = Tools.FileSystem.GetTempFileName(true);
		Tools.FileSystem.AppendAllBytes(fileName, expected.Reverse().ToArray());
		using (Tools.Scope.ExecuteOnDispose(() => File.Delete(fileName))) {
			// first load the file and sort them
			using (var binaryFile = new FileMappedBuffer(PagedFileDescriptor.From(fileName, 8, 4 * 8))) {

				QuickSorter.Sort(binaryFile, Comparer<byte>.Default);
				for (var i = 0; i < 256; i++)
					ClassicAssert.AreEqual(expected[i], binaryFile[i]);
			}

			// check file is as expected
			using (var binaryFile = new FileMappedBuffer(PagedFileDescriptor.From(fileName, 8, 4 * 8))) {

				for (var i = 0; i < 256; i++)
					ClassicAssert.AreEqual(expected[i], binaryFile[i]);
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
			using (var binaryFile = new FileMappedBuffer(PagedFileDescriptor.From(fileName, pageSize, maxOpenPages * pageSize))) {
				AssertEx.ListIntegrationTest<byte>(binaryFile, maxCapacity, (rng, i) => rng.NextBytes(i), mutateFromEndOnly: true);
			}
		}
	}
}
