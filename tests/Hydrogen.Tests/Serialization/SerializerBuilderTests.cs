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
			.Serialize(x => x.A, StringSerializer.UTF8.AsNullable())
			.Serialize(x => x.B, PrimitiveSerializer<int>.Instance)
			.Serialize(x => x.C, PrimitiveSerializer<bool>.Instance)
			.Build();
		
		var testObj = new TestObject("Hello", 123, true);
		
		var serialized = serializer.SerializeBytesLE(testObj);
		var deserialized = serializer.DeserializeBytesLE(serialized);

		Assert.That(deserialized, Is.EqualTo(testObj).Using(new TestObjectEqualityComparer()));
			
	}

	[Test]
	public void AutoBuildConsistency_Simple() {
		// test object
		var serializer1 = SerializerBuilder
			.For<TestObject>()
			.Serialize(x => x.A, StringSerializer.UTF8.AsNullable())
			.Serialize(x => x.B, PrimitiveSerializer<int>.Instance)
			.Serialize(x => x.C, PrimitiveSerializer<bool>.Instance)
			.Build();

		var serializer2 = SerializerBuilder.AutoBuild<TestObject>();

		var testObj1 = new TestObject("Hello", 123, true);
		var testObj2 = new TestObject("", -1, false);
		var testObj3 = new TestObject(null, 0, true);

		var serialized1_1 = serializer1.DeserializeBytesLE(serializer1.SerializeBytesLE(testObj1));
		var serialized1_2 = serializer1.DeserializeBytesLE(serializer1.SerializeBytesLE(testObj2));
		var serialized1_3 = serializer1.DeserializeBytesLE(serializer1.SerializeBytesLE(testObj3));
		
		var serialized2_1 = serializer2.DeserializeBytesLE(serializer2.SerializeBytesLE(testObj1));
		var serialized2_2 = serializer2.DeserializeBytesLE(serializer2.SerializeBytesLE(testObj2));
		var serialized2_3 = serializer2.DeserializeBytesLE(serializer2.SerializeBytesLE(testObj3));

		Assert.That(serialized1_1, Is.EqualTo(serialized2_1).Using(new TestObjectEqualityComparer()));
		Assert.That(serialized1_2, Is.EqualTo(serialized2_2).Using(new TestObjectEqualityComparer()));
		Assert.That(serialized1_3, Is.EqualTo(serialized2_3).Using(new TestObjectEqualityComparer()));
		
	}

	[Test]
	public void AutoBuildComplex() {
		// test object
		var serializer = SerializerBuilder.AutoBuild<ComplexObject>();
		var obj = new ComplexObject {
			TestProperty = new TestObject("Hello", 123, true),
			ObjectProperty = new KeyValuePair<string, TestObject>("Hello", new TestObject("Hello", 123, true)),
			NullableEnumProperty = CrudAction.Create,
			ManyRecursiveProperty = null
		};
		var serialized = serializer.SerializeBytesLE(obj);
		var deserialized = serializer.DeserializeBytesLE(serialized);
		var comparer = new ComplexObjectEqualityComparer(
			new KeyValuePairEqualityComparer<string, TestObject>(
				StringComparer.InvariantCulture, 
				new TestObjectEqualityComparer()
			)
			.AsProjection<KeyValuePair<string, TestObject>, object>(x => x, x => (KeyValuePair<string, TestObject>)x)
		);
		Assert.That(deserialized, Is.EqualTo(obj).Using(comparer));
	}


	[Test]
	public void TestObject_2() {
		// Test with null string field
		var serializer = SerializerBuilder
			.For<TestObject>()
			.Serialize(x => x.A, StringSerializer.UTF8.AsNullable())
			.Serialize(x => x.B, PrimitiveSerializer<int>.Instance)
			.Serialize(x => x.C, PrimitiveSerializer<bool>.Instance)
			.Build();

		
		var testObj = new TestObject(null, 999, false);
		
		var serialized = serializer.SerializeBytesLE(testObj);
		var deserialized = serializer.DeserializeBytesLE(serialized);

		Assert.That(deserialized, Is.EqualTo(testObj).Using(new TestObjectEqualityComparer()));
			
	}

	[Test]
	public void TestObject_3() {
		// Test with empty string field
		var serializer = SerializerBuilder
			.For<TestObject>()
			.Serialize(x => x.A, StringSerializer.UTF8.AsNullable())
			.Serialize(x => x.B, PrimitiveSerializer<int>.Instance)
			.Serialize(x => x.C, PrimitiveSerializer<bool>.Instance)
			.Build();

		
		var testObj = new TestObject(string.Empty, -1, false);
		
		var serialized = serializer.SerializeBytesLE(testObj);
		var deserialized = serializer.DeserializeBytesLE(serialized);

		Assert.That(deserialized, Is.EqualTo(testObj).Using(new TestObjectEqualityComparer()));
			
	}

	[Test]
	public void SupportsNull_False() {
		// Test with empty string field
		var serializer = SerializerBuilder
			.For<TestObject>()
			.Serialize(x => x.A, StringSerializer.UTF8.AsNullable())
			.Serialize(x => x.B, PrimitiveSerializer<int>.Instance)
			.Serialize(x => x.C, PrimitiveSerializer<bool>.Instance)
			.Build();
		Assert.That(serializer.SupportsNull, Is.False);	
		Assert.That(() => serializer.SerializeBytesLE(null), Throws.Exception);
	}


	[Test]
	public void SupportsNull_True() {
		// Test with empty string field
		var serializer = SerializerBuilder
			.For<TestObject>()
			.AllowNull()
			.Serialize(x => x.A, StringSerializer.UTF8.AsNullable())
			.Serialize(x => x.B, PrimitiveSerializer<int>.Instance)
			.Serialize(x => x.C, PrimitiveSerializer<bool>.Instance)
			.Build();

		Assert.That(serializer.SupportsNull, Is.True);	
		
		var serialized = serializer.SerializeBytesLE(null);
		var deserialized = serializer.DeserializeBytesLE(serialized);

		Assert.That(deserialized, Is.Null);
			
	}
}
