// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System.Collections.Generic;
using NUnit.Framework;

namespace Hydrogen.Tests;

[TestFixture]
[Parallelizable]
public class SerializerFactoryTests {

	[Test]
	public void Primitive() {
		var factory = new SerializerFactory();
		factory.Register(PrimitiveSerializer<int>.Instance);
		Assert.That(factory.GetSerializer<int>(), Is.SameAs(PrimitiveSerializer<int>.Instance));
	}

	[Test]
	public void OpenGenericSerializer_1() {
		var factory = new SerializerFactory();
		factory.Register(PrimitiveSerializer<int>.Instance);
		factory.Register(typeof(IList<>), typeof(ListInterfaceSerializer<>));
		Assert.That(factory.GetSerializer<IList<int>>(), Is.TypeOf<ListInterfaceSerializer<int>>());
	}

	[Test]
	public void OpenGenericSerializer_2() {
		var factory = new SerializerFactory();
		factory.Register(PrimitiveSerializer<int>.Instance);
		factory.Register(PrimitiveSerializer<float>.Instance);
		factory.Register(typeof(IList<>), typeof(ListInterfaceSerializer<>));
		factory.Register(typeof(KeyValuePair<,>), typeof(KeyValuePairSerializer<,>));
		Assert.That(factory.GetSerializer<IList<KeyValuePair<int, float>>>(), Is.TypeOf<ListInterfaceSerializer<KeyValuePair<int, float>>>());
	}

	[Test]
	public void OpenGenericSerializer_3() {
		var factory = new SerializerFactory();
		factory.Register(PrimitiveSerializer<int>.Instance);
		factory.Register(typeof(IList<>), typeof(ListInterfaceSerializer<>));
		Assert.That(factory.GetSerializer<IList<IList<int>>>, Is.TypeOf<ListInterfaceSerializer<IList<int>>>());
	}

	[Test]
	public void OpenGenericSerializer_4() {
		var factory = new SerializerFactory();
		factory.Register(PrimitiveSerializer<int>.Instance);
		factory.Register(PrimitiveSerializer<float>.Instance);
		factory.Register(typeof(IList<>), typeof(ListInterfaceSerializer<>));
		factory.Register(typeof(KeyValuePair<,>), typeof(KeyValuePairSerializer<,>));
		Assert.That(factory.GetSerializer<IList<KeyValuePair<IList<int>, KeyValuePair<float, IList<int>>>>>(), Is.TypeOf<ListInterfaceSerializer<KeyValuePair<IList<int>, KeyValuePair<float, IList<int>>>>>());
	}

	[Test]
	public void GetSerializerHierarchy_Open() {
		var factory = new SerializerFactory();
		factory.Register(typeof(IList<>), typeof(ListInterfaceSerializer<>));  // 0
		factory.Register(PrimitiveSerializer<int>.Instance);  // 1
		CollectionAssert.AreEqual(factory.GetSerializerHierarchy(typeof(IList<int>)).Flatten(), new[] { 0, 1});
	}

	[Test]
	public void GetSerializerHierarchy_Open_Complex() {
		var factory = new SerializerFactory();
		factory.Register(PrimitiveSerializer<int>.Instance); // 0
		factory.Register(PrimitiveSerializer<float>.Instance); // 1
		factory.Register(typeof(IList<>), typeof(ListInterfaceSerializer<>)); // 2
		factory.Register(typeof(KeyValuePair<,>), typeof(KeyValuePairSerializer<,>)); // 3
		// IList< 2
		//	KeyValuePair< 3
		//		IList< 2
		//			int>, 0
		//		KeyValuePair< 3
		//			float, 1 
		//			IList< 2
		//				int>>>> 0
		CollectionAssert.AreEqual(factory.GetSerializerHierarchy(typeof(IList<KeyValuePair<IList<int>, KeyValuePair<float, IList<int>>>>)).Flatten(), new[] { 2, 3, 2, 0, 3, 1, 2, 0 });

	}

	[Test]
	public void GetSerializerHierarchy_Closed_Complex() {
		var factory = new SerializerFactory();
		factory.Register(PrimitiveSerializer<int>.Instance); // 0
		factory.Register(PrimitiveSerializer<float>.Instance); // 1
		factory.Register(typeof(IList<>), typeof(ListInterfaceSerializer<>)); // 2
		factory.Register(typeof(KeyValuePair<,>), typeof(KeyValuePairSerializer<,>)); // 3

		var instance = new ListInterfaceSerializer<KeyValuePair<IList<int>, KeyValuePair<float, IList<int>>>>(
			new KeyValuePairSerializer<IList<int>, KeyValuePair<float, IList<int>>>(
				new ListInterfaceSerializer<int>(PrimitiveSerializer<int>.Instance),
				new KeyValuePairSerializer<float, IList<int>>(
					PrimitiveSerializer<float>.Instance,
					new ListInterfaceSerializer<int>(PrimitiveSerializer<int>.Instance)
				)
			)
		);
		factory.Register(instance); // 4 (closed specific instance)

		CollectionAssert.AreEqual(factory.GetSerializerHierarchy(typeof(IList<KeyValuePair<IList<int>, KeyValuePair<float, IList<int>>>>)).Flatten(), new[] { 4 });

	}

