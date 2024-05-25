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
		var itemSerializer = factory.GetPureSerializer<int>();
		var serializerSerializer = new SerializerSerializer(factory);
		var size = serializerSerializer.CalculateSize(itemSerializer);
		var serializedSerializer = serializerSerializer.SerializeBytesLE(itemSerializer);
		var deserializedSerializer = serializerSerializer.DeserializeBytesLE(serializedSerializer);
		var itemSerializer2 = deserializedSerializer as IItemSerializer<int>;
		Assert.That(serializedSerializer.Length, Is.EqualTo(size));
		Assert.That(itemSerializer2, Is.Not.Null);
	}

	[Test]
	public void ListOfIntSerializer() {
		var factory = new SerializerFactory();
		factory.Register(PrimitiveSerializer<int>.Instance);
		factory.Register(typeof(List<>), typeof(ListSerializer<>));
		var itemSerializer = factory.GetPureSerializer<List<int>>();
		var serializerSerializer = new SerializerSerializer(factory);
		var size = serializerSerializer.CalculateSize(itemSerializer);
		var serializedSerializer = serializerSerializer.SerializeBytesLE(itemSerializer);
		var deserializedSerializer = serializerSerializer.DeserializeBytesLE(serializedSerializer);
		var itemSerializer2 = deserializedSerializer as IItemSerializer<List<int>>;
		Assert.That(serializedSerializer.Length, Is.EqualTo(size));
		Assert.That(itemSerializer2, Is.Not.Null);
	}

	[Test]
	public void ComplexSerializer() {
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
		var itemSerializer = factory.GetPureSerializer<List<KeyValuePair<List<int>, KeyValuePair<float, List<int>>>>>();
		var serializerSerializer = new SerializerSerializer(factory);
		var size = serializerSerializer.CalculateSize(itemSerializer);
		var serializedSerializer = serializerSerializer.SerializeBytesLE(itemSerializer);
		var deserializedSerializer = serializerSerializer.DeserializeBytesLE(serializedSerializer);
		var itemSerializer2 = deserializedSerializer as IItemSerializer<List<KeyValuePair<List<int>, KeyValuePair<float, List<int>>>>>;
		Assert.That(serializedSerializer.Length, Is.EqualTo(size));
		Assert.That(itemSerializer2, Is.Not.Null);
	}
	
	[Test]
	public void OpenComplexAndSimilarClosedSpecificSerializer() {
		// This te
		var factory = new SerializerFactory();
		factory.Register(PrimitiveSerializer<int>.Instance); // 0
		factory.Register(PrimitiveSerializer<float>.Instance); // 1
		factory.Register(typeof(List<>), typeof(ListSerializer<>)); // 2
		factory.Register(typeof(KeyValuePair<,>), typeof(KeyValuePairSerializer<,>)); // 3
		
		IItemSerializer<List<KeyValuePair<List<int>, KeyValuePair<int, List<int>>>>> specificSerializer = new ListSerializer<KeyValuePair<List<int>, KeyValuePair<int, List<int>>>>(new KeyValuePairSerializer<List<int>, KeyValuePair<int, List<int>>>(new ListSerializer<int>(PrimitiveSerializer<int>.Instance), new KeyValuePairSerializer<int, List<int>>(PrimitiveSerializer<int>.Instance, new ListSerializer<int>(PrimitiveSerializer<int>.Instance))));
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
		var itemSerializer1 = factory.GetPureSerializer<List<KeyValuePair<List<int>, KeyValuePair<float, List<int>>>>>();
		var size1 = serializerSerializer.CalculateSize(itemSerializer1);
		var firstSerializerBytes = serializerSerializer.SerializeBytesLE(itemSerializer1);
		Assert.That(firstSerializerBytes.Length, Is.EqualTo(size1));
		Assert.That(firstSerializerBytes.Length, Is.EqualTo(8));  // there were 8 serializers referenced in the first serializer (stored as 8 cvarints)
		var firstSerializer = serializerSerializer.DeserializeBytesLE( firstSerializerBytes) as IItemSerializer<List<KeyValuePair<List<int>, KeyValuePair<float, List<int>>>>>;
		Assert.That(firstSerializer, Is.Not.Null);

		var itemSerializer2 = factory.GetPureSerializer<List<KeyValuePair<List<int>, KeyValuePair<int, List<int>>>>>();
		var size2 = serializerSerializer.CalculateSize(itemSerializer2);
		var secondSerializerBytes = serializerSerializer.SerializeBytesLE(itemSerializer2);
		Assert.That(secondSerializerBytes.Length, Is.EqualTo(size2));
		Assert.That(secondSerializerBytes.Length, Is.EqualTo(1));  // there was 1 serializer referenced in the first serializer 
		Assert.That( CVarInt.Read(secondSerializerBytes), Is.EqualTo(SerializerFactory.PermanentTypeCodeStartDefault + 4));  // the serializer was the 4th serializer registered in the factory
		var secondSerializer = serializerSerializer.DeserializeBytesLE( secondSerializerBytes) as IItemSerializer<List<KeyValuePair<List<int>, KeyValuePair<int, List<int>>>>>;
		Assert.That(secondSerializer, Is.Not.Null);
	}	
}
