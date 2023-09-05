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
public class PaddedSerializerTests {

	[Test]
	public void TestPaddedSerializer_Empty([Values] SizeDescriptorStrategy sizeDescriptor, [Values("", "01235678910")] string @value ) {
		var serializer = new StringSerializer().AsStaticSizeSerializer(100, sizeDescriptor);
		var bytes = serializer.SerializeLE(@value);
		Assert.AreEqual(100, bytes.Length);
	}

}
