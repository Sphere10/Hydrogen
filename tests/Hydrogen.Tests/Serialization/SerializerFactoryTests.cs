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
using System.Linq;
using NUnit.Framework;
using NUnit.Framework.Legacy;

namespace Hydrogen.Tests;

[TestFixture]
[Parallelizable]
public class SerializerFactoryTests {

	internal class PrimitiveTestObject {
		public string A { get; set; }
	}


	[Test]
	public void PureObjectSerializer() {
		var serializer = SerializerFactory.Default.GetPureSerializer<object>();
		Assert.That(serializer, Is.InstanceOf<PureObjectSerializer>());
	}

	[Test]
	public void PolymorphicObjectSerializer() {
		var serializer = SerializerFactory.Default.GetSerializer<object>();
		Assert.That(((ReferenceSerializer<object>)serializer).Internal, Is.InstanceOf<PolymorphicSerializer<object>>());
	}

	[Test]
	public void DoesNotReturnReferenceSerializer_1() {
		var serializer = SerializerFactory.Default.GetPureSerializer<string>();
		Assert.That(serializer, Is.Not.TypeOf<ReferenceSerializer<string>>());
	}

	[Test]
	public void DoesNotReturnReferenceSerializer_2() {
		var serializer = SerializerFactory.Default.GetPureSerializer<List<string>>();
		Assert.That(serializer, Is.Not.TypeOf<ReferenceSerializer<List<string>>>());
	}

	[Test]
	public void Primitive() {
		var factory = new SerializerFactory();
		factory.Register(PrimitiveSerializer<int>.Instance);
		Assert.That(factory.GetPureSerializer<int>(), Is.SameAs(PrimitiveSerializer<int>.Instance));
	}

	[Test]
	public void OpenGenericSerializer_1() {
		var factory = new SerializerFactory();
		factory.Register(PrimitiveSerializer<int>.Instance);
		factory.Register(typeof(List<>), typeof(ListSerializer<>));
		Assert.That(factory.GetPureSerializer<List<int>>(), Is.TypeOf<ListSerializer<int>>());
	}

	[Test]
	public void OpenGenericSerializer_2() {
		var factory = new SerializerFactory();
		factory.Register(PrimitiveSerializer<int>.Instance);
		factory.Register(PrimitiveSerializer<float>.Instance);
		factory.Register(typeof(List<>), typeof(ListSerializer<>));
		factory.Register(typeof(KeyValuePair<,>), typeof(KeyValuePairSerializer<,>));
		Assert.That(factory.GetPureSerializer<List<KeyValuePair<int, float>>>(), Is.TypeOf<ListSerializer<KeyValuePair<int, float>>>());
	}

	[Test]
	public void OpenGenericSerializer_3() {
		var factory = new SerializerFactory();
		factory.Register(PrimitiveSerializer<int>.Instance);
		factory.Register(typeof(List<>), typeof(ListSerializer<>));
		Assert.That(factory.GetPureSerializer<List<List<int>>>(), Is.TypeOf<ListSerializer<List<int>>>());
	}

	[Test]
	public void OpenGenericSerializer_4() {
		var factory = new SerializerFactory();
		factory.Register(PrimitiveSerializer<int>.Instance);
		factory.Register(PrimitiveSerializer<float>.Instance);
		factory.Register(typeof(List<>), typeof(ListSerializer<>));
		factory.Register(typeof(KeyValuePair<,>), typeof(KeyValuePairSerializer<,>));
		Assert.That(factory.GetPureSerializer<List<KeyValuePair<List<int>, KeyValuePair<float, List<int>>>>>(), Is.TypeOf<ListSerializer<KeyValuePair<List<int>, KeyValuePair<float, List<int>>>>>());
	}

