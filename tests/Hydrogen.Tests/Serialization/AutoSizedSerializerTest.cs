// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Text;
using NUnit.Framework;

namespace Hydrogen.Tests;

[TestFixture]
[Parallelizable]
public class AutoSizedSerializerTest {

	[Test]
	public void IntegrationTest(
		[Values(0, 1, 11, 17, byte.MaxValue, byte.MaxValue + 1, ushort.MaxValue, ushort.MaxValue + 1)]
		int stringSize,
		[Values] SizeDescriptorStrategy strategy
	) {
		var rng = new Random(31337);
		var @string = rng.NextString(stringSize);
		var serializer = new StringSerializer(Encoding.ASCII, strategy);

		var expectThrow =
			stringSize > byte.MaxValue && strategy == SizeDescriptorStrategy.UseByte ||
			stringSize > ushort.MaxValue && strategy == SizeDescriptorStrategy.UseUInt16;

		if (expectThrow) {
			Assert.That(() => serializer.SerializeBytesLE(@string), Throws.InstanceOf<ArgumentOutOfRangeException>());
		} else {
			var size = serializer.CalculateSize(@string);
			var serializedBytes = serializer.SerializeBytesLE(@string);
			var deserializedItem = serializer.DeserializeBytesLE(serializedBytes);

			Assert.That(serializedBytes.Length, Is.EqualTo(size));
			Assert.That(deserializedItem, Is.EqualTo(@string));
		}
	}

	[Test]
	public void StringTooLargeForByteDescriptor() {
		var rng = new Random(31337);
		var @string = rng.NextString(256);
		var serializer = new StringSerializer(Encoding.ASCII, SizeDescriptorStrategy.UseByte);
		Assert.That(() => serializer.SerializeBytesLE(@string), Throws.InstanceOf<ArgumentOutOfRangeException>());
	}

}
