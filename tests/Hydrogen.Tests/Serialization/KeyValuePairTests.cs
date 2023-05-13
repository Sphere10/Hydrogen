// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using AutoFixture;
using FluentAssertions;
using NUnit.Framework;

namespace Hydrogen.Tests {

	[TestFixture]
	[Parallelizable]
	public class KeyValuePairTests {

		[Test]
		public void Simple() {
			var kvp = new KeyValuePair<string, byte[]>("alpha", "alpha".ToAsciiByteArray());
			var comparer = new KeyValuePairEqualityComparer<string, byte[]>(EqualityComparer<string>.Default, new ByteArrayEqualityComparer());
			var serializer = new KeyValuePairSerializer<string, byte[]>(new StringSerializer(Encoding.UTF8), new ByteArraySerializer());
			var serializedBytes = serializer.SerializeLE(kvp);
			var deserializedItem = serializer.DeserializeLE(serializedBytes);
			Assert.That(deserializedItem, Is.EqualTo(kvp).Using(comparer));
		}

		[Test]
		public void NullValue() {
			RunTest(new KeyValuePair<string, byte[]>("alpha", null));
		}

		[Test]
		public void Integration([Values(100)] int iterations) {
			var rng = new Random(31337);
			for (var i = 0; i < iterations; i++) {
				RunTest(new KeyValuePair<string, byte[]>(rng.NextString(0, 100), rng.NextBytes(rng.Next(0,100))));
			}
		}

		private void RunTest(KeyValuePair<string, byte[]> item) {
			var comparer = new KeyValuePairEqualityComparer<string, byte[]>(EqualityComparer<string>.Default, new ByteArrayEqualityComparer());
			var serializer = new KeyValuePairSerializer<string, byte[]>(new StringSerializer(Encoding.UTF8), new ByteArraySerializer());
			var serializedBytes = serializer.SerializeLE(item);
			var deserializedItem = serializer.DeserializeLE(serializedBytes);
			Assert.That(deserializedItem, Is.EqualTo(item).Using(comparer));
		}
	}
}