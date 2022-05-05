using System;
using NUnit.Framework;

namespace Hydrogen.Tests {

	public class PagedBufferImplementationHelperTests {

		public void ReadSpan([Values(0, 10, 99, 1)] int startIndex, [Values(1000, 990, 1, 9)] int count, [Values(1, 2, 100)] int maxOpenPages) {
			var rand = new Random(31337);
			byte[] input = rand.NextBytes(1000);

			using IMemoryPagedBuffer buffer = new MemoryPagedBuffer(10, 10 * maxOpenPages);
			buffer.AddRange(input);

			int endIndex = startIndex + count;

			var span = buffer.ReadSpan(startIndex, count);
			Assert.AreEqual(input[startIndex..endIndex], span.ToArray());
		}
	}

}

