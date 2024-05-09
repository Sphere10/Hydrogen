// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.IO;
using NUnit.Framework;
using Hydrogen.NUnit;
using NUnit.Framework.Legacy;

namespace Hydrogen.Tests;

public class FragmentedStreamTests {

	private Random Random { get; } = new();


	[Test]
	public void ReadByteAndEndReturnsNegOne() {
		var fragments = new ByteArrayStreamFragmentProvider();
		var stream = new FragmentedStream(fragments);
		var expected = new MemoryStream();

		stream.Seek(0, SeekOrigin.End);
		var read = stream.ReadByte();
		Assert.That(read, Is.EqualTo(-1));
	}

	[Test]
	public void Empty() {
		var fragments = new ByteArrayStreamFragmentProvider();
		var stream = new FragmentedStream(fragments);
		var expected = new MemoryStream();

		ClassicAssert.AreEqual(expected.Position, stream.Position);
		ClassicAssert.AreEqual(expected.Length, stream.Length);
		ClassicAssert.AreEqual(expected.ToArray(), stream.ToArray());
	}

	[Test]
	public void WriteEmpty() {
		var fragments = new ByteArrayStreamFragmentProvider();
		var stream = new FragmentedStream(fragments);
		var expected = new MemoryStream();
		var data = Array.Empty<byte>();
		expected.WriteBytes(data);
		stream.WriteBytes(data);
		ClassicAssert.AreEqual(expected.ToArray(), stream.ToArray());
	}

	[Test]
	public void Write([Range(1, 100)] int iteration) {
		var rng = new Random(31337 * iteration);
		var fragments = new ByteArrayStreamFragmentProvider(() => rng.NextBytes(rng.Next(1, 11)));
		var stream = new FragmentedStream(fragments);
		var expected = new MemoryStream();
		var data = rng.NextBytes(rng.Next(0, 100));
		expected.WriteBytes(data);
		stream.WriteBytes(data);
		ClassicAssert.AreEqual(expected.ToArray(), stream.ToArray());
	}

	[Test]
	public void ReadAll() {
		var fragments = new ByteArrayStreamFragmentProvider();
		var stream = new FragmentedStream(fragments);
		var expected = new MemoryStream();
		var data = Random.NextBytes(299);

		stream.WriteBytes(data);
		expected.WriteBytes(data);

		stream.Seek(0, SeekOrigin.Begin);
		expected.Seek(0, SeekOrigin.Begin);

		ClassicAssert.AreEqual(expected.Position, stream.Position);
		ClassicAssert.AreEqual(expected.Length, stream.Length);

		ClassicAssert.AreEqual(expected.ReadBytes(data.Length), stream.ReadBytes(data.Length));
	}

	[Test]
	public void WriteUpdateAndAdd() {
		var fragments = new ByteArrayStreamFragmentProvider();
		var stream = new FragmentedStream(fragments);
		var expected = new MemoryStream();
		var data = Random.NextBytes(4);

		stream.WriteBytes(data);
		expected.WriteBytes(data);

		ClassicAssert.AreEqual(expected.Position, stream.Position);
		ClassicAssert.AreEqual(expected.Length, stream.Length);
		ClassicAssert.AreEqual(expected.ToArray(), stream.ToArray());

		int halfway = data.Length / 2;
		byte[] newData = Random.NextBytes(data.Length);

		stream.Seek(halfway, SeekOrigin.Begin);
		expected.Seek(halfway, SeekOrigin.Begin);
		ClassicAssert.AreEqual(expected.Position, stream.Position);

		stream.WriteBytes(newData);
		expected.WriteBytes(newData);

		ClassicAssert.AreEqual(expected.Position, stream.Position);
		ClassicAssert.AreEqual(expected.Length, stream.Length);
		ClassicAssert.AreEqual(expected.ToArray(), stream.ToArray());
	}

	[Test]
	[TestCase(SeekOrigin.Begin, 11)]
	[TestCase(SeekOrigin.End, -11)]
	[TestCase(SeekOrigin.Current, 11)]
	public void SeekAndRead(SeekOrigin origin, int offset) {
		var fragments = new ByteArrayStreamFragmentProvider();
		var stream = new FragmentedStream(fragments);
		var expected = new MemoryStream();
		var data = Random.NextBytes(50);

		stream.WriteBytes(data);
		expected.WriteBytes(data);
		stream.Seek(0, SeekOrigin.Begin);
		expected.Seek(0, SeekOrigin.Begin);

		stream.Seek(offset, origin);
		expected.Seek(offset, origin);

		ClassicAssert.AreEqual(expected.Position, stream.Position);
		ClassicAssert.AreEqual(expected.ReadBytes(1), stream.ReadBytes(1));
	}