	[Test]
	public void FromSerializerHierarchy_Open() {
		var factory = new SerializerFactory();
		factory.Register(typeof(IList<>), typeof(ListInterfaceSerializer<>));  // 0
		factory.Register(PrimitiveSerializer<int>.Instance);  // 1
		var serializerHierarchy = factory.GetSerializerHierarchy(typeof(IList<int>));
		var serializer = factory.FromSerializerHierarchy(serializerHierarchy);
		Assert.That(serializer, Is.TypeOf<ListInterfaceSerializer<int>>());
	}

	[Test]
	public void FromSerializerHierarchy_Open_Complex() {
		var factory = new SerializerFactory();
		factory.Register(PrimitiveSerializer<int>.Instance); // 0
		factory.Register(PrimitiveSerializer<float>.Instance); // 1
		factory.Register(typeof(IList<>), typeof(ListInterfaceSerializer<>)); // 2
		factory.Register(typeof(KeyValuePair<,>), typeof(KeyValuePairSerializer<,>)); // 3
		// IList< 2
		//	KeyValuePair< 3
		//		IList< 2
		//			int>, 0
		//		KeyValuePair< 3
		//			float, 1 
		//			IList< 2
		//				int>>>> 0
		var serializerHierarchy = factory.GetSerializerHierarchy(typeof(IList<KeyValuePair<IList<int>, KeyValuePair<float, IList<int>>>>));
		var serializer = factory.FromSerializerHierarchy(serializerHierarchy);
		Assert.That(serializer, Is.TypeOf<ListInterfaceSerializer<KeyValuePair<IList<int>, KeyValuePair<float, IList<int>>>>>());
	}

	[Test]
	public void FromSerializerHierarchy_Closed_Complex() {
		var factory = new SerializerFactory();
		factory.Register(PrimitiveSerializer<int>.Instance); // 0
		factory.Register(PrimitiveSerializer<float>.Instance); // 1
		factory.Register(typeof(IList<>), typeof(ListInterfaceSerializer<>)); // 2
		factory.Register(typeof(KeyValuePair<,>), typeof(KeyValuePairSerializer<,>)); // 3

		var instance = new ListInterfaceSerializer<KeyValuePair<IList<int>, KeyValuePair<float, IList<int>>>>(
			new KeyValuePairSerializer<IList<int>, KeyValuePair<float, IList<int>>>(
				new ListInterfaceSerializer<int>(PrimitiveSerializer<int>.Instance),
				new KeyValuePairSerializer<float, IList<int>>(
					PrimitiveSerializer<float>.Instance,
					new ListInterfaceSerializer<int>(PrimitiveSerializer<int>.Instance)
				)
			)
		);
		factory.Register(instance); // 4 (closed specific instance)

		CollectionAssert.AreEqual(factory.GetSerializerHierarchy(typeof(IList<KeyValuePair<IList<int>, KeyValuePair<float, IList<int>>>>)).Flatten(), new[] { 4 });

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
		factory.Register(typeof(IList<>), typeof(ListInterfaceSerializer<>));
		Assert.That(() => factory.Register(typeof(IList<>), typeof(ListInterfaceSerializer<>)), Throws.InvalidOperationException);
	}

	[Test]
	public void RegisterOpenSerializerWithoutComponentFails() {
		var factory = new SerializerFactory();
		
	//	Assert.That(() => factory.Register(typeof(List<int>), typeof( PrimitiveSerializer<int>.Instance), Throws.InvalidOperationException);
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
		Assert.That(hierarchy.Flatten(), Is.EqualTo(new[] { 0, 1 }));
		var serializer = factory.FromSerializerHierarchy(hierarchy);
		Assert.That(serializer, Is.TypeOf<ArraySerializer<int>>());
	}

	[Test]
	public void ResolveNotSpecializedByteArray() {
		var factory = new SerializerFactory();
		factory.Register(typeof(System.Array), typeof(ArraySerializer<>)); // 0
		factory.Register(PrimitiveSerializer<int>.Instance); // 1
		factory.Register(ByteArraySerializer.Instance); // 2 (special for byte[])
		var hierarchy = factory.GetSerializerHierarchy(typeof(int[]));
		Assert.That(hierarchy.Flatten(), Is.EqualTo(new[] { 0, 1 }));
		var serializer = factory.FromSerializerHierarchy(hierarchy);
		Assert.That(serializer, Is.TypeOf<ArraySerializer<int>>());
	}

	[Test]
	public void ResolveSpecializedByteArray() {
		var factory = new SerializerFactory();
		factory.Register(typeof(System.Array), typeof(ArraySerializer<>)); // 0
		factory.Register(PrimitiveSerializer<int>.Instance); // 1
		factory.Register(ByteArraySerializer.Instance); // 2 (special for byte[])
		var hierarchy = factory.GetSerializerHierarchy(typeof(byte[]));
		Assert.That(hierarchy.Flatten(), Is.EqualTo(new[] { 2 }));
		var serializer = factory.FromSerializerHierarchy(hierarchy);
		Assert.That(serializer, Is.TypeOf<ByteArraySerializer>());
	}


}