	[Test]
	public void GetSerializerHierarchy_Open() {
		var factory = new SerializerFactory();
		factory.Register(typeof(List<>), typeof(ListSerializer<>));  // 0
		factory.Register(PrimitiveSerializer<int>.Instance);  // 1
		ClassicAssert.AreEqual(factory.GetSerializerHierarchy(typeof(List<int>)).Flatten(), new[] { SerializerFactory.PermanentTypeCodeStartDefault + 0, SerializerFactory.PermanentTypeCodeStartDefault + 1 });
	}

	[Test]
	public void GetSerializerHierarchy_Open_Complex() {
		var factory = new SerializerFactory();
		factory.Register(PrimitiveSerializer<int>.Instance); // 0
		factory.Register(PrimitiveSerializer<float>.Instance); // 1
		factory.Register(typeof(List<>), typeof(ListSerializer<>)); // 2
		factory.Register(typeof(KeyValuePair<,>), typeof(KeyValuePairSerializer<,>)); // 3
		// List< 2
		//	KeyValuePair< 3
		//		List< 2
		//			int>, 0
		//		KeyValuePair< 3
		//			float, 1 
		//			List< 2
		//				int>>>> 0
		ClassicAssert.AreEqual(factory.GetSerializerHierarchy(typeof(List<KeyValuePair<List<int>, KeyValuePair<float, List<int>>>>)).Flatten(), new[] { 2, 3, 2, 0, 3, 1, 2, 0 }.Select(x => SerializerFactory.PermanentTypeCodeStartDefault + x));
	}

	[Test]
	public void GetSerializerHierarchy_Closed_Complex() {
		var factory = new SerializerFactory();
		factory.Register(PrimitiveSerializer<int>.Instance); // 0
		factory.Register(PrimitiveSerializer<float>.Instance); // 1
		factory.Register(typeof(List<>), typeof(ListSerializer<>)); // 2
		factory.Register(typeof(KeyValuePair<,>), typeof(KeyValuePairSerializer<,>)); // 3
		var instance = new ListSerializer<KeyValuePair<List<int>, KeyValuePair<float, List<int>>>>(
			new KeyValuePairSerializer<List<int>, KeyValuePair<float, List<int>>>(
				new ListSerializer<int>(PrimitiveSerializer<int>.Instance),
				new KeyValuePairSerializer<float, List<int>>(
					PrimitiveSerializer<float>.Instance,
					new ListSerializer<int>(PrimitiveSerializer<int>.Instance)
				)
			)
		);
		factory.Register(instance); // 4 (closed specific instance)
		ClassicAssert.AreEqual(factory.GetSerializerHierarchy(typeof(List<KeyValuePair<List<int>, KeyValuePair<float, List<int>>>>)).Flatten(), new[] { SerializerFactory.PermanentTypeCodeStartDefault + 4 });
	}

	[Test]
	public void FromSerializerHierarchy_Open() {
		var factory = new SerializerFactory();
		factory.Register(typeof(List<>), typeof(ListSerializer<>));  // 0
		factory.Register(PrimitiveSerializer<int>.Instance);  // 1
		var serializerHierarchy = factory.GetSerializerHierarchy(typeof(List<int>));
		var serializer = factory.FromSerializerHierarchy(serializerHierarchy).AsUnwrapped();
		Assert.That(serializer, Is.TypeOf<ListSerializer<int>>());
	}

	[Test]
	public void FromSerializerHierarchy_Open_Complex() {
		var factory = new SerializerFactory();
		factory.Register(PrimitiveSerializer<int>.Instance); // 0
		factory.Register(PrimitiveSerializer<float>.Instance); // 1
		factory.Register(typeof(List<>), typeof(ListSerializer<>)); // 2
		factory.Register(typeof(KeyValuePair<,>), typeof(KeyValuePairSerializer<,>)); // 3
		// List< 2
		//	KeyValuePair< 3
		//		List< 2
		//			int>, 0
		//		KeyValuePair< 3
		//			float, 1 
		//			List< 2
		//				int>>>> 0
		var serializerHierarchy = factory.GetSerializerHierarchy(typeof(List<KeyValuePair<List<int>, KeyValuePair<float, List<int>>>>));
		var serializer = factory.FromSerializerHierarchy(serializerHierarchy).AsUnwrapped();;
		Assert.That(serializer, Is.TypeOf<ListSerializer<KeyValuePair<List<int>, KeyValuePair<float, List<int>>>>>());
	}

