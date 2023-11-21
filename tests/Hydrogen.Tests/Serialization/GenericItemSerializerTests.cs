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
using System.IO;
using System.Linq;
using AutoFixture;
using FluentAssertions;
using NUnit.Framework;

namespace Hydrogen.Tests;

public class GenericItemSerializerTests {
	private readonly IFixture _fixture = new Fixture();

	[Test]
	public void PrimitiveSerializeDeserialize() {
		var item = _fixture.Create<PrimitiveTestObject>();
		var serializer = SerializerFactory.Default.GetSerializer<PrimitiveTestObject>();
		var bytes = serializer.SerializeBytesLE(item);
		var deserializedItem = serializer.DeserializeBytesLE(bytes);

		item.Should().BeEquivalentTo(deserializedItem);
	}

	[Test]
	public void ValueTypeSerializeDeserialize() {
		var item = _fixture.Create<ValueTypeTestObject>();
		var serializer = ItemSerializer<ValueTypeTestObject>.Default;
		using var memoryStream = new MemoryStream();
		var writer = new EndianBinaryWriter(EndianBitConverter.Little, memoryStream);
		var byteCount = serializer.SerializeReturnSize(item, writer);

		Assert.AreEqual(memoryStream.Length, byteCount);

		memoryStream.Seek(0, SeekOrigin.Begin);
		var reader = new EndianBinaryReader(EndianBitConverter.Little, memoryStream);
		var deserializedItem = serializer.Deserialize(reader);

		item.Should().BeEquivalentTo(deserializedItem);
	}

	[Test]
	public void CollectionSerializeDeserialize() {
		var item = _fixture.Create<CollectionTestObject>();
		var serializer = ItemSerializer<CollectionTestObject>.Default;
		using var memoryStream = new MemoryStream();
		var writer = new EndianBinaryWriter(EndianBitConverter.Little, memoryStream);
		var byteCount = serializer.SerializeReturnSize(item, writer);
		var hash = Hashers.Hash(CHF.SHA2_256, memoryStream.ToArray());

		Assert.AreEqual(memoryStream.Length, byteCount);

		memoryStream.Seek(0, SeekOrigin.Begin);
		var reader = new EndianBinaryReader(EndianBitConverter.Little, memoryStream);
		var deserializedItem = serializer.Deserialize(reader);

		item.Should().BeEquivalentTo(deserializedItem);
	}

	[Test]
	public void NullObjectSerializeDeserialize() {
		var item = new NullTestObject {
			U = null,
			V = null
		};

		var serializer = ItemSerializer<NullTestObject>.Default;

		using var memoryStream = new MemoryStream();
		var writer = new EndianBinaryWriter(EndianBitConverter.Little, memoryStream);
		var byteCount = serializer.SerializeReturnSize(item, writer);

		Assert.AreEqual(memoryStream.Length, byteCount);

		memoryStream.Seek(0, SeekOrigin.Begin);
		var reader = new EndianBinaryReader(EndianBitConverter.Little, memoryStream);
		var deserializedItem = serializer.Deserialize(reader);

		item.Should().BeEquivalentTo(deserializedItem);
	}

	[Test]
	public void ReferenceTypeSerializeDeserialize() {
		var item = _fixture.Create<ReferenceTypeObject>();

		IItemSerializer<ReferenceTypeObject> serializer = ItemSerializer<ReferenceTypeObject>.Default;

		using var memoryStream = new MemoryStream();
		var writer = new EndianBinaryWriter(EndianBitConverter.Little, memoryStream);
		var byteCount = serializer.SerializeReturnSize(item, writer);

		Assert.AreEqual(memoryStream.Length, byteCount);

		memoryStream.Seek(0, SeekOrigin.Begin);
		var reader = new EndianBinaryReader(EndianBitConverter.Little, memoryStream);
		IItemSerializer<ReferenceTypeObject> deserializer = ItemSerializer<ReferenceTypeObject>.Default;
		var deserializedItem = deserializer.Deserialize(reader);

		deserializedItem.Should().BeEquivalentTo(item);
	}

