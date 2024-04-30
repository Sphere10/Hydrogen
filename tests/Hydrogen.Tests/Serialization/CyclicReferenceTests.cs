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
using NUnit.Framework;

namespace Hydrogen.Tests;

[TestFixture]
[Parallelizable]
public class CyclicReferenceTests {
	

	public class SinglePropertyObject {
		public object Property { get; set; }
	}



	[Test]
	public void CyclicReference_1() {
		// test object
		var serializer = SerializerBuilder.AutoBuild<SinglePropertyObject>();
		var obj = new SinglePropertyObject();
		obj.Property = obj;

		var serialized = serializer.SerializeBytesLE(obj);
		var deserialized = serializer.DeserializeBytesLE(serialized);

		Assert.That(deserialized.Property, Is.SameAs(deserialized));
	}


	[Test]
	public void CyclicReference_1_CalculateSize() {
		// test object
		var serializer = SerializerBuilder.AutoBuild<SinglePropertyObject>();
		var obj = new SinglePropertyObject();
		obj.Property = obj;
		var size = serializer.CalculateSize(obj);
		var serialized = serializer.SerializeBytesLE(obj);
		Assert.That(size, Is.EqualTo(serialized.Length));
	}

	[Test]
	public void CyclicReference_2() {
		// test object
		var serializer = SerializerBuilder.AutoBuild<SinglePropertyObject>();
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
	public void CyclicReference_2_CalculateSize() {
		// test object
		var serializer = SerializerBuilder.AutoBuild<SinglePropertyObject>();
		var obj1 = new SinglePropertyObject();
		var obj2 = new SinglePropertyObject();
		obj1.Property = obj2;
		obj2.Property = obj1;

		var size = serializer.CalculateSize(obj1);
		var serialized = serializer.SerializeBytesLE(obj1);
		Assert.That(size, Is.EqualTo(serialized.Length));
	}

	[Test]
	public void CyclicReference_3() {
		// test object
		var serializer = SerializerBuilder.AutoBuild<SinglePropertyObject>();
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
	public void CyclicReference_3_CalculateSize() {
		// test object
		var serializer = SerializerBuilder.AutoBuild<SinglePropertyObject>();
		var obj1 = new SinglePropertyObject();
		var obj2 = new SinglePropertyObject();
		obj1.Property = obj2;
		obj2.Property = obj2;

		var size = serializer.CalculateSize(obj1);
		var serialized = serializer.SerializeBytesLE(obj1);
		Assert.That(size, Is.EqualTo(serialized.Length));
	}

	[Test]
	public void CyclicReference_4() {
		// test object
		var serializer = SerializerBuilder.AutoBuild<SinglePropertyObject>();
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
	public void CyclicReference_4_CalculateSize() {
		// test object
		var serializer = SerializerBuilder.AutoBuild<SinglePropertyObject>();
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
	public void CyclicReference_None() {
		// test object
		var serializer = SerializerBuilder.AutoBuild<SinglePropertyObject>();
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
	public void CyclicReference_None_CalculateSize() {
		// test object
		var serializer = SerializerBuilder.AutoBuild<SinglePropertyObject>();
		var obj1 = new SinglePropertyObject();
		var obj2 = new SinglePropertyObject();
		obj1.Property = obj2;
		obj2.Property = null;

		var size = serializer.CalculateSize(obj1);
		var serialized = serializer.SerializeBytesLE(obj1);
		Assert.That(size, Is.EqualTo(serialized.Length));
	}


	
	[Test]
	public void CyclicReference_NestedType() {
		var factory = SerializerFactory.Default;
		var serializer = factory.GetSerializer<NestedType>();

		var item = new NestedType();
		item.Nested = item;
		var bytes = serializer.SerializeBytesLE(item);
		var item2 = serializer.DeserializeBytesLE(bytes);

		Assert.That(item2, Is.TypeOf<NestedType>());
		Assert.That(item2.Nested, Is.SameAs(item2));
	}
}
