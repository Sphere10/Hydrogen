using FluentAssertions;
using NUnit.Framework;

namespace Sphere10.Framework.Tests {

	[Parallelizable]
	public class SortedListTests {
		[Test]
		public void SortInts_Ascending_Default() {
			SortInts_Ascending();
		}

		[Test]
		public void SortInts_Ascending() {
			var sortedList = new SortedList<int>(SortDirection.Ascending);
			sortedList.AddRangeSequentially(new[] { 4, 5, 1, 3, 1, 2, 5 });
			sortedList.Should().BeEquivalentTo(new[] { 1, 1, 2, 3, 4, 5, 5  });
		}


		[Test]
		public void SortInts_Descending() {
			var sortedList = new SortedList<int>(SortDirection.Descending);
			sortedList.AddRangeSequentially(new[] { 4, 5, 1, 3, 1, 2, 5 });
			sortedList.Should().BeEquivalentTo(new[] { 5, 5, 4, 3, 2, 1, 1 });
		}
	}
}