	[Test]
	public void RemoveAll() {
		var fragments = new ByteArrayStreamFragmentProvider();
		var stream = new FragmentedStream(fragments);
		var expected = new MemoryStream();
		var data = Random.NextBytes(50);

		stream.WriteBytes(data);
		expected.WriteBytes(data);

		stream.SetLength(0);
		expected.SetLength(0);

		ClassicAssert.AreEqual(expected.Position, stream.Position);
		ClassicAssert.AreEqual(expected.Length, stream.Length);
		ClassicAssert.AreEqual(expected.ToArray(), stream.ToArray());
	}

	[Test]
	public void IncreaseSize() {
		var fragments = new ByteArrayStreamFragmentProvider();
		var stream = new FragmentedStream(fragments);
		var expected = new MemoryStream();
		var data = Random.NextBytes(Random.Next(1, 99));

		stream.WriteBytes(data);
		expected.WriteBytes(data);

		int newSpace = Random.Next(1, 250);
		stream.SetLength(stream.Length + newSpace);
		expected.SetLength(expected.Length + newSpace);

		ClassicAssert.AreEqual(expected.Position, stream.Position);
		ClassicAssert.AreEqual(expected.Length, stream.Length);
		ClassicAssert.AreEqual(expected.ToArray(), stream.ToArray());
	}

	[Test]
	public void DecreaseSize([Range(1, 1000)] int iteration) {
		var rng = new Random(31337 * iteration);
		var fragments = new ByteArrayStreamFragmentProvider();
		var stream = new FragmentedStream(fragments);
		var expected = new MemoryStream();
		var data = rng.NextBytes(rng.Next(0, 99));

		stream.WriteBytes(data);
		expected.WriteBytes(data);


		ClassicAssert.AreEqual(expected.Length, stream.Length);
		ClassicAssert.AreEqual(expected.Position, stream.Position);

		int newSpace = Random.Next(0, (int)expected.Length);
		expected.SetLength(expected.Length - newSpace);
		stream.SetLength(stream.Length - newSpace);


		ClassicAssert.AreEqual(expected.Position, stream.Position);
		ClassicAssert.AreEqual(expected.Length, stream.Length);
		ClassicAssert.AreEqual(expected.ToArray(), stream.ToArray());
	}

	[Test]
	public void WriteLastByteOfFragment() {
		var fragmentProvider = new ByteArrayStreamFragmentProvider(2);
		var stream = new FragmentedStream(fragmentProvider);
		Assert.That(stream.Position, Is.EqualTo(0));

		stream.WriteBytes(new byte[] { 0 });
		Assert.That(stream.Position, Is.EqualTo(1));
		Assert.That(fragmentProvider.FragmentCount, Is.EqualTo(1));

		stream.WriteBytes(new byte[] { 1 });
		Assert.That(stream.Position, Is.EqualTo(2));
		Assert.That(fragmentProvider.FragmentCount, Is.EqualTo(1));

		stream.WriteBytes(new byte[] { 2 });
		Assert.That(stream.Position, Is.EqualTo(3));
		Assert.That(fragmentProvider.FragmentCount, Is.EqualTo(2));
	}

	[Test]
	public void WriteBytesCorrectly() {
		var fragmentProvider = new ByteArrayStreamFragmentProvider(3);
		var stream = new FragmentedStream(fragmentProvider);
		stream.WriteBytes(new byte[] { 0 });
		Assert.That(stream.ToArray(), Is.EqualTo(new byte[] { 0 }).Using(ByteArrayEqualityComparer.Instance));

		stream.WriteBytes(new byte[] { 1 });
		Assert.That(stream.ToArray(), Is.EqualTo(new byte[] { 0, 1 }).Using(ByteArrayEqualityComparer.Instance));

		stream.WriteBytes(new byte[] { 2 });
		Assert.That(stream.ToArray(), Is.EqualTo(new byte[] { 0, 1, 2 }).Using(ByteArrayEqualityComparer.Instance));

		stream.WriteBytes(new byte[] { 3 });
		Assert.That(stream.ToArray(), Is.EqualTo(new byte[] { 0, 1, 2, 3 }).Using(ByteArrayEqualityComparer.Instance));

		stream.WriteBytes(new byte[] { 4 });
		Assert.That(stream.ToArray(), Is.EqualTo(new byte[] { 0, 1, 2, 3, 4 }).Using(ByteArrayEqualityComparer.Instance));

		stream.WriteBytes(new byte[] { 5 });
		Assert.That(stream.ToArray(), Is.EqualTo(new byte[] { 0, 1, 2, 3, 4, 5 }).Using(ByteArrayEqualityComparer.Instance));

		stream.WriteBytes(new byte[] { 6 });
		Assert.That(stream.ToArray(), Is.EqualTo(new byte[] { 0, 1, 2, 3, 4, 5, 6 }).Using(ByteArrayEqualityComparer.Instance));

		stream.WriteBytes(new byte[] { 7 });
		Assert.That(stream.ToArray(), Is.EqualTo(new byte[] { 0, 1, 2, 3, 4, 5, 6, 7 }).Using(ByteArrayEqualityComparer.Instance));

	}

