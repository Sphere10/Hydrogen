// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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

	public struct TestStruct {
		public string Property1 { get; set; }
		public string Property2 { get; set; }
		public string Property3 { get; set; }
	
	}

	public class BadAnnotationObject {

		[ReferenceMode(Nullable = true)]  // can't specify this for value type members
		public TestStruct Property1 { get; set; }
	}

	public class BaseClass {

		public static int StaticProp1 { get; set; }

		public string Prop1 { get; set; }

		public virtual string Prop2 { get; set; }

		public string Prop3 { get; set; }
	}

	public class SubClass : BaseClass {
		public static int StaticProp2 { get; set; }

		public override string Prop2 { get; set; }

		public new string Prop3 { get; set; }

		public string Prop4 { get; set; }
	}

	public class GenericType<T1, T2> {
		public T1 Prop1 { get; set; }
		public T2 Prop2 { get; set; }
	}

	public class NoConstructorClass {
		private NoConstructorClass() {
		}

		public NoConstructorClass(string prop1) {
			Prop1 = prop1;
		}
		public string Prop1 { get; set; }

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

		var serializer2 = SerializerBuilder.FactoryAssemble<TestObject>();

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
		var serializer = SerializerBuilder.FactoryAssemble<ComplexObject>();
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
		var serializer = SerializerBuilder.FactoryAssemble<ComplexObject>();
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

		var serializer = SerializerBuilder.FactoryAssemble<ComplexObject>(factory);
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

		var serializer = SerializerBuilder.FactoryAssemble<ComplexObject>(factory);
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
	public void BugCase_ValueTuple7() {
		var serializerType = typeof(ValueTupleSerializer<,,,,,,>);
		var datType = typeof(ValueTuple<,,,,,,>);
		Assert.That(serializerType.IsSubtypeOfGenericType(typeof(IItemSerializer<>).MakeGenericType(datType)), Is.True);
	}

	[Test]
	public void BugCase_ValueTuple8() {
		var serializerType = typeof(ValueTupleSerializer<,,,,,,,>);
		var datType = typeof(ValueTuple<,,,,,,,>);
		Assert.That(serializerType.IsSubtypeOfGenericType(typeof(IItemSerializer<>).MakeGenericType(datType)), Is.True);
	}

	[Test]
	public void BugCase_Tuple7() {
		var serializerType = typeof(TupleSerializer<,,,,,,>);
		var datType = typeof(Tuple<,,,,,,>);
		Assert.That(serializerType.IsSubtypeOfGenericType(typeof(IItemSerializer<>).MakeGenericType(datType)), Is.True);
	}

	[Test]
	public void BugCase_Tuple8() {
		var serializerType = typeof(TupleSerializer<,,,,,,,>);
		var datType = typeof(Tuple<,,,,,,,>);
		Assert.That(serializerType.IsSubtypeOfGenericType(typeof(IItemSerializer<>).MakeGenericType(datType)), Is.True);
	}

	[Test]
	public void AutoBuildComplex_Cyclic() {
		var factory = new SerializerFactory(SerializerFactory.Default);
		factory.RegisterAutoBuild<TestObject>();

		var serializer = SerializerBuilder.FactoryAssemble<ComplexObject>(factory);
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

		var serializer = SerializerBuilder.FactoryAssemble<ComplexObject>(factory);
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
		var serializer = SerializerBuilder.FactoryAssemble<TwoPropertyObject>(factory);
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
		var serializer = SerializerBuilder.FactoryAssemble<TwoPropertyObject>(factory);
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
		var serializer = SerializerBuilder.FactoryAssemble<TwoPropertyObject>(factory);
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
		var serializer = SerializerBuilder.FactoryAssemble<TwoPropertyObject>(factory);
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
		var serializer = SerializerBuilder.FactoryAssemble<TwoPropertyObject>(factory);
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
		var serializer = SerializerBuilder.FactoryAssemble<List<string>>(factory);
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
		var serializer = SerializerBuilder.FactoryAssemble<TwoPropertyObject>(factory);
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
		var serializer = SerializerBuilder.FactoryAssemble<TwoPropertyObject>(factory);
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

		var serializer = SerializerBuilder.FactoryAssemble<SingleObjectPropertyClass>(factory);

		var obj = new SingleObjectPropertyClass {
			Property = new KeyValuePair<string, TestObject>("Hello", new TestObject("Hello2", 123, true))
		};

		var serialized = serializer.SerializeBytesLE(obj);
		Assert.That(() => serializer.DeserializeBytesLE(serialized), Throws.Nothing);
	}

	[Test]
	public void BugCase_1A() {
		var factory = new SerializerFactory(SerializerFactory.Default);
		var serializer = SerializerBuilder.FactoryAssemble<TestObject>(factory);

		var obj = new TestObject("Hello2", 123, true);

		var serialized = serializer.SerializeBytesLE(obj);
		Assert.That(() => serializer.DeserializeBytesLE(serialized), Throws.Nothing);
	}

	[Test]
	public void BugCase_2() {
		var factory = new SerializerFactory(SerializerFactory.Default);
		factory.RegisterAutoBuild<TwoPropertyObject>();
		var serializer = SerializerBuilder.FactoryAssemble<TwoPropertyObject>(factory);
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
		var serializer = SerializerBuilder.FactoryAssemble<CrudAction?>();

		var serialized = serializer.SerializeBytesLE(null);
		var deserialized = serializer.DeserializeBytesLE(serialized);

		Assert.That(deserialized, Is.Null);
	}

	[Test]
	public void NullableEnum_Value() {
		// test object
		var serializer = SerializerBuilder.FactoryAssemble<CrudAction?>();

		var serialized = serializer.SerializeBytesLE(CrudAction.Create);
		var deserialized = serializer.DeserializeBytesLE(serialized);

		Assert.That(deserialized, Is.EqualTo(CrudAction.Create));
	}

	[Test]
	public void NullableEnumProperty_Null() {
		// test object
		var serializer = SerializerBuilder.FactoryAssemble<SingleNullableEnumPropertyClass>();
		var serialized = serializer.SerializeBytesLE(new SingleNullableEnumPropertyClass { Property = null });
		var deserialized = serializer.DeserializeBytesLE(serialized);
		Assert.That(deserialized.Property, Is.Null);
	}

	[Test]
	public void GenericTypeTest() {
		// test object
		var serializer = SerializerBuilder.FactoryAssemble<GenericType<int, string>>();
		var item = new GenericType<int, string> {
			Prop1 = 123,
			Prop2 = "Hello"
		};
		var serialized = serializer.SerializeBytesLE(item);
		var deserialized = serializer.DeserializeBytesLE(serialized);

		Assert.That(deserialized.Prop1, Is.EqualTo(item.Prop1));
		Assert.That(deserialized.Prop2, Is.EqualTo(item.Prop2));
	}

	[Test]
	public void NullableEnumProperty_Value() {

		var serializer = SerializerBuilder.FactoryAssemble<SingleNullableEnumPropertyClass>();
		var serialized = serializer.SerializeBytesLE(new SingleNullableEnumPropertyClass { Property = CrudAction.Create });
		var deserialized = serializer.DeserializeBytesLE(serialized);

		Assert.That(deserialized.Property, Is.EqualTo(CrudAction.Create));
	}

	[Test]
	public void NullableEnumPropertyAsObject_Null() {
		var factory = new SerializerFactory(SerializerFactory.Default);
		factory.RegisterEnum<CrudAction>();
		var serializer = SerializerBuilder.FactoryAssemble<SingleObjectPropertyClass>(factory);

		var serialized = serializer.SerializeBytesLE(new SingleObjectPropertyClass { Property = null });
		var deserialized = serializer.DeserializeBytesLE(serialized);

		Assert.That(deserialized.Property, Is.Null);
	}

	[Test]
	public void NullableEnumPropertyAsObject_Value() {
		var factory = new SerializerFactory(SerializerFactory.Default);
		factory.RegisterEnum<CrudAction>();
		var serializer = SerializerBuilder.FactoryAssemble<SingleObjectPropertyClass>(factory);

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
			//.AllowNull()
			.Serialize(x => x.A, StringSerializer.UTF8.AsReferenceSerializer())
			.Serialize(x => x.B, PrimitiveSerializer<int>.Instance)
			.Serialize(x => x.C, PrimitiveSerializer<bool>.Instance)
			.Build()
			.AsNullableSerializer();

		Assert.That(serializer.SupportsNull, Is.True);	
		
		var serialized = serializer.SerializeBytesLE(null);
		var deserialized = serializer.DeserializeBytesLE(serialized);

		Assert.That(deserialized, Is.Null);
	}

	[Test]
	public void BadAnnotation_1() {
		Assert.That(SerializerBuilder.FactoryAssemble<BadAnnotationObject>, Throws.InvalidOperationException);
	}

	[Test]
	public void SerializableMembersInBaseFirstOrder() {
		var members = SerializerBuilder.GetSerializableMembers(typeof(SubClass));
		Assert.That(members.Length, Is.EqualTo(4));
		Assert.That(members[0].Name, Is.EqualTo("Prop1"));
		Assert.That(members[1].Name, Is.EqualTo("Prop2"));
		Assert.That(members[2].Name, Is.EqualTo("Prop3"));
		Assert.That(members[3].Name, Is.EqualTo("Prop4"));
	}

	[Test]
	public void NoSerializerForUnconstructableClass() {
		Assert.That (() => ItemSerializer<NoConstructorClass>.Default, Throws.InvalidOperationException);
	}
}
