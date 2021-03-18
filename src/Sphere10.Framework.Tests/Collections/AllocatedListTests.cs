using System.Linq;
using NUnit.Framework;
using Sphere10.Framework.Collections;
using Sphere10.Framework.NUnit;

namespace Sphere10.Framework.Tests
{

    public class AllocatedListTests {
        [Test]
        public void AddRange() {
            int[] start = Enumerable.Repeat(0, 10).ToArray();

            ExtendedList<int> list = new ExtendedList<int>(start);
            AllocatedList<int> preallocatedList = new AllocatedList<int>(list);
            Assert.IsTrue(preallocatedList.All(x => x == default));

            preallocatedList.AddRange(Enumerable.Range(0, 5));
            preallocatedList.AddRange(Enumerable.Range(5, 5));

            Assert.AreEqual(Enumerable.Range(0, 10).ToList(), preallocatedList);
        }


        [Test]
        public void Insert_Basic() {
            var store = new ExtendedList<int>(new[] { 0, 0, 0 });
            var list = new AllocatedList<int>(store);
            list.AddRange(new[] { 1, 3 });
            list.Insert(1, 2);
            Assert.AreEqual(new[] { 1, 2, 3 }, list);
        }


        [Test]
        public void InsertAtIndex() {
            int[] input = Enumerable.Repeat(0, 10).ToArray();
            ExtendedList<int> list = new ExtendedList<int>(input);
			AllocatedList<int> preallocatedList = new AllocatedList<int>(list);
            preallocatedList.InsertRange(0, input.Reverse());
            Assert.AreEqual(input.Reverse(), list);
        }

        [Test]
        public void RemoveAtIndex() {
            int[] expected = Enumerable.Repeat(0, 10).ToArray();

            ExtendedList<int> list = new ExtendedList<int>(expected);
			AllocatedList<int> preallocatedList = new AllocatedList<int>(list);

            preallocatedList.RemoveRange(0, preallocatedList.Count);

            Assert.IsTrue(preallocatedList.All(x => x == default));
        }

        [Test]
        public void Enumerate()
        {
            ExtendedList<int> list = new ExtendedList<int>(Enumerable.Repeat(9, 100).ToArray());
            AllocatedList<int> allocatedList = new AllocatedList<int>(list);
            Assert.AreEqual(allocatedList.ToList(), list);
            
        }


        [Test]
        [Pairwise]
        public void IntegrationTests([Values(0, 1, 793, 2000)] int maxCapacity) {
            var fixedStore = new ExtendedList<int>(Tools.Array.Gen(maxCapacity, 0));
            var list = new AllocatedList<int>(fixedStore);
            AssertEx.ListIntegrationTest(list, maxCapacity, (rng, i) => rng.NextInts(i));
        }
    }

}