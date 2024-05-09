// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using NUnit.Framework.Legacy;

namespace Hydrogen.Tests;

[TestFixture]
[Parallelizable(ParallelScope.Children)]
public class MiscellaneousTests {


	[Test]
	public void InterpolateNullAssumption() {
		ClassicAssert.AreEqual(string.Empty, $"{null}");
	}

	[Test]
	public void MemoryStreamSetLengthClearsOldBytes() {
		var rng = new Random(31337);
		for (var i = 0; i < 1000; i++) {
			using var stream = new MemoryStream();
			var data = rng.NextBytes(i);
			stream.Write(data);
			for (var j = i; j >= 0; j--) {
				var bytes = stream.ToArray();
				stream.Position = rng.Next(0, (int)stream.Length);
				stream.SetLength(j);
				stream.SetLength(i);
				// j-i bytes should be 0
				Assert.That(stream.ToArray().AsSpan(^(i - j)).ToArray().All(b => b == 0));
				// reset stream
				stream.Position = 0;
				stream.Write(data);

			}
		}
	}

	[Test]
	public void StandardBehaviour_ListGetRangeNotSupportOverflow() {
		var list = new List<int>();
		list.AddRange(new[] { 1, 2, 3 });
		Assert.That(() => list.GetRange(1, 3), Throws.InstanceOf<ArgumentException>());
	}

	[Test]
	public void StandardBehaviour_ListRemoveRangeNotSupportOverflow() {
		var list = new List<int>();
		list.AddRange(new[] { 1, 2, 3 });
		Assert.That(() => list.RemoveRange(1, 3), Throws.InstanceOf<ArgumentException>());
	}

	[Test]
	public void StandardBehaviour_ListInsertRangeThrowsOnNull() {
		var list = new List<int>();
		Assert.That(() => list.InsertRange(0, null), Throws.InstanceOf<ArgumentException>());
	}

	[Test]
	public void StandardBehaviour_FileMode_Open_DoesNotTruncate() {
		// append 100b to a file
		// open a file stream overwrite
		// append 50b
		// close stream
		// check file is 100b

		var rng = new Random(31337);
		var file = Tools.FileSystem.GetTempFileName(true);
		var _ = Tools.Scope.DeleteFileOnDispose(file);
		Tools.FileSystem.AppendAllBytes(file, rng.NextBytes(100));
		Assert.That(Tools.FileSystem.GetFileSize(file), Is.EqualTo(100));
		using (var stream = File.Open(file, FileMode.Open, FileAccess.Write))
			stream.Write(rng.NextBytes(50));
		Assert.That(Tools.FileSystem.GetFileSize(file), Is.EqualTo(100));
	}

	[Test]
	public void StandardBehaviour_FileMode_Truncate() {
		// append 100b to a file
		// open a file stream overwrite
		// append 50b
		// close stream
		// check file is 50b

		var rng = new Random(31337);
		var file = Tools.FileSystem.GetTempFileName(true);
		var _ = Tools.Scope.DeleteFileOnDispose(file);
		Tools.FileSystem.AppendAllBytes(file, rng.NextBytes(100));
		Assert.That(Tools.FileSystem.GetFileSize(file), Is.EqualTo(100));
		using (var stream = File.Open(file, FileMode.Truncate, FileAccess.Write))
			stream.Write(rng.NextBytes(50));
		Assert.That(Tools.FileSystem.GetFileSize(file), Is.EqualTo(50));
	}

	[Test]
	public void StandardBehaviour_FileOpenWrite_DoesNotTruncate() {
		// append 100b to a file
		// open a file stream overwrite
		// append 50b
		// close stream
		// check file is 100b

		var rng = new Random(31337);
		var file = Tools.FileSystem.GetTempFileName(true);
		var _ = Tools.Scope.DeleteFileOnDispose(file);
		Tools.FileSystem.AppendAllBytes(file, rng.NextBytes(100));
		Assert.That(Tools.FileSystem.GetFileSize(file), Is.EqualTo(100));
		using (var stream = File.OpenWrite(file))
			stream.Write(rng.NextBytes(50));
		Assert.That(Tools.FileSystem.GetFileSize(file), Is.EqualTo(100));
	}

