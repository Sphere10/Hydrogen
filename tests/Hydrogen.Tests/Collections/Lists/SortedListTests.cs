// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using FluentAssertions;
using NUnit.Framework;

namespace Hydrogen.Tests;

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
		sortedList.Should().BeEquivalentTo(new[] { 1, 1, 2, 3, 4, 5, 5 });
	}


	[Test]
	public void SortInts_Descending() {
		var sortedList = new SortedList<int>(SortDirection.Descending);
		sortedList.AddRangeSequentially(new[] { 4, 5, 1, 3, 1, 2, 5 });
		sortedList.Should().BeEquivalentTo(new[] { 5, 5, 4, 3, 2, 1, 1 });
	}
}
