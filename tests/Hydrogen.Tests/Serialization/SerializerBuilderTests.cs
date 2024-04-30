// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Collections.Generic;
using FastSerialization;
using FluentAssertions;
using NUnit.Framework;

namespace Hydrogen.Tests;

[TestFixture]
[Parallelizable]
public class SerializerBuilderTests {

	public class SingleNullableEnumPropertyClass {
		public CrudAction? Property { get; set; }
	}


	public class SingleObjectPropertyClass {
		public object Property { get; set; }
	}

	public class TwoPropertyObject {
		public object Prop1 { get; set; }

		public object Prop2 { get; set; }
	}

	public class TransientPropertyObject {

		public string Prop1 { get; set; }

		[Transient]
		public int TransientInt { get; set; }

		[Transient]
		public string TransientString { get; set ; }

		[Transient]
		public TransientPropertyObject Nested { get; set; }
	}

	[Test]
	public void TestObject_1() {
		// test object
		var serializer = SerializerBuilder
			.For<TestObject>()
			.Serialize(x => x.A, StringSerializer.UTF8.AsReferenceSerializer())
			.Serialize(x => x.B, PrimitiveSerializer<int>.Instance)
			.Serialize(x => x.C, PrimitiveSerializer<bool>.Instance)
			.Build();
		
		var testObj = new TestObject("Hello", 123, true);
		
		var serialized = serializer.SerializeBytesLE(testObj);
		var deserialized = serializer.DeserializeBytesLE(serialized);

		Assert.That(deserialized, Is.EqualTo(testObj).Using(new TestObjectEqualityComparer()));
			
	}

	[Test]
	public void TestObject_1_CalculateSize() {
		// test object
		var serializer = SerializerBuilder
			.For<TestObject>()
			.Serialize(x => x.A, StringSerializer.UTF8.AsReferenceSerializer())
			.Serialize(x => x.B, PrimitiveSerializer<int>.Instance)
			.Serialize(x => x.C, PrimitiveSerializer<bool>.Instance)
			.Build();
		
		var testObj = new TestObject("Hello", 123, true);
		var size = serializer.CalculateSize(testObj);
		var serialized = serializer.SerializeBytesLE(testObj);
		Assert.That(size, Is.EqualTo(serialized.Length));
	}