	[Test]
	public void FromSerializerHierarchy_Closed_Complex() {
		var factory = new SerializerFactory();
		factory.Register(PrimitiveSerializer<int>.Instance); // 0
		factory.Register(PrimitiveSerializer<float>.Instance); // 1
		factory.Register(typeof(List<>), typeof(ListSerializer<>)); // 2
		factory.Register(typeof(KeyValuePair<,>), typeof(KeyValuePairSerializer<,>)); // 3

		var instance = new ListSerializer<KeyValuePair<List<int>, KeyValuePair<float, List<int>>>>(
			new KeyValuePairSerializer<List<int>, KeyValuePair<float, List<int>>>(
				new ListSerializer<int>(PrimitiveSerializer<int>.Instance),
				new KeyValuePairSerializer<float, List<int>>(
					PrimitiveSerializer<float>.Instance,
					new ListSerializer<int>(PrimitiveSerializer<int>.Instance)
				)
			)
		);
		factory.Register(instance); // 4 (closed specific instance)

		ClassicAssert.AreEqual(factory.GetSerializerHierarchy(typeof(List<KeyValuePair<List<int>, KeyValuePair<float, List<int>>>>)).Flatten(), new[] { SerializerFactory.PermanentTypeCodeStartDefault + 4 });

	}
	
	[Test]
	public void RegisterSameTypeTwiceFails() {
		var factory = new SerializerFactory();
		factory.Register(PrimitiveSerializer<int>.Instance);
		Assert.That(() => factory.Register(PrimitiveSerializer<int>.Instance), Throws.InvalidOperationException);
	}

	[Test]
	public void RegisterSameOpenTypeFails() {
		var factory = new SerializerFactory();
		factory.Register(typeof(List<>), typeof(ListSerializer<>));
		Assert.That(() => factory.Register(typeof(List<>), typeof(ListSerializer<>)), Throws.InvalidOperationException);
	}

	[Test]
	public void CannotRegisterNotSerializingType() {
		var factory = new SerializerFactory();
		Assert.That(() => factory.Register(typeof(int), typeof(PrimitiveSerializer<float>)), Throws.ArgumentException);
	}

	[Test]
	public void Array() {
		var factory = new SerializerFactory();
		factory.Register(typeof(System.Array), typeof(ArraySerializer<>));
		factory.Register(PrimitiveSerializer<int>.Instance);
		var hierarchy = factory.GetSerializerHierarchy(typeof(int[]));
		Assert.That(hierarchy.Flatten(), Is.EqualTo(new[] { SerializerFactory.PermanentTypeCodeStartDefault + 0, SerializerFactory.PermanentTypeCodeStartDefault +1 }));
		var serializer = factory.FromSerializerHierarchy(hierarchy).AsDereferencedSerializer();
		Assert.That(serializer, Is.TypeOf<ArraySerializer<int>>());
	}

	[Test]
	public void Register_ArrayList() {
		var factory = new SerializerFactory();
		factory.Register<ArrayList, ArrayListSerializer>(() => new ArrayListSerializer(SerializerFactory.Default));
		var serializer = factory.GetSerializer<ArrayList>();
		Assert.That(serializer, Is.TypeOf<ReferenceSerializer<ArrayList>>());
		Assert.That(((ReferenceSerializer<ArrayList>)serializer).Internal, Is.TypeOf<PolymorphicSerializer<ArrayList>>());
		Assert.That(((PolymorphicSerializer<ArrayList>) ((ReferenceSerializer<ArrayList>)serializer).Internal).PureSerializer, Is.TypeOf<ArrayListSerializer>());
	}


