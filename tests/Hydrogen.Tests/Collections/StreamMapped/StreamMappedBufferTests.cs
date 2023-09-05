// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using NUnit.Framework;
using System.IO;
using Hydrogen.Collections;
using Hydrogen.NUnit;

namespace Hydrogen.Tests;

[TestFixture]
[Parallelizable(ParallelScope.Children)]
public class StreamMappedBufferTests {

	[Test]
	public void Insert_1([Values(1, 2, 3, 17, 18, 100)] int blockSize) {
		using var stream = new MemoryStream();
		var buffer = new StreamMappedBuffer(stream, blockSize);
		buffer.AddRange(new byte[] { 1, 2, 10, 11, 12, 13, 14, 15, 16, 17 });
		buffer.InsertRange(2, new byte[] { 3, 4, 5, 6, 7, 8, 9 });
		Assert.That(buffer, Is.EqualTo(new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17 }));
	}


	[Test]
	public void Insert_2([Values(1, 2, 3, 17, 18, 100)] int blockSize) {
		using var stream = new MemoryStream();
		var buffer = new StreamMappedBuffer(stream, blockSize);
		buffer.AddRange(new byte[] { 1, 2, 17 });
		buffer.InsertRange(2, new byte[] { 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16 });
		Assert.That(buffer, Is.EqualTo(new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17 }));
	}


	[Test]
	public void Remove([Values(1, 2, 3, 17, 18, 100)] int blockSize) {
		using var stream = new MemoryStream();
		var buffer = new StreamMappedBuffer(stream, blockSize);
		buffer.AddRange(new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17 });
		buffer.RemoveRange(2, 7);
	}

	[Test]
	public void IntegrationTests(
		[Values(1, 3, 11, 111, HydrogenDefaults.DefaultBufferOperationBlockSize)]
		int blockSize,
		[Values(1, 2, 3, 11, 111, 1111, 11111)]
		int maxCapacity) {
		using var stream = new MemoryStream();
		var buffer = new StreamMappedBuffer(stream, blockSize);
		AssertEx.BufferIntegrationTest(buffer, maxCapacity);
	}
}
