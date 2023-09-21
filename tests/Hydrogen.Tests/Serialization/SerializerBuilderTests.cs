// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace Hydrogen.Tests;

[TestFixture]
[Parallelizable]
public class SerializerBuilderTests {



	[Test]
	public void TestObject_1() {
		// test object
		var serializer = SerializerBuilder
			.For<TestObject>()
			.ForMember(x => x.A, StringSerializer.UTF8.AsNullable().AsAutoSized())
			.ForMember(x => x.B, PrimitiveSerializer<int>.Instance)
			.ForMember(x => x.C, PrimitiveSerializer<bool>.Instance)
			.Build();
		
		var testObj = new TestObject("Hello", 123, true);
		
		var serialized = serializer.SerializeLE(testObj);
		var deserialized = serializer.DeserializeLE(serialized);

		Assert.That(deserialized, Is.EqualTo(testObj).Using(new TestObjectComparer()));
			
	}


	[Test]
	public void TestObject_2() {
		// Test with null string field
		var serializer = SerializerBuilder
			.For<TestObject>()
			.ForMember(x => x.A, StringSerializer.UTF8.AsNullable().AsAutoSized())
			.ForMember(x => x.B, PrimitiveSerializer<int>.Instance)
			.ForMember(x => x.C, PrimitiveSerializer<bool>.Instance)
			.Build();

		
		var testObj = new TestObject(null, 999, false);
		
		var serialized = serializer.SerializeLE(testObj);
		var deserialized = serializer.DeserializeLE(serialized);

		Assert.That(deserialized, Is.EqualTo(testObj).Using(new TestObjectComparer()));
			
	}

	[Test]
	public void TestObject_3() {
		// Test with empty string field
		var serializer = SerializerBuilder
			.For<TestObject>()
			.ForMember(x => x.A, StringSerializer.UTF8.AsNullable().AsAutoSized())
			.ForMember(x => x.B, PrimitiveSerializer<int>.Instance)
			.ForMember(x => x.C, PrimitiveSerializer<bool>.Instance)
			.Build();

		
		var testObj = new TestObject(string.Empty, -1, false);
		
		var serialized = serializer.SerializeLE(testObj);
		var deserialized = serializer.DeserializeLE(serialized);

		Assert.That(deserialized, Is.EqualTo(testObj).Using(new TestObjectComparer()));
			
	}

	[Test]
	public void SupportsNull_False() {
		// Test with empty string field
		var serializer = SerializerBuilder
			.For<TestObject>()
			.ForMember(x => x.A, StringSerializer.UTF8.AsNullable().AsAutoSized())
			.ForMember(x => x.B, PrimitiveSerializer<int>.Instance)
			.ForMember(x => x.C, PrimitiveSerializer<bool>.Instance)
			.Build();
		Assert.That(serializer.SupportsNull, Is.False);	
		Assert.That(() => serializer.SerializeLE(null), Throws.Exception);
	}


	[Test]
	public void SupportsNull_True() {
		// Test with empty string field
		var serializer = SerializerBuilder
			.For<TestObject>()
			.AllowNull()
			.ForMember(x => x.A, StringSerializer.UTF8.AsNullable().AsAutoSized())
			.ForMember(x => x.B, PrimitiveSerializer<int>.Instance)
			.ForMember(x => x.C, PrimitiveSerializer<bool>.Instance)
			.Build();

		Assert.That(serializer.SupportsNull, Is.True);	
		
		var serialized = serializer.SerializeLE(null);
		var deserialized = serializer.DeserializeLE(serialized);

		Assert.That(deserialized, Is.Null);
			
	}
}