	[Test]
	public void GenericReferenceTypeSerializeDeserialize() {
		var item = _fixture.Create<GenericTypeObj>();
		IItemSerializer<GenericTypeObj> serializer = ItemSerializer<GenericTypeObj>.Default;

		using var memoryStream = new MemoryStream();
		var writer = new EndianBinaryWriter(EndianBitConverter.Little, memoryStream);
		var byteCount = serializer.SerializeReturnSize(item, writer);

		Assert.AreEqual(memoryStream.Length, byteCount);

		memoryStream.Seek(0, SeekOrigin.Begin);
		var reader = new EndianBinaryReader(EndianBitConverter.Little, memoryStream);
		IItemSerializer<GenericTypeObj> deserializer = ItemSerializer<GenericTypeObj>.Default;
		var deserializedItem = deserializer.Deserialize(reader);

		deserializedItem.Should().BeEquivalentTo(item);
	}

	[Test]
	public void CircularReferenceObj() {
		var parent = new CircularReferenceObj {
			A = new CircularReferenceObj {
				B = _fixture.Create<PrimitiveTestObject>()
			},
			B = _fixture.Create<PrimitiveTestObject>()
		};

		parent.A.A = parent;

		IItemSerializer<CircularReferenceObj> serializer = ItemSerializer<CircularReferenceObj>.Default;

		using var memoryStream = new MemoryStream();
		var writer = new EndianBinaryWriter(EndianBitConverter.Little, memoryStream);
		var byteCount = serializer.SerializeReturnSize(parent, writer);

		Assert.AreEqual(memoryStream.Length, byteCount);

		memoryStream.Seek(0, SeekOrigin.Begin);
		var reader = new EndianBinaryReader(EndianBitConverter.Little, memoryStream);

		IItemSerializer<CircularReferenceObj> deserializer = ItemSerializer<CircularReferenceObj>.Default;
		var deserializedItem = deserializer.Deserialize(reader);

		deserializedItem
			.Should()
			.BeEquivalentTo(parent, x => x.IgnoringCyclicReferences());

		Assert.That(deserializedItem.A.A, Is.SameAs(deserializedItem));
	}

	[Test]
	public void ObjectTypePropertiesSerialized() {
		var item = new ObjectObj {
			A = new List<int> { 1, 2, 3, 4 },
			B = null,
			C = _fixture.Create<ReferenceTypeObject>(),
			D = false,
			E = _fixture.Create<byte[]>()
		};

		var factory = new SerializerFactory(SerializerFactory.Default);
		factory.RegisterAutoBuild<ReferenceTypeObject>();
		var serializer =  factory.GetSerializer<ObjectObj>();
		using var memoryStream = new MemoryStream();
		var writer = new EndianBinaryWriter(EndianBitConverter.Little, memoryStream);
		var byteCount = serializer.SerializeReturnSize(item, writer);

		Assert.AreEqual(memoryStream.Length, byteCount);

		memoryStream.Seek(0, SeekOrigin.Begin);
		var reader = new EndianBinaryReader(EndianBitConverter.Little, memoryStream);


		var deserializedItem = serializer.Deserialize(reader);

		deserializedItem.Should().BeEquivalentTo(item);
	}

	[Test]
	public void SubClassInCollectionTest() {
		List<PrimitiveTestObject> list = new List<PrimitiveTestObject>() {
			_fixture.Create<PrimitiveTestObject>(),
			_fixture.Create<SubClassObj>()
		};

		IItemSerializer<List<PrimitiveTestObject>> serializer = ItemSerializer<List<PrimitiveTestObject>>.Default;

		var stream = new MemoryStream();
		var len = serializer.SerializeReturnSize(list, new EndianBinaryWriter(EndianBitConverter.Little, stream));
		stream.Seek(0, SeekOrigin.Begin);
		var reader = new EndianBinaryReader(EndianBitConverter.Little, stream);
		IItemSerializer<List<PrimitiveTestObject>> deserializer = ItemSerializer<List<PrimitiveTestObject>>.Default;

		// Subclass instance must be registered
		var item = deserializer.Deserialize(reader);

		item.Should().BeEquivalentTo(list);
	}

