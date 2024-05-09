// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using NUnit.Framework;
using NUnit.Framework.Legacy;

namespace Hydrogen.Tests;

[TestFixture]
[Parallelizable(ParallelScope.Children)]
public class MemoryMetricTests {


	[Test]
	public void Byte2Bit() {
		ClassicAssert.AreEqual(16, Tools.Memory.ConvertMemoryMetric(2, MemoryMetric.Byte, MemoryMetric.Bit));
	}

	[Test]
	public void Bit2Byte() {
		ClassicAssert.AreEqual(2, Tools.Memory.ConvertMemoryMetric(16, MemoryMetric.Bit, MemoryMetric.Byte));
	}

	[Test]
	public void Kilobyte2Byte() {
		ClassicAssert.AreEqual(1000, Tools.Memory.ConvertMemoryMetric(1, MemoryMetric.Kilobyte, MemoryMetric.Byte));
	}

	[Test]
	public void Byte2Kilobyte() {
		ClassicAssert.AreEqual(1, Tools.Memory.ConvertMemoryMetric(1000, MemoryMetric.Byte, MemoryMetric.Kilobyte));
	}

	[Test]
	public void KilobyteToMegabyte() {
		ClassicAssert.AreEqual(1, Tools.Memory.ConvertMemoryMetric(1000, MemoryMetric.Kilobyte, MemoryMetric.Megabyte));
	}

	[Test]
	public void KilobyteToMegabit() {
		ClassicAssert.AreEqual(1 * 8, Tools.Memory.ConvertMemoryMetric(1000, MemoryMetric.Kilobyte, MemoryMetric.Megabit));
	}
}
