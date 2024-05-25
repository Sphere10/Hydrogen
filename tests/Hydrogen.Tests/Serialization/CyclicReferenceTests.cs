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
using FluentAssertions;
using NUnit.Framework;

namespace Hydrogen.Tests;

[TestFixture]
[Parallelizable]
public class CyclicReferenceTests {
	
	public enum SerializerType {
		Binary,
		Factory
	}

	public class SinglePropertyObject {
		public object Property { get; set; }
	}

	private IItemSerializer<T> CreateSerializer<T>(SerializerType serializerType) {
		switch (serializerType) {
			case SerializerType.Binary:
				return new BinarySerializer().AsCastedSerializer<object, T>();
			case SerializerType.Factory:
				return SerializerBuilder.FactoryAssemble<T>();
			default:
				throw new ArgumentOutOfRangeException();
		}
	}

	[Test]
	public void CyclicReference_1([Values] SerializerType serializerType) {
		// test object
		var serializer = CreateSerializer<SinglePropertyObject>(serializerType);
		var obj = new SinglePropertyObject();
		obj.Property = obj;

		var serialized = serializer.SerializeBytesLE(obj);
		var deserialized = serializer.DeserializeBytesLE(serialized);

		Assert.That(deserialized.Property, Is.SameAs(deserialized));
	}

	[Test]
	public void CyclicReference_1_CalculateSize([Values] SerializerType serializerType) {
		// test object
		var serializer = CreateSerializer<SinglePropertyObject>(serializerType);
		var obj = new SinglePropertyObject();
		obj.Property = obj;
		var size = serializer.CalculateSize(obj);
		var serialized = serializer.SerializeBytesLE(obj);
		Assert.That(size, Is.EqualTo(serialized.Length));
	}

	[Test]
	public void CyclicReference_2([Values] SerializerType serializerType) {
		// test object
		var serializer = CreateSerializer<SinglePropertyObject>(serializerType);
		var obj1 = new SinglePropertyObject();
		var obj2 = new SinglePropertyObject();
		obj1.Property = obj2;
		obj2.Property = obj1;

		var serialized = serializer.SerializeBytesLE(obj1);
		var deserialized = serializer.DeserializeBytesLE(serialized);

		Assert.That(deserialized.Property, Is.Not.SameAs(deserialized));
		Assert.That(deserialized.Property, Is.TypeOf<SinglePropertyObject>());
		Assert.That(((SinglePropertyObject)deserialized.Property).Property, Is.SameAs(deserialized));
	}

	[Test]
	public void CyclicReference_2_CalculateSize([Values] SerializerType serializerType) {
		// test object
		var serializer = CreateSerializer<SinglePropertyObject>(serializerType);
		var obj1 = new SinglePropertyObject();
		var obj2 = new SinglePropertyObject();
		obj1.Property = obj2;
		obj2.Property = obj1;

		var size = serializer.CalculateSize(obj1);
		var serialized = serializer.SerializeBytesLE(obj1);
		Assert.That(size, Is.EqualTo(serialized.Length));
	}

	[Test]
	public void CyclicReference_3([Values] SerializerType serializerType) {
		// test object
		var serializer = CreateSerializer<SinglePropertyObject>(serializerType);
		var obj1 = new SinglePropertyObject();
		var obj2 = new SinglePropertyObject();
		obj1.Property = obj2;
		obj2.Property = obj2;

		var serialized = serializer.SerializeBytesLE(obj1);
		var deserialized = serializer.DeserializeBytesLE(serialized);

		Assert.That(deserialized.Property, Is.Not.SameAs(deserialized));
		Assert.That(deserialized.Property, Is.TypeOf<SinglePropertyObject>());
		Assert.That(((SinglePropertyObject)deserialized.Property).Property, Is.SameAs(deserialized.Property));
	}
	
	[Test]
	public void CyclicReference_3_CalculateSize([Values] SerializerType serializerType) {
		// test object
		var serializer = CreateSerializer<SinglePropertyObject>(serializerType);
		var obj1 = new SinglePropertyObject();
		var obj2 = new SinglePropertyObject();
		obj1.Property = obj2;
		obj2.Property = obj2;

		var size = serializer.CalculateSize(obj1);
		var serialized = serializer.SerializeBytesLE(obj1);
		Assert.That(size, Is.EqualTo(serialized.Length));
	}