	[Test]
	public void SetLengthForward() {
		var fragmentProvider = new ByteArrayStreamFragmentProvider(3);
		var stream = new FragmentedStream(fragmentProvider);
		stream.WriteBytes(new byte[] { 0, 1, 2, 3 });
		stream.SetLength(5);
		Assert.That(stream.ToArray(), Is.EqualTo(new byte[] { 0, 1, 2, 3, 0 }).Using(ByteArrayEqualityComparer.Instance));
	}

	[Test]
	public void SetLengthBackward() {
		var fragmentProvider = new ByteArrayStreamFragmentProvider(3);
		var stream = new FragmentedStream(fragmentProvider);
		stream.WriteBytes(new byte[] { 0, 1, 2, 3 });
		stream.SetLength(2);
		Assert.That(stream.ToArray(), Is.EqualTo(new byte[] { 0, 1 }).Using(ByteArrayEqualityComparer.Instance));
	}

	[Test]
	public void SetLengthBackwardThenForward() {
		var fragmentProvider = new ByteArrayStreamFragmentProvider(3);
		var stream = new FragmentedStream(fragmentProvider);
		stream.WriteBytes(new byte[] { 0, 1, 2, 3 });
		Assert.That(fragmentProvider.TotalBytes, Is.EqualTo(6));
		stream.SetLength(2);
		stream.SetLength(4);
		Assert.That(fragmentProvider.TotalBytes, Is.EqualTo(6));
		Assert.That(fragmentProvider.FragmentCount, Is.EqualTo(2));
		Assert.That(stream.ToArray(), Is.EqualTo(new byte[] { 0, 1, 0, 0 }).Using(ByteArrayEqualityComparer.Instance));
	}

	[Test]
	public void SetLengthBackwardThenForward_SameFragment() {
		var fragmentProvider = new ByteArrayStreamFragmentProvider(4);
		var stream = new FragmentedStream(fragmentProvider);
		stream.WriteBytes(new byte[] { 0, 1, 2, 3 });
		Assert.That(fragmentProvider.TotalBytes, Is.EqualTo(4));
		stream.SetLength(3);
		stream.SetLength(4);
		Assert.That(fragmentProvider.TotalBytes, Is.EqualTo(4));
		Assert.That(fragmentProvider.FragmentCount, Is.EqualTo(1));
		Assert.That(stream.ToArray(), Is.EqualTo(new byte[] { 0, 1, 2, 0 }).Using(ByteArrayEqualityComparer.Instance));
	}

	[Test]
	public void Position_SetEnd() {
		var fragmentProvider = new ByteArrayStreamFragmentProvider();
		using var stream = new FragmentedStream(fragmentProvider);
		stream.WriteBytes(new byte[] { 0, 1, 2, 3 });
		stream.Position = 0;
		Assert.That(stream.Position, Is.EqualTo(0));
		stream.Position = 4;
		Assert.That(stream.Position, Is.EqualTo(4));
	}


	[Test]
	public void SeekEnd() {
		var fragmentProvider = new ByteArrayStreamFragmentProvider();
		using var stream = new FragmentedStream(fragmentProvider);
		stream.WriteBytes(new byte[] { 0, 1, 2, 3 });
		stream.Position = 0;
		Assert.That(stream.Position, Is.EqualTo(0));
		stream.Seek(4L, SeekOrigin.Begin);
		Assert.That(stream.Position, Is.EqualTo(4));
	}

	[Test]
	public void IntegrationTests_ConstantSizeFragments([Values(0, 3, 111, 1371)] int maxSize, [Values(3, 11, 10000)] int fragmentSize) {
		var rng = new Random(31337);
		using var stream = new FragmentedStream(new ByteArrayStreamFragmentProvider(fragmentSize));
		AssertEx.StreamIntegrationTests(maxSize, stream, RNG: rng);
	}

	[Test]
	public void IntegrationTests_DynamicSizeFragments([Values(0, 3, 111, 1371)] int maxSize) {
		var rng = new Random(31337);
		using var stream = new FragmentedStream(new ByteArrayStreamFragmentProvider(() => new byte[rng.Next(1, 100)]));
		AssertEx.StreamIntegrationTests(maxSize, stream, RNG: rng);
	}

}
