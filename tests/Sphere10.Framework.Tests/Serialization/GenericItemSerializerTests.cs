using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AutoFixture;
using FluentAssertions;
using NUnit.Framework;

namespace Sphere10.Framework.Tests
{
    public class GenericItemSerializerTests
    {
        private readonly IFixture _fixture = new Fixture();

        public GenericItemSerializerTests()
        {
            NullTestObject CreateNullTestObj()
            {
                return new()
                {
                    U = null,
                    V = null
                };
            }

            CollectionTestObject CreateCollectionTestObj()
            {
                return new()
                {
                    A = _fixture.Create<List<DateTime>>(),
                    B = _fixture.Build<ArrayList>()
                        .FromFactory(() => new ArrayList(_fixture.CreateMany<PrimitiveTestObject>().ToArray()))
                        .Create(),
                    C = _fixture.Create<PrimitiveTestObject[]>()
                };
            }

            _fixture.Customize<NullTestObject>(x => x.FromFactory(CreateNullTestObj));
            _fixture.Customize<CollectionTestObject>(x => x.FromFactory(CreateCollectionTestObj));

            GenericItemSerializer.Register<SubClassObj>();
            GenericItemSerializer.Register(typeof(CircularReferenceObj));
            GenericItemSerializer.Register(typeof(GenericTypeObj<,>));
            GenericItemSerializer.Register(typeof(SortedSet<>));
            GenericItemSerializer.Register<ObjectObj>();
            GenericItemSerializer.Register<GenericTypeObj>();
            GenericItemSerializer.Register<ReferenceTypeObject>();
            GenericItemSerializer.Register<PrimitiveTestObject>();
            GenericItemSerializer.Register<ValueTypeTestObject>();
            GenericItemSerializer.Register<CollectionTestObject>();
            GenericItemSerializer.Register<NullTestObject>();
        }

        [Test]
        public void PrimitiveSerializeDeserialize()
        {
            var item = _fixture.Create<PrimitiveTestObject>();
            var serializer = GenericItemSerializer<PrimitiveTestObject>.Default;
            using var memoryStream = new MemoryStream();
            var writer = new EndianBinaryWriter(EndianBitConverter.Little, memoryStream);
            var byteCount = serializer.Serialize(item, writer);

            Assert.AreEqual(memoryStream.Length, byteCount);

            memoryStream.Seek(0, SeekOrigin.Begin);
            var reader = new EndianBinaryReader(EndianBitConverter.Little, memoryStream);
            var deserializedItem = serializer.Deserialize(byteCount, reader);

            item.Should().BeEquivalentTo(deserializedItem);
        }

        [Test]
        public void ValueTypeSerializeDeserialize()
        {
            var item = _fixture.Create<ValueTypeTestObject>();
            var serializer = GenericItemSerializer<ValueTypeTestObject>.Default;
            using var memoryStream = new MemoryStream();
            var writer = new EndianBinaryWriter(EndianBitConverter.Little, memoryStream);
            var byteCount = serializer.Serialize(item, writer);

            Assert.AreEqual(memoryStream.Length, byteCount);

            memoryStream.Seek(0, SeekOrigin.Begin);
            var reader = new EndianBinaryReader(EndianBitConverter.Little, memoryStream);
            var deserializedItem = serializer.Deserialize(byteCount, reader);

            item.Should().BeEquivalentTo(deserializedItem);
        }

        [Test]
        public void CollectionSerializeDeserialize()
        {
            var item = _fixture.Create<CollectionTestObject>();
            var serializer = GenericItemSerializer<CollectionTestObject>.Default;
            using var memoryStream = new MemoryStream();
            var writer = new EndianBinaryWriter(EndianBitConverter.Little, memoryStream);
            var byteCount = serializer.Serialize(item, writer);

            Assert.AreEqual(memoryStream.Length, byteCount);

            memoryStream.Seek(0, SeekOrigin.Begin);
            var reader = new EndianBinaryReader(EndianBitConverter.Little, memoryStream);
            var deserializedItem = serializer.Deserialize(byteCount, reader);

            item.Should().BeEquivalentTo(deserializedItem);
        }

        [Test]
        public void NullObjectSerializeDeserialize()
        {
            var item = new NullTestObject()
            {
                U = null,
                V = null
            };

            var serializer = GenericItemSerializer<NullTestObject>.Default;

            using var memoryStream = new MemoryStream();
            var writer = new EndianBinaryWriter(EndianBitConverter.Little, memoryStream);
            var byteCount = serializer.Serialize(item, writer);

            Assert.AreEqual(memoryStream.Length, byteCount);

            memoryStream.Seek(0, SeekOrigin.Begin);
            var reader = new EndianBinaryReader(EndianBitConverter.Little, memoryStream);
            var deserializedItem = serializer.Deserialize(byteCount, reader);

            item.Should().BeEquivalentTo(deserializedItem);
        }

        [Test]
        public void ReferenceTypeSerializeDeserialize()
        {
            var item = _fixture.Create<ReferenceTypeObject>();

            var serializer = GenericItemSerializer<ReferenceTypeObject>.Default;

            using var memoryStream = new MemoryStream();
            var writer = new EndianBinaryWriter(EndianBitConverter.Little, memoryStream);
            var byteCount = serializer.Serialize(item, writer);

            Assert.AreEqual(memoryStream.Length, byteCount);

            memoryStream.Seek(0, SeekOrigin.Begin);
            var reader = new EndianBinaryReader(EndianBitConverter.Little, memoryStream);
            var deserializedItem = serializer.Deserialize(byteCount, reader);

            deserializedItem.Should().BeEquivalentTo(item);
        }

