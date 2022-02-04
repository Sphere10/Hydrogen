using System.Linq;
using NUnit.Framework;
using Sphere10.Framework.Collections;
using Sphere10.Framework.NUnit;

namespace Sphere10.Framework.Tests {

	public class PreAllocatedListTests {
		[Test]
		public void AddRange() {
			var list = new PreAllocatedList<int>(10, () => default);
			Assert.IsTrue(list.All(x => x == default));

			list.AddRange(Enumerable.Range(0, 5));
			list.AddRange(Enumerable.Range(5, 5));

			Assert.AreEqual(Enumerable.Range(0, 10).ToList(), list);
		}

		[Test]
		public void Insert_Basic() {
			var store = new ExtendedList<int>(new[] { 0, 0, 0 });
			var list = new PreAllocatedList<int>(store, 0, PreAllocationPolicy.Fixed, 3, () => default);
			list.AddRange(new[] { 1, 3 });
			list.Insert(1, 2);
			Assert.AreEqual(new[] { 1, 2, 3 }, list);
		}

		[Test]
		public void InsertAtIndex() {
			int[] input = Enumerable.Repeat(0, 10).ToArray();
			ExtendedList<int> list = new ExtendedList<int>(input);
			PreAllocatedList<int> preallocatedList = new PreAllocatedList<int>(list, 0, PreAllocationPolicy.Fixed, 10, () => default);
			preallocatedList.InsertRange(0, input.Reverse());
			Assert.AreEqual(input.Reverse(), list);
		}

		[Test]
		public void RemoveAtIndex() {
			int[] expected = Enumerable.Repeat(0, 10).ToArray();

			ExtendedList<int> list = new ExtendedList<int>(expected);
			PreAllocatedList<int> preallocatedList = new PreAllocatedList<int>(list, 0, PreAllocationPolicy.Fixed, 10, () => default);

			preallocatedList.RemoveRange(0, preallocatedList.Count);

			Assert.IsTrue(preallocatedList.All(x => x == default));
		}

		[Test]
		public void IntegrationTests_Fixed([Values(1, 793, 2000)] int maxCapacity) {
			var fixedStore = new ExtendedList<int>(Tools.Array.Gen(maxCapacity, 0));
			var list = new PreAllocatedList<int>(fixedStore, 0, PreAllocationPolicy.Fixed, maxCapacity, () => default);
			AssertEx.ListIntegrationTest(list, maxCapacity, (rng, i) => rng.NextInts(i));
		}

		[Test]
		public void IntegrationTests_ByBlock([Values(0, 1, 793, 2000)] int maxCapacity) {
			var list = new PreAllocatedList<int>(PreAllocationPolicy.ByBlock, 5, () => default);
			AssertEx.ListIntegrationTest(list, maxCapacity, (rng, i) => rng.NextInts(i));
		}

		[Test]
		public void IntegrationTests_MinimumRequired([Values(0, 1, 793, 2000)] int maxCapacity) {
			var list = new PreAllocatedList<int>(PreAllocationPolicy.MinimumRequired, 0, () => default);
			AssertEx.ListIntegrationTest(list, maxCapacity, (rng, i) => rng.NextInts(i));
		}

	}

}