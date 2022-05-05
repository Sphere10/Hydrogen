﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using NUnit.Framework;
using Hydrogen.Collections;
using Hydrogen.Misc;
using Hydrogen.NUnit;

namespace Hydrogen.Tests {

	public class ColumnarListTests {

		private class TestItem {
			private int _field;

			public string Property { get; set; }

			public static TestItem GenRandom(Random rng) {
				var item = new TestItem();
				item._field = rng.Next();
				item.Property = rng.NextString(rng.Next(0, 10));
				return item;
			}

			public object[] ToObjectArray() => new object[] { _field, Property };
		}


		private static object[][] Gen(Random rng, int quantity)
			=> Enumerable.Range(0, quantity).Select(x => TestItem.GenRandom(rng).ToObjectArray()).ToArray();

		[Test]
		[Pairwise]
		public void IntegrationTests([Values(0, 1, 793, 2000)] int maxCapacity) {
			var columnarList = new ColumnarList(new ExtendedList<object>(), new ExtendedList<object>());
			AssertEx.ListIntegrationTest<object[]>(columnarList, maxCapacity, Gen, itemComparer: new ArrayEqualityComparer<object>());
		}
	}

}