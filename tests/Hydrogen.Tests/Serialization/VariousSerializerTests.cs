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
using AutoFixture;
using FluentAssertions;
using NUnit.Framework;

namespace Hydrogen.Tests;

[TestFixture]
public class VariousSerializerTests {

	#region Test types

	public enum TestType {
		BinarySerializer_Sizing_Then_Serialize,
		BinarySerializer_SerializeOnly,
		Factory_Sizing_Then_Serialize,
		Factory_SerializeOnly,
	}

	internal class PrimitiveTestObject {
		public string A { get; set; }
		public bool B { get; set; }
		public int C { get; set; }
		public uint D { get; set; }
		public short E { get; set; }
		public ushort F { get; set; }
		public long G { get; set; }
		public ulong H { get; set; }
		public double I { get; set; }
		public decimal J { get; set; }
		public sbyte K { get; set; }
		public byte L { get; set; }
		public float M { get; set; }
	}

	internal class ValueTypeTestObject {
		public int? A { get; set; }
		public DateTime B { get; set; }
		public DateTime? C { get; set; }
		public DateTimeOffset D { get; set; }
		public DateTimeOffset? E { get; set; }
	}

	internal class CollectionTestObject {
		public List<DateTime> A { get; set; }
		public ArrayList B { get; set; }
		public PrimitiveTestObject[] C { get; set; }
		public IDictionary<int, PrimitiveTestObject> D { get; set; }
		public List<int> E { get; set; }
		public byte[] F { get; set; }
		public IList<PrimitiveTestObject> G { get; set; }
		public SortedSet<bool> H { get; set; }
	}

	internal class NullTestObject {
		public object U { get; set; }
		public string V { get; set; }
	}

	internal class ReferenceTypeObject {
		public ValueTypeTestObject V { get; set; }
		public EnumObj W { get; set; }
		public PrimitiveTestObject X { get; set; }
		public CollectionTestObject Y { get; set; }
		public NullTestObject Z { get; set; }
	}

	internal class GenericTypeObj {
		public GenericTypeObj<ReferenceTypeObject, CollectionTestObject> A { get; set; }
	}

	internal class GenericTypeObj<T1, T2> {
		public T1 A { get; set; }
		public T2 B { get; set; }
	}

	internal class CircularReferenceObj {
		public CircularReferenceObj A { get; set; }
		public PrimitiveTestObject B { get; set; }
	}


	internal class ObjectObj {
		public object A { get; set; }
		public object B { get; set; }
		public object C { get; set; }
		public object D { get; set; }
		public object E { get; set; }
	}

	internal class SubClassObj : PrimitiveTestObject {
		public int X { get; set; }
	}

	internal class EnumObj {
		[Flags]
		internal enum TestEnum : byte {
			A = 1,
			B = 2,
			C = 3,
			D = 4
		}

		public TestEnum A { get; set; }

		public TestEnum B { get; set; }
	}

	#endregion

	public void DoTestForType(Type type, TestType testType, out object deserialized) {
		var args = new object[] { testType, null };
		typeof(VariousSerializerTests).GetGenericMethod("DoTest", 1).MakeGenericMethod([type]).Invoke(this, args);
		deserialized = args[1];
	}

	public void DoTest<T>(TestType testType, out T deserialized) {
		DoTest(new Fixture().Create<T>(), testType, out var deserializedObj);
		deserialized = (T)deserializedObj;
	}

