// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Hydrogen.Tests;

public class HashingStreamTests {

	[Test]
	public void TestEmpty([Values(CHF.SHA2_256, CHF.SHA1_160, CHF.Blake2b_128)] CHF chf) {
		using var hashingStream = new HashingStream(chf);
		Assert.That(hashingStream.GetDigest(), Is.EqualTo(Hashers.Hash(chf, Array.Empty<byte>())));
	}

	[Test]
	public void Test1b([Values(CHF.SHA2_256, CHF.SHA1_160, CHF.Blake2b_128)] CHF chf) {
		using var hashingStream = new HashingStream(chf);
		hashingStream.WriteByte(1);
		Assert.That(hashingStream.GetDigest(), Is.EqualTo(Hashers.Hash(chf, new byte[] { 1 })));
	}


	[Test]
	public void Test256b_1([Values(CHF.SHA2_256, CHF.SHA1_160, CHF.Blake2b_128)] CHF chf) {
		var data = Enumerable.Range(0, 256).Select(x => (byte)x).ToArray();
		using var hashingStream = new HashingStream(chf);
		hashingStream.Write(data);
		Assert.That(hashingStream.GetDigest(), Is.EqualTo(Hashers.Hash(chf, data)));
	}

	[Test]
	public void Test256b_2([Values(CHF.SHA2_256, CHF.SHA1_160, CHF.Blake2b_128)] CHF chf) {
		var data = Enumerable.Range(0, 256).Select(x => (byte)x).ToArray();
		using var hashingStream = new HashingStream(chf);
		hashingStream.Write(data, 10, 99);
		Assert.That(hashingStream.GetDigest(), Is.EqualTo(Hashers.Hash(chf, data.Skip(10).Take(99).ToArray())));
	}


	[Test]
	public async Task Test256bAsync_1([Values(CHF.SHA2_256, CHF.SHA1_160, CHF.Blake2b_128)] CHF chf) {
		var data = Enumerable.Range(0, 256).Select(x => (byte)x).ToArray();
		await using var hashingStream = new HashingStream(chf);
		await hashingStream.WriteAsync(data);
		Assert.That(hashingStream.GetDigest(), Is.EqualTo(Hashers.Hash(chf, data)));
	}

	[Test]
	public async Task Test256bAsync_2([Values(CHF.SHA2_256, CHF.SHA1_160, CHF.Blake2b_128)] CHF chf) {
		var data = Enumerable.Range(0, 256).Select(x => (byte)x).ToArray();
		await using var hashingStream = new HashingStream(chf);
		await hashingStream.WriteAsync(data, CancellationToken.None);
		Assert.That(hashingStream.GetDigest(), Is.EqualTo(Hashers.Hash(chf, data)));
	}

	[Test]
	public async Task Test256bAsync_3([Values(CHF.SHA2_256, CHF.SHA1_160, CHF.Blake2b_128)] CHF chf) {
		var data = Enumerable.Range(0, 256).Select(x => (byte)x).ToArray();
		await using var hashingStream = new HashingStream(chf);
		await hashingStream.WriteAsync(data, 10, 99, CancellationToken.None);
		Assert.That(hashingStream.GetDigest(), Is.EqualTo(Hashers.Hash(chf, data.Skip(10).Take(99).ToArray())));
	}

	[Test]
	public async Task TestAll([Values(CHF.SHA2_256, CHF.SHA1_160, CHF.Blake2b_128)] CHF chf) {
		await using var hashingStream = new HashingStream(chf);
		hashingStream.WriteByte(0);
		hashingStream.Write(new byte[] { 1, 2, 3 });
		hashingStream.Write(new byte[] { 0, 0, 4, 5, 6, 0, 0 }, 2, 3);
		await hashingStream.WriteAsync(new byte[] { 7, 8, 9 });
		await hashingStream.WriteAsync(new byte[] { 0, 0, 10, 11, 12, 0, 0 }, 2, 3);
		Assert.That(hashingStream.GetDigest(), Is.EqualTo(Hashers.Hash(chf, new byte[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12 })));
	}
}
