// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System.Text;
using NUnit.Framework;
using NUnit.Framework.Legacy;

namespace Hydrogen.Tests;

[TestFixture]
[Parallelizable]
public class PaddedSerializerTests {

	[Test]
	public void TestPaddedSerializer_Empty([Values] SizeDescriptorStrategy sizeDescriptor, [Values("", "01235678910")] string @value ) {
		var serializer = new StringSerializer(Encoding.UTF8, sizeDescriptor).AsConstantSize(100);
		var size = serializer.CalculateSize(@value);
		var bytes = serializer.SerializeBytesLE(@value);
		Assert.That(bytes.Length, Is.EqualTo(size));
		ClassicAssert.AreEqual(100, bytes.Length);
	}

}