	[Test]
	public void Register_NullableInt() {
		var factory = new SerializerFactory();
		factory.Register(NullableSerializer<int>.Instance);
		var serializer = factory.GetSerializer<int?>();
		Assert.That(serializer, Is.TypeOf<NullableSerializer<int>>());
	}
	[Test]
	public void ResolveNotSpecializedByteArray() {
		var factory = new SerializerFactory();
		factory.Register(typeof(System.Array), typeof(ArraySerializer<>)); // 0
		factory.Register(PrimitiveSerializer<int>.Instance); // 1
		factory.Register(ByteArraySerializer.Instance); // 2 (special for byte[])
		var hierarchy = factory.GetSerializerHierarchy(typeof(int[]));
		Assert.That(hierarchy.Flatten(), Is.EqualTo(new[] { SerializerFactory.PermanentTypeCodeStartDefault + 0, SerializerFactory.PermanentTypeCodeStartDefault + 1 }));
		var serializer = factory.FromSerializerHierarchy(hierarchy).AsDereferencedSerializer();
		Assert.That(serializer, Is.TypeOf<ArraySerializer<int>>());
	}

	[Test]
	public void ResolveSpecializedByteArray() {
		var factory = new SerializerFactory();
		factory.Register(typeof(System.Array), typeof(ArraySerializer<>)); // 0
		factory.Register(PrimitiveSerializer<int>.Instance); // 1
		factory.Register(ByteArraySerializer.Instance); // 2 (special for byte[])
		var hierarchy = factory.GetSerializerHierarchy(typeof(byte[]));
		Assert.That(hierarchy.Flatten(), Is.EqualTo(new[] { SerializerFactory.PermanentTypeCodeStartDefault + 2 }));
		var serializer = factory.FromSerializerHierarchy(hierarchy).AsDereferencedSerializer();
		Assert.That(serializer, Is.TypeOf<ByteArraySerializer>());
	}


	[Test]
	public void ArrayList_References() {
		var serializer = SerializerFactory.Default.GetSerializer<ArrayList>();

		var item = new ArrayList();
		item.Add("hello");
		item.Add(111);
		item.Add("world");

		var size = serializer.CalculateSize(item);
		var bytes = serializer.SerializeBytesLE(item);
		var item2 = serializer.DeserializeBytesLE(bytes);

		Assert.That(bytes.Length, Is.EqualTo(size));
		Assert.That(item2, Is.TypeOf<ArrayList>());
		Assert.That(item2.Count, Is.EqualTo(3));
		Assert.That(item2[0], Is.EqualTo("hello"));
		Assert.That(item2[1], Is.EqualTo(111));
		Assert.That(item2[2], Is.EqualTo("world"));
	}


	[Test]
	public void ArrayList_References_Shared() {
		var serializer = SerializerBuilder.FactoryAssemble<ArrayList>();

		var item = new ArrayList();
		var obj = new object();
		item.Add(obj);
		item.Add(111);
		item.Add(obj);

		var size = serializer.CalculateSize(item);
		var bytes = serializer.SerializeBytesLE(item);
		var item2 = serializer.DeserializeBytesLE(bytes);

		Assert.That(bytes.Length, Is.EqualTo(size));
		Assert.That(item2, Is.TypeOf<ArrayList>());
		Assert.That(item2.Count, Is.EqualTo(3));
		Assert.That(item2[1], Is.EqualTo(111));
		Assert.That(item2[2], Is.SameAs(item2[0]));
	}


