using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NUnit.Framework;
using Sphere10.Framework.Collections;

namespace Sphere10.Framework.Tests
{

    public class FixedClusterMappedListTests
    {
        [Test]
        public void AddAndGet()
        {
            var stream = new MemoryStream();
            var list = new ClusteredStreamMappedList<int>(32, 100, 100, new IntSerializer(), stream);

            list.Add(999);

            Assert.AreEqual(999, list[0]);
        }

        [Test]
        [Ignore("foobaz")]
        public void RestoreAndGet()
        {
            var stream = new MemoryStream();
            var list = new ClusteredStreamMappedList<int>(32, 100, 100, new IntSerializer(), stream);

            list.Add(999);

            var secondlist = new ClusteredStreamMappedList<int>(32, 100, 100, new IntSerializer(), stream);

            Assert.AreEqual(999, secondlist[0]);
        }

        [Test]
        public void Update()
        {
            var stream = new MemoryStream();
            var list = new ClusteredStreamMappedList<int>(32, 100, 100, new IntSerializer(), stream);

            list.Add(999);
            list.Add(1000);
            list.Add(1001);
            list.Add(1002);
            list.Update(0, 998);
            int read = list[0];

            Assert.AreEqual(998, read);
            Assert.AreEqual(4, list.Count);
        }

        [Test]
        public void Remove()
        {
            var stream = new MemoryStream();
            var list = new ClusteredStreamMappedList<int>(32, 100, 100, new IntSerializer(), stream);

            list.Add(999);
            list.Add(1000);
            list.RemoveAt(0);

            Assert.AreEqual(1000, list[0]);
        }

        [Test]
        public void IndexOf()
        {
            var stream = new MemoryStream();
            var list = new ClusteredStreamMappedList<int>(32, 100, 100, new IntSerializer(), stream);

            list.Add(998);
            list.Add(999);
            list.Add(1000);
            list.Add(1001);

            IEnumerable<int> indexes = list.IndexOfRange(new[] {999, 1000});

            Assert.AreEqual(new[] {1, 2}, indexes);
        }

        [Test]
        public void Count()
        {
            var stream = new MemoryStream();
            var list = new ClusteredStreamMappedList<int>(32, 100, 100, new IntSerializer(), stream);

            list.Add(998);
            list.Add(999);
            list.Add(1000);
            list.Add(1001);

            Assert.AreEqual(4, list.Count);
        }

        [Test]
        [Pairwise]
        public void IntegrationTests()
        {
            var serializer = new IntSerializer();
            var RNG = new Random(1231);
            var list = new ClusteredStreamMappedList<int>(16, 100, 10000, serializer, new MemoryStream());
            var expected = new List<int>();

            int capacity = list.Capacity;
            
            for (var i = 0; i < 25; i++)
            {
                // add a random amount
                var remainingCapacity = capacity - list.Count;
                var newItemsCount = RNG.Next(1, remainingCapacity);
                IEnumerable<int> newItems = RNG.NextInts(newItemsCount);
                list.AddRange(newItems);
                expected.AddRange(newItems);
                Assert.AreEqual(expected, list.ToList());

                if (list.Count > 0)
                {
                    // update a random amount
                    var range = RNG.NextRange(list.Count);
                    newItems = RNG.NextInts(range.End - range.Start + 1);
                    list.UpdateRange(range.Start, newItems);
                    expected.UpdateRangeSequentially(range.Start, newItems);
                    Assert.AreEqual(expected, list.ToList());

                    // shuffle a random amount
                    range = RNG.NextRange(list.Count);
                    newItems = list.ReadRange(range.Start, range.End - range.Start + 1);
                    var expectedNewItems = expected.GetRange(range.Start, range.End - range.Start + 1);
                    range = RNG.NextRange(list.Count, rangeLength: newItems.Count());
                    expected.UpdateRangeSequentially(range.Start, expectedNewItems);
                    list.UpdateRange(range.Start, newItems);

                    Assert.AreEqual(expected.Count, list.Count);
                    Assert.AreEqual(expected, list.ToList());

                    // remove a random amount
                    range = RNG.NextRange(list.Count);
                    list.RemoveRange(range.Start, range.End - range.Start + 1);
                    expected.RemoveRange(range.Start, range.End - range.Start + 1);
                    Assert.AreEqual(expected, list.ToList());
                }

                // insert a random amount
                remainingCapacity = capacity - list.Count;
                newItemsCount = RNG.Next(0, remainingCapacity);
                newItems = RNG.NextInts(newItemsCount);
                var insertIX = RNG.Next(0, list.Count);
                list.InsertRange(insertIX, newItems);
                expected.InsertRange(insertIX, newItems);
                Assert.AreEqual(expected, list.ToList());
            }
        }
    }

    internal class IntSerializer : FixedSizeObjectSerializer<int> {
        public IntSerializer() : base(4) {
        }

        public override int Serialize(int @object, EndianBinaryWriter writer) {
            writer.Write(BitConverter.GetBytes(@object));
            return sizeof(int);
        }

        public override int Deserialize(int size, EndianBinaryReader reader) {
            return reader.ReadInt32();
        }

    }

}