	public void DoTest(object item, TestType testType, out object deserialized) {
		IItemSerializer<object> serializer;
		var testSize = false;
		switch (testType) {
			case TestType.BinarySerializer_Sizing_Then_Serialize:
			case TestType.BinarySerializer_SerializeOnly:
				serializer = new BinarySerializer();
				break;
			case TestType.Factory_Sizing_Then_Serialize:
			case TestType.Factory_SerializeOnly:
				var factory = new SerializerFactory(SerializerFactory.Default);
				if (!factory.ContainsSerializer<PrimitiveTestObject>())
					factory.RegisterAutoBuild<PrimitiveTestObject>();
				if (!factory.ContainsSerializer<ValueTypeTestObject>())
					factory.RegisterAutoBuild<ValueTypeTestObject>();
				if (!factory.ContainsSerializer<CollectionTestObject>())
					factory.RegisterAutoBuild<CollectionTestObject>();
				if (!factory.ContainsSerializer<NullTestObject>())
					factory.RegisterAutoBuild<NullTestObject>();
				if (!factory.ContainsSerializer<ReferenceTypeObject>())
					factory.RegisterAutoBuild<ReferenceTypeObject>();
				if (!factory.ContainsSerializer<GenericTypeObj>())
					factory.RegisterAutoBuild<GenericTypeObj>();
				if (!factory.ContainsSerializer<GenericTypeObj<ReferenceTypeObject, CollectionTestObject>>())
					factory.RegisterAutoBuild<GenericTypeObj<ReferenceTypeObject, CollectionTestObject>>();
				if (!factory.ContainsSerializer<CircularReferenceObj>())
					factory.RegisterAutoBuild<CircularReferenceObj>();
				if (!factory.ContainsSerializer<ObjectObj>())
					factory.RegisterAutoBuild<ObjectObj>();
				if (!factory.ContainsSerializer<SubClassObj>())
					factory.RegisterAutoBuild<SubClassObj>();
				if (!factory.ContainsSerializer<EnumObj>())
					factory.RegisterAutoBuild<EnumObj>();

				serializer = new ObjectSerializer(factory);
				break;
			default:
				throw new ArgumentOutOfRangeException();
		}

		switch (testType) {
			case TestType.BinarySerializer_Sizing_Then_Serialize:
			case TestType.Factory_Sizing_Then_Serialize:
				testSize = true;
				break;
		}

		var len = testSize ? serializer.CalculateSize(item) : 0;
		var bytes = serializer.SerializeBytesLE(item);
		deserialized = serializer.DeserializeBytesLE(bytes);
		if (testSize)
			Assert.That(len, Is.EqualTo(bytes.Length));
		item.Should().BeEquivalentTo(deserialized, x => x.IgnoringCyclicReferences());
	}

	[Test]
	public void Simple_1([Values] TestType testType) {
		DoTest(123, testType, out _);
	}

	[Test]
	public void Simple_2([Values] TestType testType) {
		var obj = new List<int> { 1, 2, 3, 4, 5 };
		DoTest(obj, testType, out _);
	}

	[Test]
	public void Complex([Values] TestType testType) {
		var obj = new List<string> { "hello", "world", "hello" };
		DoTest(obj, testType, out var deserializedObj);
		var deserialized = (List<string>)deserializedObj;
		Assert.That(deserialized[0], Is.SameAs(deserialized[2]));
	}

	[Test]
	public void StandardTests(
		[Values(typeof(ValueTypeTestObject), typeof(CollectionTestObject), typeof(ReferenceTypeObject), typeof(GenericTypeObj), typeof(EnumObj))]
		Type type,
		[Values] TestType testType
	) => DoTestForType(type, testType, out _);

	[Test]
	public void NullObjectSerializeDeserialize([Values] TestType testType) {
		var item = new NullTestObject {
			U = null,
			V = null
		};
		DoTest(item, testType, out _);
	}

	[Test]
	public void CircularReferenceObjTests([Values] TestType testType) {
		var fixture = new Fixture();
		var item = new CircularReferenceObj {
			A = new CircularReferenceObj {
				B = fixture.Create<PrimitiveTestObject>()
			},
			B = fixture.Create<PrimitiveTestObject>()
		};
		item.A.A = item;
		DoTest(item, testType, out var deserialized);
		var deserializedItem = (CircularReferenceObj)deserialized;
		deserializedItem
			.Should()
			.BeEquivalentTo(item, x => x.IgnoringCyclicReferences());

		Assert.That(item.A.A, Is.SameAs(item));
		Assert.That(deserializedItem.A.A, Is.SameAs(deserializedItem));
	}

	[Test]
	public void ObjectTypePropertiesSerialized([Values] TestType testType) {
		var fixture = new Fixture();

		var item = new ObjectObj {
			A = new List<int> { 1, 2, 3, 4 },
			B = null,
			C = fixture.Create<ReferenceTypeObject>(),
			D = false,
			E = fixture.Create<byte[]>()
		};
		DoTest(item, testType, out _);
	}

	[Test]
	public void SubClassInCollectionTest([Values] TestType testType) {
		var fixture = new Fixture();
		List<PrimitiveTestObject> list = new List<PrimitiveTestObject>() {
			fixture.Create<PrimitiveTestObject>(),
			fixture.Create<SubClassObj>()
		};
		DoTest(list, testType, out _);
	}

	[Test]
	public void SubClassSerializeDeserialize([Values] TestType testType) {
		var fixture = new Fixture();
		var item = fixture.Create<SubClassObj>();
		DoTest(item, testType, out _);
	}

	[Test]
	public void CalculateSizeOfObject_Bug([Values] TestType testType) {
		var item = new ReferenceTypeObject();
		item.Y = new CollectionTestObject() {
			F = new byte[] { 1, 2, 3 },
		};
		DoTest(item, testType, out _);
	}


}


