using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AutoFixture;
using FluentAssertions;
using Microsoft.Diagnostics.Tracing.Parsers.IIS_Trace;
using NUnit.Framework;

namespace Sphere10.Framework.Tests
{
    public class GenericItemSerializerTests
    {
        private readonly IFixture _fixture = new Fixture();

        public GenericItemSerializerTests()
        {
            GenericItemSerializer.Register<EnumObj>();
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
            var size = serializer.CalculateSize(item);
            var byteCount = serializer.Serialize(item, writer);

            Assert.AreEqual(memoryStream.Length, byteCount);
            Assert.AreEqual(size, byteCount);

            memoryStream.Seek(0, SeekOrigin.Begin);
            var reader = new EndianBinaryReader(EndianBitConverter.Little, memoryStream);
            var deserializedItem = serializer.Deserialize(byteCount, reader);

            item.Should().BeEquivalentTo(deserializedItem);
        }

        [Test]
        public void NullObjectSerializeDeserialize()
        {
            var item = new NullTestObject
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

        [Test]
        public void CalculateSizeOfObject()
        {
            var item = _fixture.Create<ReferenceTypeObject>();
            var serializer = GenericItemSerializer<ReferenceTypeObject>.Default;
            using var memoryStream = new MemoryStream();
            var writer = new EndianBinaryWriter(EndianBitConverter.Little, memoryStream);
            
            var calculatedSize = serializer.CalculateSize(item);
            Assert.AreEqual(0, memoryStream.Length);
            
            var serializedSize = serializer.Serialize(item, writer);
            Assert.AreEqual(serializedSize, calculatedSize);
            Assert.AreEqual(memoryStream.Length, calculatedSize);
        }

        [Test]
        public void CalculateTotalSize()
        {
            var items = _fixture.CreateMany<ReferenceTypeObject>();
            var serializer = GenericItemSerializer<ReferenceTypeObject>.Default;
            using var memoryStream = new MemoryStream();
            var writer = new EndianBinaryWriter(EndianBitConverter.Little, memoryStream);
            
            var calculatedTotalSize = serializer.CalculateTotalSize(items, true, out var sizes);
            Assert.AreEqual(0, memoryStream.Length);

            int serializedTotal = 0;
            foreach (var item in items)
            {
                serializedTotal += serializer.Serialize(item, writer);
            }
            
          
            Assert.AreEqual(serializedTotal, calculatedTotalSize);
            Assert.AreEqual(memoryStream.Length, calculatedTotalSize);
            Assert.AreEqual(items.Count(), sizes.Length);
            Assert.AreEqual(calculatedTotalSize, sizes.Sum(x => x));
        }

        [Test]
        public void EnumSerializeDeserialize()
        {
            var item = _fixture.Create<EnumObj>();
            var serializer = GenericItemSerializer<EnumObj>.Default;

            using var memoryStream = new MemoryStream();
            var writer = new EndianBinaryWriter(EndianBitConverter.Little, memoryStream);
            var size = serializer.CalculateSize(item);
            Assert.AreEqual(0, memoryStream.Length);
            var serializedSize = serializer.Serialize(item, writer);
            Assert.AreEqual(serializedSize, size);
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
        public ValueTypeTestObject V { get; set; }
        
        public EnumObj W { get; set; }
        
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
        public int X { get; set; }
    }

    
    internal class EnumObj
    {
        [Flags]
        internal enum TestEnum : byte
        {
            A = 1,
            B = 2,
            C = 3,
            D = 4
        }

        public TestEnum A { get; set; }
        
        public TestEnum B { get; set; }
    }
}