	[Test]
	public void StandardBehaviour_StreamAllowsTipCursor() {
		var rng = new Random(31337);
		using Stream stream = new MemoryStream();
		Assert.That(stream.Length, Is.EqualTo(0));
		Assert.That(stream.Position, Is.EqualTo(0));
		stream.Write(rng.NextBytes(100));
		Assert.That(stream.Position, Is.EqualTo(100));
	}

	[Test]
	public void StandardBehaviour_WhenAllDoesntAbandonAfterSingleFailure() {
		var rng = new Random(31337);
		using Stream stream = new MemoryStream();

		var ran1 = false;
		var ran2 = false;

		async Task Task1() {
			await Task.Delay(100);
			ran1 = true;
			Guard.Ensure(false, "Exception");
		}

		;

		async Task Task2() {
			while (true) {
				await Task.Delay(100);
			}
			ran2 = true;
		}

		Assert.That(() => Task.WhenAll(Task1(), Task2()).WithTimeout(250), Throws.InstanceOf<TaskCanceledException>());
		Assert.That(ran1, Is.True);
		Assert.That(ran2, Is.False);
	}

	[Test]
	public void StandardBehaviour_WhenAllDoesntThrowAfterSingleFailure() {
		var rng = new Random(31337);
		using Stream stream = new MemoryStream();

		var ran1 = false;
		var ran2 = false;

		async Task Task1() {
			await Task.Delay(100);
			ran1 = true;
			Guard.Ensure(false, "Exception");
		}

		;

		async Task Task2() {
			await Task.Delay(150);
			ran2 = true;
		}

		Assert.That(() => Task.WhenAll(Task1(), Task2()).WithTimeout(250), Throws.Nothing);
		Assert.That(ran1, Is.True);
		Assert.That(ran2, Is.True);
	}

	[Test]
	public void StandardBehaviour_WhenAllDoesntThrowAfterAllFailure() {
		var rng = new Random(31337);
		using Stream stream = new MemoryStream();

		var ran1 = false;
		var ran2 = false;

		async Task Task1() {
			await Task.Delay(100);
			ran1 = true;
			Guard.Ensure(false, "Exception");
		}

		async Task Task2() {
			await Task.Delay(100);
			ran2 = true;
			Guard.Ensure(false, "Exception");
		}

		Assert.That(() => Task.WhenAll(Task1(), Task2()).WithTimeout(250), Throws.Nothing);
		Assert.That(ran1, Is.True);
		Assert.That(ran2, Is.True);
	}


	[Test]
	public async Task StandardBehaviour_WhenAnyAbandonsAfterSingleFailure() {
		var rng = new Random(31337);
		using Stream stream = new MemoryStream();

		var task1 = Task.Factory.StartNew(() => {
			Thread.Sleep(100);
			Guard.Ensure(false, "Exception");
		});
		var task2 = Task.Factory.StartNew(() => {
			while (true) {
				Thread.Sleep(100);
			}
		});

		await Task.WhenAny(task1, task2);
		Assert.That(task1.Exception, Is.Not.Null);
		Assert.That(task1.Exception.InnerExceptions.Count, Is.EqualTo(1));
		Assert.That(task1.Exception.InnerExceptions[0], Is.TypeOf<InvalidOperationException>());
		Assert.That(task2.Exception, Is.Null);

	}

	[Test]
	public void StandardBehaviour_OutOfBoundsIntCastDoesntThrow() {
		var x = (long)int.MaxValue + 1;
		Assert.That(() => (int)x, Throws.Nothing);
	}


	[Test]
	public async Task AsyncTaskIgnoringExceptionsWorks() {

		await SomeTaskAsync().IgnoringExceptions();

		async Task SomeTaskAsync() {
			throw new InvalidOperationException("Should never be seen");
		}
	}


}