	[Test]
	public void CyclicReference_4([Values] SerializerType serializerType) {
		// test object
		var serializer = CreateSerializer<SinglePropertyObject>(serializerType);
		var obj1 = new SinglePropertyObject();
		var obj2 = new SinglePropertyObject();
		var obj3 = new SinglePropertyObject();

		obj1.Property = obj2;
		obj2.Property = obj3;
		obj3.Property = obj2;

		var serialized = serializer.SerializeBytesLE(obj1);
		var d_obj1 = serializer.DeserializeBytesLE(serialized);
		var d_obj2 = (SinglePropertyObject)d_obj1.Property;
		var d_obj3 = (SinglePropertyObject)d_obj2.Property;


		Assert.That(d_obj1, Is.Not.Null);
		Assert.That(d_obj2, Is.Not.Null);
		Assert.That(d_obj3, Is.Not.Null);

		Assert.That(d_obj1, Is.Not.SameAs(d_obj1.Property));
		Assert.That(d_obj1, Is.Not.SameAs(d_obj2.Property));
		Assert.That(d_obj1, Is.Not.SameAs(d_obj3.Property));

		Assert.That(d_obj2, Is.Not.SameAs(d_obj2.Property));
		Assert.That(d_obj2, Is.SameAs(d_obj3.Property));

	}

	[Test]
	public void CyclicReference_4_CalculateSize([Values] SerializerType serializerType) {
		// test object
		var serializer = CreateSerializer<SinglePropertyObject>(serializerType);
		var obj1 = new SinglePropertyObject();
		var obj2 = new SinglePropertyObject();
		var obj3 = new SinglePropertyObject();

		obj1.Property = obj2;
		obj2.Property = obj3;
		obj3.Property = obj2;

		var size = serializer.CalculateSize(obj1);
		var serialized = serializer.SerializeBytesLE(obj1);
		Assert.That(size, Is.EqualTo(serialized.Length));
	}

	[Test]
	public void CyclicReference_None([Values] SerializerType serializerType) {
		// test object
		var serializer = CreateSerializer<SinglePropertyObject>(serializerType);
		var obj1 = new SinglePropertyObject();
		var obj2 = new SinglePropertyObject();
		obj1.Property = obj2;
		obj2.Property = null;

		var serialized = serializer.SerializeBytesLE(obj1);
		var deserialized = serializer.DeserializeBytesLE(serialized);

		Assert.That(deserialized.Property, Is.Not.SameAs(deserialized));
		Assert.That(deserialized.Property, Is.TypeOf<SinglePropertyObject>());
		Assert.That(((SinglePropertyObject)deserialized.Property).Property, Is.Null);
	}

	[Test]
	public void CyclicReference_None_CalculateSize([Values] SerializerType serializerType) {
		// test object
		var serializer = CreateSerializer<SinglePropertyObject>(serializerType);
		var obj1 = new SinglePropertyObject();
		var obj2 = new SinglePropertyObject();
		obj1.Property = obj2;
		obj2.Property = null;

		var size = serializer.CalculateSize(obj1);
		var serialized = serializer.SerializeBytesLE(obj1);
		Assert.That(size, Is.EqualTo(serialized.Length));
	}

	[Test]
	public void CyclicReference_NestedType([Values] SerializerType serializerType) {
		var serializer = CreateSerializer<NestedType>(serializerType);

		var item = new NestedType();
		item.Nested = item;

		var size = serializer.CalculateSize(item);
		var serialized = serializer.SerializeBytesLE(item);
		var deserialized = serializer.DeserializeBytesLE(serialized);

		Assert.That(size, Is.EqualTo(serialized.Length));
		Assert.That(deserialized, Is.TypeOf<NestedType>());
		Assert.That(deserialized.Nested, Is.SameAs(deserialized));
	}

	[Test]
	public void CyclicReference_BugCase([Values] SerializerType serializerType) {
		var serializer = CreateSerializer<List<SinglePropertyObject>>(serializerType);

		var obj = new SinglePropertyObject();
		var item = new List<SinglePropertyObject> { obj };
		obj.Property = item;

		var size = serializer.CalculateSize(item);
		var serialized = serializer.SerializeBytesLE(item);
		var deserialized = serializer.DeserializeBytesLE(serialized);

		Assert.That(size, Is.EqualTo(serialized.Length));
		deserialized.Should().BeEquivalentTo(item, x=> x.IgnoringCyclicReferences());
		
		
	}
}
