// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using NUnit.Framework;

namespace Hydrogen.Tests;

[TestFixture]
[Parallelizable(ParallelScope.Children)]
public class MemoryMetricTests {


	[Test]
	public void Byte2Bit() {
		Assert.AreEqual(16, Tools.Memory.ConvertMemoryMetric(2, MemoryMetric.Byte, MemoryMetric.Bit));
	}

	[Test]
	public void Bit2Byte() {
		Assert.AreEqual(2, Tools.Memory.ConvertMemoryMetric(16, MemoryMetric.Bit, MemoryMetric.Byte));
	}

	[Test]
	public void Kilobyte2Byte() {
		Assert.AreEqual(1000, Tools.Memory.ConvertMemoryMetric(1, MemoryMetric.Kilobyte, MemoryMetric.Byte));
	}

	[Test]
	public void Byte2Kilobyte() {
		Assert.AreEqual(1, Tools.Memory.ConvertMemoryMetric(1000, MemoryMetric.Byte, MemoryMetric.Kilobyte));
	}

	[Test]
	public void KilobyteToMegabyte() {
		Assert.AreEqual(1, Tools.Memory.ConvertMemoryMetric(1000, MemoryMetric.Kilobyte, MemoryMetric.Megabyte));
	}

	[Test]
	public void KilobyteToMegabit() {
		Assert.AreEqual(1 * 8, Tools.Memory.ConvertMemoryMetric(1000, MemoryMetric.Kilobyte, MemoryMetric.Megabit));
	}
}
