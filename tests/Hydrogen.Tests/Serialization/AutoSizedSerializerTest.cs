// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System.Text;
using NUnit.Framework;

namespace Hydrogen.Tests;

[TestFixture]
[Parallelizable]
public class AutoSizedSerializerTest {

	[Test]
	public void String([Values("", "A", "Hello World!")] string arg) {
		var serializer = new AutoSizedSerializer<string>(new StringSerializer(Encoding.UTF8));
		var serializedBytes = serializer.SerializeLE(arg);
		var deserializedItem = serializer.DeserializeLE(serializedBytes);
		Assert.That(deserializedItem, Is.EqualTo(arg));
	}

}
