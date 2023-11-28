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
public class SerializerSerializerTests {


	[Test]
	public void IntSerializer() {
		var factory = new SerializerFactory();
		factory.Register(PrimitiveSerializer<int>.Instance);
		var itemSerializer = factory.GetRegisteredSerializer<int>();
		var serializerSerializer = new SerializerSerializer(factory);
		var serializedSerializer = serializerSerializer.SerializeBytesLE(itemSerializer);
		var deserializedSerializer = serializerSerializer.DeserializeBytesLE(serializedSerializer);
		var itemSerializer2 = deserializedSerializer as IItemSerializer<int>;
		Assert.That(itemSerializer2, Is.Not.Null);
	}

	[Test]
	public void ListOfIntSerializer() {
		var factory = new SerializerFactory();
		factory.Register(PrimitiveSerializer<int>.Instance);
		factory.Register(typeof(IList<>), typeof(ListInterfaceSerializer<>));
		var itemSerializer = factory.GetRegisteredSerializer<IList<int>>();
		var serializerSerializer = new SerializerSerializer(factory);
		var serializedSerializer = serializerSerializer.SerializeBytesLE(itemSerializer);
		var deserializedSerializer = serializerSerializer.DeserializeBytesLE(serializedSerializer);
		var itemSerializer2 = deserializedSerializer as IItemSerializer<IList<int>>;
		Assert.That(itemSerializer2, Is.Not.Null);
	}

	[Test]
	public void ComplexSerializer() {
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
		var itemSerializer = factory.GetRegisteredSerializer<IList<KeyValuePair<IList<int>, KeyValuePair<float, IList<int>>>>>();
		var serializerSerializer = new SerializerSerializer(factory);
		var serializedSerializer = serializerSerializer.SerializeBytesLE(itemSerializer);
		var deserializedSerializer = serializerSerializer.DeserializeBytesLE(serializedSerializer);
		var itemSerializer2 = deserializedSerializer as IItemSerializer<IList<KeyValuePair<IList<int>, KeyValuePair<float, IList<int>>>>>;
		Assert.That(itemSerializer2, Is.Not.Null);
	}
	
	[Test]
	public void OpenComplexAndSimilarClosedSpecificSerializer() {
		// This te
		var factory = new SerializerFactory();
		factory.Register(PrimitiveSerializer<int>.Instance); // 0
		factory.Register(PrimitiveSerializer<float>.Instance); // 1
		factory.Register(typeof(IList<>), typeof(ListInterfaceSerializer<>)); // 2
		factory.Register(typeof(KeyValuePair<,>), typeof(KeyValuePairSerializer<,>)); // 3
		
		IItemSerializer<IList<KeyValuePair<IList<int>, KeyValuePair<int, IList<int>>>>> specificSerializer = new ListInterfaceSerializer<KeyValuePair<IList<int>, KeyValuePair<int, IList<int>>>>(new KeyValuePairSerializer<IList<int>, KeyValuePair<int, IList<int>>>(new ListInterfaceSerializer<int>(PrimitiveSerializer<int>.Instance), new KeyValuePairSerializer<int, IList<int>>(PrimitiveSerializer<int>.Instance, new ListInterfaceSerializer<int>(PrimitiveSerializer<int>.Instance))));
		factory.Register(specificSerializer); // 4

		// First:
		// IList< 2
		//	KeyValuePair< 3
		//		IList< 2
		//			int>, 0
		//		KeyValuePair< 3
		//			float, 1 
		//			IList< 2
		//				int>>>> 0

		// Second:
		// IList< 
		//	KeyValuePair< 
		//		IList< 
		//			int>, 
		//		KeyValuePair< 
		//			int, 
		//			IList< 
		//				int>>>>  = 4

		
		var serializerSerializer = new SerializerSerializer(factory);
		var firstSerializerBytes = serializerSerializer.SerializeBytesLE(
			factory.GetRegisteredSerializer<IList<KeyValuePair<IList<int>, KeyValuePair<float, IList<int>>>>>()
		);
		Assert.That(firstSerializerBytes.Length, Is.EqualTo(8));  // there were 8 serializers referenced in the first serializer (stored as 8 cvarints)
		var firstSerializer = serializerSerializer.DeserializeBytesLE( firstSerializerBytes) as IItemSerializer<IList<KeyValuePair<IList<int>, KeyValuePair<float, IList<int>>>>>;
		Assert.That(firstSerializer, Is.Not.Null);

		var secondSerializerBytes = serializerSerializer.SerializeBytesLE(
			factory.GetRegisteredSerializer<IList<KeyValuePair<IList<int>, KeyValuePair<int, IList<int>>>>>()
		);
		Assert.That(secondSerializerBytes.Length, Is.EqualTo(1));  // there was 1 serializer referenced in the first serializer 
		Assert.That( CVarInt.Read(secondSerializerBytes), Is.EqualTo(SerializerFactory.PermanentTypeCodeStartDefault + 4));  // the serializer was the 4th serializer registered in the factory
		var secondSerializer = serializerSerializer.DeserializeBytesLE( secondSerializerBytes) as IItemSerializer<IList<KeyValuePair<IList<int>, KeyValuePair<int, IList<int>>>>>;
		Assert.That(secondSerializer, Is.Not.Null);
	}	
}