        [Test]
        public void GenericReferenceTypeSerializeDeserialize()
        {
            var item = _fixture.Create<GenericTypeObj>();
            var serializer = GenericItemSerializer<GenericTypeObj>.Default;

            using var memoryStream = new MemoryStream();
            var writer = new EndianBinaryWriter(EndianBitConverter.Little, memoryStream);
            var byteCount = serializer.Serialize(item, writer);

            Assert.AreEqual(memoryStream.Length, byteCount);

            memoryStream.Seek(0, SeekOrigin.Begin);
            var reader = new EndianBinaryReader(EndianBitConverter.Little, memoryStream);
            var deserializedItem = serializer.Deserialize(byteCount, reader);

            deserializedItem.Should().BeEquivalentTo(item);
        }

        [Test]
        public void CircularReferenceObj()
        {
            var parent = new CircularReferenceObj
            {
                A = new CircularReferenceObj
                {
                    B = _fixture.Create<PrimitiveTestObject>()
                },
                B = _fixture.Create<PrimitiveTestObject>()
            };

            parent.A.A = parent;
            
            var serializer = GenericItemSerializer<CircularReferenceObj>.Default;

            using var memoryStream = new MemoryStream();
            var writer = new EndianBinaryWriter(EndianBitConverter.Little, memoryStream);
            var byteCount = serializer.Serialize(parent, writer);

            Assert.AreEqual(memoryStream.Length, byteCount);

            memoryStream.Seek(0, SeekOrigin.Begin);
            var reader = new EndianBinaryReader(EndianBitConverter.Little, memoryStream);
            var deserializedItem = serializer.Deserialize(byteCount, reader);

            deserializedItem
                .Should()
                .BeEquivalentTo(parent, x => x.IgnoringCyclicReferences());
        }

        [Test]
        public void ObjectTypePropertiesSerialized()
        {
            var item = new ObjectObj
            {
                A = new List<int> {1, 2, 3, 4},
                B = null,
                C = _fixture.Create<ReferenceTypeObject>(),
                D = false
            };

            var serializer = GenericItemSerializer<ObjectObj>.Default;

            using var memoryStream = new MemoryStream();
            var writer = new EndianBinaryWriter(EndianBitConverter.Little, memoryStream);
            var byteCount = serializer.Serialize(item, writer);

            Assert.AreEqual(memoryStream.Length, byteCount);

            memoryStream.Seek(0, SeekOrigin.Begin);
            var reader = new EndianBinaryReader(EndianBitConverter.Little, memoryStream);
            var deserializedItem = serializer.Deserialize(byteCount, reader);

            deserializedItem.Should().BeEquivalentTo(item);
        }

        [Test]
        public void SubClassInCollectionTest()
        {
            List<PrimitiveTestObject> list = new List<PrimitiveTestObject>()
            {
                _fixture.Create<PrimitiveTestObject>(),
                _fixture.Create<SubClassObj>()
            };
            
            var serializer = GenericItemSerializer<List<PrimitiveTestObject>>.Default;
            
            var stream = new MemoryStream();
            serializer.Serialize(list, new EndianBinaryWriter(EndianBitConverter.Little, stream));
            stream.Seek(0, SeekOrigin.Begin);
            var reader = new EndianBinaryReader(EndianBitConverter.Little, stream);
            var item = serializer.Deserialize(0, reader);

            item.Should().BeEquivalentTo(list);
        }
        
        [Test]
        public void SubClassSerializeDeserialize()
        {
            var serializer = GenericItemSerializer<PrimitiveTestObject>.Default;
            var item = _fixture.Create<SubClassObj>();
            
            var stream = new MemoryStream();
            serializer.Serialize(item, new EndianBinaryWriter(EndianBitConverter.Little, stream));
            stream.Seek(0, SeekOrigin.Begin);
            var reader = new EndianBinaryReader(EndianBitConverter.Little, stream);
            var deserialized = serializer.Deserialize(0, reader);

            deserialized.Should().BeEquivalentTo(item);
        }
    }

    internal class PrimitiveTestObject
    {
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

    internal class ValueTypeTestObject
    {
        public int? A { get; set; }
        public DateTime B { get; set; }
        public DateTime? C { get; set; }
        public DateTimeOffset D { get; set; }
        public DateTimeOffset? E { get; set; }
    }

    internal class CollectionTestObject
    {
        public List<DateTime> A { get; set; }

        public ArrayList B { get; set; }

        public PrimitiveTestObject[] C { get; set; }

        public Dictionary<int, PrimitiveTestObject> D { get; set; }

        public List<int> E { get; set; }

        public byte[] F { get; set; }

        public List<PrimitiveTestObject> G { get; set; }
        
        public SortedSet<bool> H { get; set; }
    }

    internal class NullTestObject
    {
        public object U { get; set; }

        public string V { get; set; }
    }

    internal class ReferenceTypeObject
    {
        public PrimitiveTestObject X { get; set; }

        public CollectionTestObject Y { get; set; }

        public NullTestObject Z { get; set; }
    }

    internal class GenericTypeObj
    {
        public GenericTypeObj<ReferenceTypeObject, CollectionTestObject> A { get; set; }
    }

    internal class GenericTypeObj<T1, T2>
    {
        public T1 A { get; set; }

        public T2 B { get; set; }
    }

    internal class CircularReferenceObj
    {
        public CircularReferenceObj A { get; set; }

        public PrimitiveTestObject B { get; set; }
    }

    internal class ObjectObj
    {
        public object A { get; set; }
        public object B { get; set; }
        public object C { get; set; }
        public object D { get; set; }
    }

    internal class SubClassObj : PrimitiveTestObject
    {
        public int  X { get; set; }
    }
}