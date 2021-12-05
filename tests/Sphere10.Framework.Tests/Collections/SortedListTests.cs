using FluentAssertions;
using NUnit.Framework;

namespace Sphere10.Framework.Tests
{
    public class SortedListTests
    {
        [Test]
        [Ignore("Failing")]
        public void SortIntsDefaultComparer()
        {
            var sortedList = new SortedList<int>();
            sortedList.AddRangeSequentially(new [] {5, 4, 3, 2, 1});

            sortedList.Should().BeInAscendingOrder();
        }
    }
}