	[Test]
	public void Dictionary_References_Shared() {
		var serializer = SerializerBuilder.FactoryAssemble<Dictionary<int, string>>();

		var item = new Dictionary<int, string>();
		item.Add(1, "hello");
		item.Add(2, "world");
		item.Add(3, "hello");

		var size = serializer.CalculateSize(item);
		var bytes = serializer.SerializeBytesLE(item);
		var item2 = serializer.DeserializeBytesLE(bytes);

		Assert.That(bytes.Length, Is.EqualTo(size));
		Assert.That(item2, Is.TypeOf<Dictionary<int, string>>());
		Assert.That(item2.Count, Is.EqualTo(3));
		Assert.That(item2[1], Is.EqualTo("hello"));
		Assert.That(item2[2], Is.EqualTo("world"));
		Assert.That(item2[3], Is.EqualTo("hello"));
		Assert.That(item2[1], Is.SameAs(item2[3]));
	}


	[Test]
	public void Dictionary_References_Shared_Complex() {
		var serializer = SerializerBuilder.FactoryAssemble<Dictionary<int, PrimitiveTestObject>>();

		var item = new Dictionary<int, PrimitiveTestObject>();
		item.Add(1, new PrimitiveTestObject { A = "hello" });
		item.Add(2, new PrimitiveTestObject { A = "world" });
		item.Add(3, item[1]);

		var bytes = serializer.SerializeBytesLE(item);
		var item2 = serializer.DeserializeBytesLE(bytes);

		var size = serializer.CalculateSize(item);
		Assert.That(bytes.Length, Is.EqualTo(size));
		Assert.That(item2, Is.TypeOf<Dictionary<int, PrimitiveTestObject>>());
		Assert.That(item2.Count, Is.EqualTo(3));
		Assert.That(item2[1].A, Is.EqualTo("hello"));
		Assert.That(item2[2].A, Is.EqualTo("world"));
		Assert.That(item2[3].A, Is.EqualTo("hello"));
		Assert.That(item2[1], Is.SameAs(item2[3]));
	}


	[Test]
	public void Array_References_Shared_Complex() {
		var serializer = SerializerBuilder.FactoryAssemble<PrimitiveTestObject[]>();

		var item = new PrimitiveTestObject[3];
		item[0] = new PrimitiveTestObject { A = "hello" };
		item[1] = new PrimitiveTestObject { A = "world" };
		item[2] = item[0];

		var bytes = serializer.SerializeBytesLE(item);
		var item2 = serializer.DeserializeBytesLE(bytes);

		var size = serializer.CalculateSize(item);
		Assert.That(bytes.Length, Is.EqualTo(size));
		Assert.That(item2, Is.TypeOf<PrimitiveTestObject[]>());
		Assert.That(item2.Count, Is.EqualTo(3));
		Assert.That(item2[0].A, Is.EqualTo("hello"));
		Assert.That(item2[1].A, Is.EqualTo("world"));
		Assert.That(item2[2].A, Is.EqualTo("hello"));
		Assert.That(item2[0], Is.SameAs(item2[2]));
	}


	[Test]
	public void NestedType_1() {
		var serializer = SerializerBuilder.FactoryAssemble<NestedType>();

		var item = new NestedType();

		var size = serializer.CalculateSize(item);
		var bytes = serializer.SerializeBytesLE(item);
		var item2 = serializer.DeserializeBytesLE(bytes);

		Assert.That(bytes.Length, Is.EqualTo(size));
		Assert.That(item2, Is.TypeOf<NestedType>());
		Assert.That(item2.Nested, Is.Null);
	}

	[Test]
	public void NestedType_2() {
		var serializer = SerializerBuilder.FactoryAssemble<NestedType>();

		var item = new NestedType();
		item.Nested = new NestedType();

		var size = serializer.CalculateSize(item);
		var bytes = serializer.SerializeBytesLE(item);
		var item2 = serializer.DeserializeBytesLE(bytes);

		Assert.That(bytes.Length, Is.EqualTo(size));
		Assert.That(item2, Is.TypeOf<NestedType>());
		Assert.That(item2.Nested, Is.Not.Null);
	}
}
