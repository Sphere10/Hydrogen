// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using NUnit.Framework;
using NUnit.Framework.Legacy;

namespace Hydrogen.Tests;

public class PagedBufferImplementationHelperTests {

	public void ReadSpan([Values(0, 10, 99, 1)] int startIndex, [Values(1000, 990, 1, 9)] int count, [Values(1, 2, 100)] int maxOpenPages) {
		var rand = new Random(31337);
		byte[] input = rand.NextBytes(1000);

		using IMemoryPagedBuffer buffer = new MemoryPagedBuffer(10, 10 * maxOpenPages);
		buffer.AddRange(input);

		int endIndex = startIndex + count;

		var span = buffer.ReadSpan(startIndex, count);
		ClassicAssert.AreEqual(input[startIndex..endIndex], span.ToArray());
	}
}