	[Test]
	public void AutoBuildConsistency_Simple() {
		// test object
		var serializer1 = SerializerBuilder
			.For<TestObject>()
			.Serialize(x => x.A, StringSerializer.UTF8.AsReferenceSerializer())
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
	public void AutoBuildComplex_CalculateSize() {
		var serializer = SerializerBuilder.AutoBuild<ComplexObject>();
		var obj = new ComplexObject {
			TestProperty = new TestObject("Hello", 123, true),
			ObjectProperty = new KeyValuePair<string, TestObject>("Hello", new TestObject("Hello", 123, true)),
			NullableEnumProperty = CrudAction.Create,
			ManyRecursiveProperty = null
		};

		var size = serializer.CalculateSize(obj);
		var serialized = serializer.SerializeBytesLE(obj);
		Assert.That(size, Is.EqualTo(serialized.Length));
	}

	[Test]
	public void AutoBuildComplex_2() {
		var factory = new SerializerFactory(SerializerFactory.Default);
		factory.RegisterAutoBuild<TestObject>();

		var serializer = SerializerBuilder.AutoBuild<ComplexObject>(factory);
		var obj = new ComplexObject {
			TestProperty = null,
			ObjectProperty = new KeyValuePair<string, TestObject>("Hello", new TestObject(null, 123, true)),
			NullableEnumProperty = null,
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
	public void AutoBuildComplex_2_CalculateSize() {
		var factory = new SerializerFactory(SerializerFactory.Default);
		factory.RegisterAutoBuild<TestObject>();

		var serializer = SerializerBuilder.AutoBuild<ComplexObject>(factory);
		var obj = new ComplexObject {
			TestProperty = null,
			ObjectProperty = new KeyValuePair<string, TestObject>("Hello", new TestObject(null, 123, true)),
			NullableEnumProperty = null,
			ManyRecursiveProperty = null
		};

		var size = serializer.CalculateSize(obj);
		var serialized = serializer.SerializeBytesLE(obj);
		Assert.That(size, Is.EqualTo(serialized.Length));
	}

	[Test]
	public void AutoBuildComplex_Cyclic() {
		var factory = new SerializerFactory(SerializerFactory.Default);
		factory.RegisterAutoBuild<TestObject>();

		var serializer = SerializerBuilder.AutoBuild<ComplexObject>(factory);
		var testObj = new TestObject(null, 123, true);
		var obj = new ComplexObject {
			TestProperty = testObj,
			ObjectProperty = new KeyValuePair<string, TestObject>("Hello", testObj),
			NullableEnumProperty = null,
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
		Assert.That(deserialized.TestProperty, Is.SameAs(((KeyValuePair<string, TestObject>)deserialized.ObjectProperty).Value));
	}

	[Test]
	public void AutoBuildComplex_Cyclic_CalculateSize() {
		var factory = new SerializerFactory(SerializerFactory.Default);
		factory.RegisterAutoBuild<TestObject>();

		var serializer = SerializerBuilder.AutoBuild<ComplexObject>(factory);
		var testObj = new TestObject(null, 123, true);
		var obj = new ComplexObject {
			TestProperty = testObj,
			ObjectProperty = new KeyValuePair<string, TestObject>("Hello", testObj),
			NullableEnumProperty = null,
			ManyRecursiveProperty = null
		};
		var size = serializer.CalculateSize(obj);
		var serialized = serializer.SerializeBytesLE(obj);
		Assert.That(size, Is.EqualTo(serialized.Length));
	}

	[Test]
	public void BugCase_Cyclic1() {
		var factory = new SerializerFactory(SerializerFactory.Default);
		factory.RegisterAutoBuild<TwoPropertyObject>();
		var serializer = SerializerBuilder.AutoBuild<TwoPropertyObject>(factory).AsReferenceSerializer();
		var obj = new TwoPropertyObject {
			Prop1 = "Hello"
		};
		obj.Prop2 = obj;
		var serialized = serializer.SerializeBytesLE(obj);
		var deserialized = serializer.DeserializeBytesLE(serialized);
		Assert.That(deserialized.Prop1, Is.EqualTo("Hello"));
		Assert.That(deserialized.Prop2, Is.SameAs(deserialized));
	}

	
	[Test]
	public void BugCase_Cyclic1_CalculateSize() {
		var factory = new SerializerFactory(SerializerFactory.Default);
		factory.RegisterAutoBuild<TwoPropertyObject>();
		var serializer = SerializerBuilder.AutoBuild<TwoPropertyObject>(factory).AsReferenceSerializer();
		var obj = new TwoPropertyObject {
			Prop1 = "Hello"
		};
		obj.Prop2 = obj;
		var size = serializer.CalculateSize(obj);
		var serialized = serializer.SerializeBytesLE(obj);
		Assert.That(size, Is.EqualTo(serialized.Length));
	}


	[Test]
	public void BugCase_Cyclic_2() {
		var factory = new SerializerFactory(SerializerFactory.Default);
		factory.RegisterAutoBuild<TwoPropertyObject>();
		var serializer = SerializerBuilder.AutoBuild<TwoPropertyObject>(factory).AsReferenceSerializer();
		var obj = new TwoPropertyObject {
			Prop1 = "Hello",
			Prop2 = new TwoPropertyObject {
				Prop1 = "Hello",
				Prop2 = new List<string> { "Test1", "test2" }
			}
		};
		var serialized = serializer.SerializeBytesLE(obj);
		var deserialized = serializer.DeserializeBytesLE(serialized);
		Assert.That(deserialized.Prop1, Is.EqualTo("Hello"));
		Assert.That(deserialized.Prop2, Is.Not.Null);
		Assert.That(deserialized.Prop2, Is.TypeOf<TwoPropertyObject>());
		var deserialized2 = (TwoPropertyObject)deserialized.Prop2;
		Assert.That(deserialized2.Prop1, Is.EqualTo("Hello"));
		Assert.That(deserialized2.Prop2, Is.Not.Null);
		Assert.That(deserialized2.Prop2, Is.TypeOf<List<string>>());
		Assert.That(deserialized2.Prop1, Is.Not.SameAs(obj.Prop1));
		var deserialized3 = (List<string>)deserialized2.Prop2;
		Assert.That(deserialized3, Is.EqualTo(new List<string> { "Test1", "test2" }));
	}

	[Test]
	public void BugCase_Cyclic_2_CalculateSize() {
		var factory = new SerializerFactory(SerializerFactory.Default);
		factory.RegisterAutoBuild<TwoPropertyObject>();
		var serializer = SerializerBuilder.AutoBuild<TwoPropertyObject>(factory).AsReferenceSerializer();
		var obj = new TwoPropertyObject {
			Prop1 = "Hello",
			Prop2 = new TwoPropertyObject {
				Prop1 = "Hello",
				Prop2 = new List<string> { "Test1", "test2" }
			}
		};
		var size = serializer.CalculateSize(obj);
		var serialized = serializer.SerializeBytesLE(obj);
		Assert.That(size, Is.EqualTo(serialized.Length));
	}

	[Test]
	public void BugCase_Cyclic_2_CalculateSize_2() {
		var factory = new SerializerFactory(SerializerFactory.Default);
		factory.RegisterAutoBuild<TwoPropertyObject>();
		var serializer = SerializerBuilder.AutoBuild<TwoPropertyObject>(factory).AsReferenceSerializer();
		var obj = new TwoPropertyObject {
			Prop1 = "Hello",
			Prop2 = new TwoPropertyObject {
				Prop1 = "Hello",
			}
		};
		var size = serializer.CalculateSize(obj);
		var serialized = serializer.SerializeBytesLE(obj);
		Assert.That(size, Is.EqualTo(serialized.Length));
	}

	[Test]
	public void BugCase_Cyclic_2_CalculateSize_3() {
		var factory = new SerializerFactory(SerializerFactory.Default);
		factory.RegisterAutoBuild<TwoPropertyObject>();
		var serializer = SerializerBuilder.AutoBuild<List<string>>(factory).AsReferenceSerializer();
		var obj = new List<string> { "4" };
		var size = serializer.CalculateSize(obj);
		var serialized = serializer.SerializeBytesLE(obj);
		Assert.That(size, Is.EqualTo(serialized.Length));
	}

	[Test]
	public void BugCase() {
		var factory = new SerializerFactory(SerializerFactory.Default);
		factory.RegisterAutoBuild<TwoPropertyObject>();
		var serializer = factory.GetSerializer<List<string>>();
		var obj = new List<string> { "4" };
		var size = serializer.CalculateSize(obj);
		var serialized = serializer.SerializeBytesLE(obj);
		Assert.That(size, Is.EqualTo(serialized.Length));
	}


	[Test]
	public void BugCase_Cyclic_3() {
		var factory = new SerializerFactory(SerializerFactory.Default);
		factory.RegisterAutoBuild<TwoPropertyObject>();
		var serializer = SerializerBuilder.AutoBuild<TwoPropertyObject>(factory).AsReferenceSerializer();
		var obj = new TwoPropertyObject {
			Prop1 = "Hello",
			Prop2 = "Hello"
		};
		var serialized = serializer.SerializeBytesLE(obj);
		var deserialized = serializer.DeserializeBytesLE(serialized);
		Assert.That(deserialized.Prop1, Is.EqualTo("Hello"));
		Assert.That(deserialized.Prop2, Is.EqualTo("Hello"));
	}

	[Test]
	public void BugCase_Cyclic_3_CalculateSize() {
		var factory = new SerializerFactory(SerializerFactory.Default);
		factory.RegisterAutoBuild<TwoPropertyObject>();
		var serializer = SerializerBuilder.AutoBuild<TwoPropertyObject>(factory).AsReferenceSerializer();
		var obj = new TwoPropertyObject {
			Prop1 = "Hello",
			Prop2 = "Hello"
		};
		var size = serializer.CalculateSize(obj);
		var serialized = serializer.SerializeBytesLE(obj);
		Assert.That(size, Is.EqualTo(serialized.Length));
	}

	[Test]
	public void BugCase_1() {
		var factory = new SerializerFactory(SerializerFactory.Default);
		factory.RegisterAutoBuild<TestObject>();

		var serializer = SerializerBuilder.AutoBuild<SingleObjectPropertyClass>(factory);

		var obj = new SingleObjectPropertyClass {
			Property = new KeyValuePair<string, TestObject>("Hello", new TestObject("Hello2", 123, true))
		};

		var serialized = serializer.SerializeBytesLE(obj);
		Assert.That(() => serializer.DeserializeBytesLE(serialized), Throws.Nothing);
	}

	[Test]
	public void BugCase_1A() {
		var factory = new SerializerFactory(SerializerFactory.Default);
		var serializer = SerializerBuilder.AutoBuild<TestObject>(factory);

		var obj = new TestObject("Hello2", 123, true);

		var serialized = serializer.SerializeBytesLE(obj);
		Assert.That(() => serializer.DeserializeBytesLE(serialized), Throws.Nothing);
	}



	[Test]
	public void BugCase_2() {
		var factory = new SerializerFactory(SerializerFactory.Default);
		factory.RegisterAutoBuild<TwoPropertyObject>();
		var serializer = SerializerBuilder.AutoBuild<TwoPropertyObject>(factory);
		var obj = new TwoPropertyObject {
			Prop1 = "Hello",
			Prop2 = new TwoPropertyObject {
				Prop1 = "Hello",
				Prop2 = new List<string> { "Test1", "test2" }
			}
		};
		var serialized = serializer.SerializeBytesLE(obj);
		Assert.That(() => serializer.DeserializeBytesLE(serialized), Throws.Nothing);
	}


	[Test]
	public void NullableEnum_Null() {
		// test object
		var serializer = SerializerBuilder.AutoBuild<CrudAction?>();

		var serialized = serializer.SerializeBytesLE(null);
		var deserialized = serializer.DeserializeBytesLE(serialized);

		Assert.That(deserialized, Is.Null);
	}

	[Test]
	public void NullableEnum_Value() {
		// test object
		var serializer = SerializerBuilder.AutoBuild<CrudAction?>();

		var serialized = serializer.SerializeBytesLE(CrudAction.Create);
		var deserialized = serializer.DeserializeBytesLE(serialized);

		Assert.That(deserialized, Is.EqualTo(CrudAction.Create));
	}

	[Test]
	public void NullableEnumProperty_Null() {
		// test object
		var serializer = SerializerBuilder.AutoBuild<SingleNullableEnumPropertyClass>();

		var serialized = serializer.SerializeBytesLE(new SingleNullableEnumPropertyClass { Property = null });
		var deserialized = serializer.DeserializeBytesLE(serialized);

		Assert.That(deserialized.Property, Is.Null);
	}

	[Test]
	public void NullableEnumProperty_Value() {

		var serializer = SerializerBuilder.AutoBuild<SingleNullableEnumPropertyClass>();
		var serialized = serializer.SerializeBytesLE(new SingleNullableEnumPropertyClass { Property = CrudAction.Create });
		var deserialized = serializer.DeserializeBytesLE(serialized);

		Assert.That(deserialized.Property, Is.EqualTo(CrudAction.Create));
	}

	[Test]
	public void NullableEnumPropertyAsObject_Null() {
		var factory = new SerializerFactory(SerializerFactory.Default);
		factory.RegisterEnum<CrudAction>();
		var serializer = SerializerBuilder.AutoBuild<SingleObjectPropertyClass>(factory);

		var serialized = serializer.SerializeBytesLE(new SingleObjectPropertyClass { Property = null });
		var deserialized = serializer.DeserializeBytesLE(serialized);

		Assert.That(deserialized.Property, Is.Null);
	}

	[Test]
	public void NullableEnumPropertyAsObject_Value() {
		var factory = new SerializerFactory(SerializerFactory.Default);
		factory.RegisterEnum<CrudAction>();
		var serializer = SerializerBuilder.AutoBuild<SingleObjectPropertyClass>(factory);

		var serialized = serializer.SerializeBytesLE(new SingleObjectPropertyClass { Property = CrudAction.Create });
		var deserialized = serializer.DeserializeBytesLE(serialized);

		Assert.That(deserialized.Property, Is.EqualTo(CrudAction.Create));
	}
	
	[Test]
	public void TestObject_2() {
		// Test with null string field
		var serializer = SerializerBuilder
			.For<TestObject>()
			.Serialize(x => x.A, StringSerializer.UTF8.AsReferenceSerializer())
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
			.Serialize(x => x.A, StringSerializer.UTF8.AsReferenceSerializer())
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
			.Serialize(x => x.A, StringSerializer.UTF8.AsReferenceSerializer())
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
			.Serialize(x => x.A, StringSerializer.UTF8.AsReferenceSerializer())
			.Serialize(x => x.B, PrimitiveSerializer<int>.Instance)
			.Serialize(x => x.C, PrimitiveSerializer<bool>.Instance)
			.Build();

		Assert.That(serializer.SupportsNull, Is.True);	
		
		var serialized = serializer.SerializeBytesLE(null);
		var deserialized = serializer.DeserializeBytesLE(serialized);

		Assert.That(deserialized, Is.Null);
			
	}


	[Test]
	public void Transient_1() {
		// test object
		var serializer = SerializerBuilder.AutoBuild<TransientPropertyObject>();
		
		var testObj = new TransientPropertyObject { Prop1 = "alpha", TransientInt = 11, TransientString = "beta" };
		testObj.Nested = testObj;
		
		var serialized = serializer.SerializeBytesLE(testObj);
		var deserialized = serializer.DeserializeBytesLE(serialized);

		Assert.That(deserialized.Prop1, Is.EqualTo("alpha"));
		Assert.That(deserialized.TransientInt, Is.EqualTo(default(int)));
		Assert.That(deserialized.TransientString, Is.EqualTo(default(string)));
		Assert.That(deserialized.Nested, Is.EqualTo(default(string)));
	}
}
