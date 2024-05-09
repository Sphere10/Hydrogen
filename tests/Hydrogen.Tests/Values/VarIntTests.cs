// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.IO;
using FluentAssertions;
using NUnit.Framework;
using NUnit.Framework.Legacy;

namespace Hydrogen.Tests.Values;

public class VarIntTests {
	[Test]
	[TestCase(0UL, 1)]
	[TestCase(ulong.MinValue, 1)]
	[TestCase((ulong)0xFC, 1)]
	[TestCase((ulong)0xFD, 3)]
	[TestCase((ulong)0xFFFF, 3)]
	[TestCase((ulong)0xFFFFF, 5)]
	[TestCase((ulong)0xFFFFFFFF, 5)]
	[TestCase((ulong)0xFFFFFFFFF, 9)]
	[TestCase(ulong.MaxValue, 9)]
	public void WriteAndReadStream(ulong value, int expectedByteLength) {
		var stream = new MemoryStream();
		var a = new VarInt(value);
		a.Write(stream);
		ClassicAssert.AreEqual(expectedByteLength, stream.Length);

		stream.Seek(0, SeekOrigin.Begin);
		ulong b = VarInt.Read(stream);
		b.Should().Be(a).And.Be(value);
	}

	[Test]
	[TestCase(ulong.MinValue, 1)]
	[TestCase((ulong)0xFC, 1)]
	[TestCase((ulong)0xFD, 3)]
	[TestCase((ulong)0xFFFF, 3)]
	[TestCase((ulong)0xFFFFF, 5)]
	[TestCase((ulong)0xFFFFFFFF, 5)]
	[TestCase((ulong)0xFFFFFFFFF, 9)]
	[TestCase(ulong.MaxValue, 9)]
	public void ToFromBytes(ulong value, int expectedByteLength) {
		VarInt a = new VarInt(value);
		var bytes = a.ToBytes();
		bytes.Length.Should().Be(expectedByteLength);

		ulong b = VarInt.From(bytes);
		b.Should().Be(a);
	}

	[Test]
	public void ArithmeticOperatorOverloads() {
		((ulong)(new VarInt(1) + new VarInt(1))).Should().Be(2);
		((ulong)(new VarInt(1) + 1)).Should().Be(2);
		((ulong)(new VarInt(1) - new VarInt(1))).Should().Be(0);
		((ulong)(new VarInt(1) - 1)).Should().Be(0);
		((ulong)(new VarInt(10) / new VarInt(3))).Should().Be(3);
		((ulong)(new VarInt(10) / 3)).Should().Be(3);
		((ulong)(new VarInt(1) * 2)).Should().Be(2);
	}


	[Test]
	public void IntegrationTest([Values(1000000)] int iterations) {
		var rng = new Random(31337);
		using var memStream = new MemoryStream();
		for (var i = 0; i < iterations; i++) {
			memStream.SetLength(0);
			ulong val = (uint)rng.Next() + (uint)rng.Next();
			VarInt.Write(val, memStream);
			memStream.Seek(0, SeekOrigin.Begin);
			ClassicAssert.AreEqual(val, VarInt.Read(memStream));
		}
	}

}
