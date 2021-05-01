using System;
using NUnit.Framework;

namespace Sphere10.Framework.Tests
{

    public class PagedBufferImplementationHelperTests
    {
        [TestCase(0, 1000)]
        [TestCase(10, 990)]
        [TestCase(999, 1)]
        [TestCase(1, 9)]
        public void ReadSpan(int startIndex, int count)
        {
            var rand = new Random(31337);
            byte[] input = rand.NextBytes(1000);
            
            using IMemoryPagedBuffer buffer = new MemoryPagedBuffer(10, 100);
            buffer.AddRange(input);

            int endIndex = startIndex + count;
            
            var span = buffer.ReadSpan(startIndex, count);
            Assert.AreEqual(input[startIndex..endIndex], span.ToArray());
        }
    }
}