	[Test]
	public void SubClassSerializeDeserialize() {
		IItemSerializer<SubClassObj> serializer = ItemSerializer<SubClassObj>.Default;
		var item = _fixture.Create<SubClassObj>();

		var stream = new MemoryStream();
		var len = serializer.SerializeReturnSize(item, new EndianBinaryWriter(EndianBitConverter.Little, stream));
		stream.Seek(0, SeekOrigin.Begin);

		var reader = new EndianBinaryReader(EndianBitConverter.Little, stream);
		IItemSerializer<SubClassObj> deserializer = ItemSerializer<SubClassObj>.Default;

		//Subclass instance must be registered during deserialization. 
		var deserialized = deserializer.Deserialize(reader);

		deserialized.Should().BeEquivalentTo(item);
	}


	[Test]
	public void CalculateSizeOfObject() {
		var item = _fixture.Create<ReferenceTypeObject>();
		var serializer = ItemSerializer<ReferenceTypeObject>.Default;
		var calculatedSize = serializer.CalculateSize(item);
		var serialized = serializer.SerializeBytesLE(item);
		Assert.AreEqual(calculatedSize, serialized.Length);
	}

	[Test]
	public void CalculateSizeOfObject_Bug() {
		var item = new ReferenceTypeObject();
		item.Y = new CollectionTestObject() {
			F = new byte[] { 1, 2, 3 },
		};
		var serializer = ItemSerializer<ReferenceTypeObject>.Default;
		var  calculatedSize = serializer.CalculateSize(item);
		var serialized = serializer.SerializeBytesLE(item);
		Assert.AreEqual(calculatedSize, serialized.Length);
	}

	[Test]
	public void CalculateTotalSize() {
		var items = _fixture.CreateMany<ReferenceTypeObject>();
		var serializer = ItemSerializer<ReferenceTypeObject>.Default;
		using var memoryStream = new MemoryStream();
		var writer = new EndianBinaryWriter(EndianBitConverter.Little, memoryStream);

		var calculatedTotalSize = serializer.CalculateTotalSize(items, true, out var sizes);
		Assert.AreEqual(0, memoryStream.Length);

		var serializedTotal = 0L;
		foreach (var item in items) {
			serializedTotal += serializer.SerializeReturnSize(item, writer);
		}

		Assert.AreEqual(serializedTotal, calculatedTotalSize);
		Assert.AreEqual(memoryStream.Length, calculatedTotalSize);
		Assert.AreEqual(items.Count(), sizes.Length);
		Assert.AreEqual(calculatedTotalSize, sizes.Sum(x => x));
	}

	[Test]
	public void EnumSerializeDeserialize() {
		var item = _fixture.Create<EnumObj>();
		IItemSerializer<EnumObj> serializer = ItemSerializer<EnumObj>.Default;

		using var memoryStream = new MemoryStream();
		var writer = new EndianBinaryWriter(EndianBitConverter.Little, memoryStream);
		var size = serializer.CalculateSize(item);
		Assert.AreEqual(0, memoryStream.Length);

		IItemSerializer<EnumObj> deserializer = ItemSerializer<EnumObj>.Default;
		var serializedSize = deserializer.SerializeReturnSize(item, writer);
		Assert.AreEqual(serializedSize, size);
	}
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

	public Dictionary<int, PrimitiveTestObject> D { get; set; }

	public List<int> E { get; set; }

	public byte[] F { get; set; }

	public List<PrimitiveTestObject> G { get; set; }

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
