using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using NUnit.Framework;

namespace Sphere10.Framework.NUnit {

	public static class AssertEx {

		public static void ListIntegrationTest<T>(IExtendedList<T> list, int maxCapacity, Func<Random, int, T[]> randomItemGenerator, bool mutateFromEndOnly = false, int iterations = 100, IList<T> expected = null, IEqualityComparer<T> itemComparer = null) {
			var RNG = new Random(31337);
			expected ??= new List<T>();
			itemComparer ??= EqualityComparer<T>.Default;

			// Test 1: Add nothing
			expected.AddRangeSequentially(Enumerable.Empty<T>());
			list.AddRange(Enumerable.Empty<T>());
			Assert.That(expected, Is.EqualTo(list).Using(itemComparer));

			// Test 2: Insert nothing
			expected.InsertRangeSequentially(0, Enumerable.Empty<T>());
			list.InsertRange(0, Enumerable.Empty<T>());
			Assert.That(expected, Is.EqualTo(list).Using(itemComparer));

			if (maxCapacity >= 1) {
				// Test 3: Insert at 0 when empty 
				var item = randomItemGenerator(RNG, 1).Single();
				expected.Insert(0, item);
				list.Insert(0, item);
				Assert.That(expected, Is.EqualTo(list).Using(itemComparer));
			}

			if (maxCapacity >= 2) {
				// Test 4: Insert at end of list (same as add)
				var item = randomItemGenerator(RNG, 1).Single();
				expected.Insert(1, item);
				list.Insert(1, item);
				Assert.That(expected, Is.EqualTo(list).Using(itemComparer));

				// Test 5: Delete from beginning of list
				if (!mutateFromEndOnly) {
					expected.RemoveAt(0);
					list.RemoveAt(0);
					Assert.That(expected, Is.EqualTo(list).Using(itemComparer));
				}
			}

			if (maxCapacity >= 1) {
				// Test 6: Delete from end of list
				expected.RemoveAt(^1);
				list.RemoveAt(^1);
				Assert.That(expected, Is.EqualTo(list).Using(itemComparer));
			}

			// Test 7: AddRange half capacity
			T[] newItems = randomItemGenerator(RNG, maxCapacity / 2);
			expected.AddRangeSequentially(newItems);
			list.AddRange(newItems);
			Assert.That(expected, Is.EqualTo(list).Using(itemComparer));

			// Test 8: Enumerator consistency
			using (var expectedEnumerator = expected.GetEnumerator())
			using (var enumerator = list.GetEnumerator()) {
				
				Assert.That(expectedEnumerator.Current, Is.EqualTo(enumerator.Current).Using(itemComparer));
				bool expectedMoveNext;
				do {
					expectedMoveNext = expectedEnumerator.MoveNext();
					var moveNext = enumerator.MoveNext();
					Assert.That(expectedMoveNext, Is.EqualTo(moveNext).Using(itemComparer));
					if (expectedMoveNext)
						Assert.That(expectedEnumerator.Current, Is.EqualTo(enumerator.Current).Using(itemComparer));
				} while (expectedMoveNext);
			}

			// Test 9: Clear
			for (var i = 0; i < 3; i++) {
				Assert.That(expected, Is.EqualTo(list).Using(itemComparer));
				// add a random amount
				var remainingCapacity = maxCapacity - list.Count;
				var newItemsCount = RNG.Next(0, remainingCapacity + 1);
				newItems = randomItemGenerator(RNG, newItemsCount);
				expected.AddRangeSequentially(newItems);
				list.AddRange(newItems);

				expected.Clear();
				list.Clear();
			}



			// Test 10: Iterate with random mutations
			for (var i = 0; i < iterations; i++) {

				// add a random amount
				var remainingCapacity = maxCapacity - list.Count;
				var newItemsCount = RNG.Next(0, remainingCapacity + 1);
				newItems = randomItemGenerator(RNG, newItemsCount);
				list.AddRange(newItems);
				expected.AddRangeSequentially(newItems);
				Assert.That(expected, Is.EqualTo(list).Using(itemComparer));

				if (list.Count > 0) {
					// update a random range
					var range = RNG.NextRange(list.Count);
					var rangeLen = Math.Max(0, range.End - range.Start + 1);
					newItems = randomItemGenerator(RNG, rangeLen);
					list.UpdateRange(range.Start, newItems);
					expected.UpdateRangeSequentially(range.Start, newItems);
					Assert.That(expected, Is.EqualTo(list).Using(itemComparer));

					// copy/paste a random range
					range = RNG.NextRange(list.Count);
					rangeLen = Math.Max(0, range.End - range.Start + 1);
					newItems = list.ReadRange(range.Start, rangeLen).ToArray();
					var expectedNewItems = expected.ReadRangeSequentially(range.Start, rangeLen).ToArray();

					range = RNG.NextRange(list.Count, rangeLength: newItems.Length);
					expected.UpdateRangeSequentially(range.Start, expectedNewItems);
					list.UpdateRange(range.Start, newItems);
					Assert.That(expected, Is.EqualTo(list).Using(itemComparer));

					// remove a random amount
					range = RNG.NextRange(list.Count, fromEndOnly: mutateFromEndOnly);
					rangeLen = Math.Max(0, range.End - range.Start + 1);
					list.RemoveRange(range.Start, rangeLen);
					expected.RemoveRangeSequentially(range.Start, rangeLen);
					Assert.That(expected, Is.EqualTo(list).Using(itemComparer));
				}

				// insert a random amount
				remainingCapacity = maxCapacity - list.Count;
				newItemsCount = RNG.Next(0, remainingCapacity + 1);
				newItems = randomItemGenerator(RNG, newItemsCount);
				var insertIX = mutateFromEndOnly ? list.Count : RNG.Next(0, list.Count + 1);  // + 1 allows inserting from end (same as add)
				list.InsertRange(insertIX, newItems);
				expected.InsertRangeSequentially(insertIX, newItems);
				Assert.That(expected, Is.EqualTo(list).Using(itemComparer));

			}
		}
		
		/// <summary>
		/// Same as <see cref="ListIntegrationTest{T}(IExtendedList{T}, int, Func{Random, int, T[]}, bool, int, IList{T})"/> but using span mutator methods
		/// </summary>
		/// <param name="buffer">The buffer being tested</param>
		/// <param name="maxCapacity">Maximum bytes buffer can store</param>
		/// <param name="mutateFromEndOnly">Mutate only from end of buffer, not middle</param>
		/// <param name="iterations">Number of internal iterations where random operations are performed </param>
		/// <param name="expected">Buffer implementation which <see cref="buffer"/> is tested against. This is assumed to be bug-free.</param>
		public static void BufferIntegrationTest(IBuffer buffer, int maxCapacity, bool mutateFromEndOnly = false, int iterations = 100, IBuffer expected = null) {
			var RNG = new Random(31337);
			expected ??= new BufferAdapter(new List<byte>());
			for (var i = 0; i < iterations; i++) {

				// add a random amount
				var remainingCapacity = maxCapacity - buffer.Count;
				var newItemsCount = RNG.Next(0, remainingCapacity + 1);
				ReadOnlySpan<byte> newItems = RNG.NextBytes(newItemsCount);
				buffer.AddRange(newItems);
				expected.AddRange(newItems);
				Assert.AreEqual(expected, buffer);

				if (buffer.Count > 0) {
					// update a random range
					var range = RNG.NextRange(buffer.Count);
					var rangeLen = Math.Max(0, range.End - range.Start + 1);
					newItems = RNG.NextBytes(rangeLen);
					buffer.UpdateRange(range.Start, newItems);
					expected.UpdateRange(range.Start, newItems);
					Assert.AreEqual(expected, buffer);

					// copy/paste a random range
					range = RNG.NextRange(buffer.Count);
					rangeLen = Math.Max(0, range.End - range.Start + 1);
					newItems = buffer.ReadSpan(range.Start, rangeLen);
					ReadOnlySpan<byte> expectedNewItems = expected.ReadSpan(range.Start, rangeLen);

					range = RNG.NextRange(buffer.Count, rangeLength: newItems.Length);
					expected.UpdateRange(range.Start, expectedNewItems);
					buffer.UpdateRange(range.Start, newItems);
					Assert.AreEqual(expected, buffer);

					// remove a random amount
					range = RNG.NextRange(buffer.Count, fromEndOnly: mutateFromEndOnly);
					rangeLen = Math.Max(0, range.End - range.Start + 1);
					buffer.RemoveRange(range.Start, rangeLen);
					expected.RemoveRangeSequentially(range.Start, rangeLen);
					Assert.AreEqual(expected, buffer);
				}

				// insert a random amount
				remainingCapacity = maxCapacity - buffer.Count;
				newItemsCount = RNG.Next(0, remainingCapacity + 1);
				newItems = RNG.NextBytes(newItemsCount);
				var insertIX = mutateFromEndOnly ? buffer.Count : RNG.Next(0, buffer.Count);
				buffer.InsertRange(insertIX, newItems);
				expected.InsertRange(insertIX, newItems);
				Assert.AreEqual(expected, buffer);
			}
		}

		public static void AssertTreeEqual(IUpdateableMerkleTree expected, IUpdateableMerkleTree actual) {
			Assert.AreEqual(expected.Size, actual.Size);
			Assert.AreEqual(expected.Leafs.Count, actual.Leafs.Count);
			Assert.AreEqual(expected.Leafs.ToArray(), actual.Leafs.ToArray());
			Assert.AreEqual(expected.Root, actual.Root);
		}

		public static void NotHasFlags<T>(T expected, T actual) where T : Enum {
			Assert.IsFalse(actual.HasFlag(expected));
		}

		public static void HasFlags<T>(T expected, T actual) where T : Enum {
			Assert.IsTrue(actual.HasFlag(expected));
		}

		public static void HasAllFlags<T>(T actual, params T[] flags) where T : Enum {
			foreach(var flag in flags)
				HasFlags(flag, actual);
		}

		public static void RegexMatch(string expected, string regexPattern, params Tuple<string, string>[] expectedCaptures) {
			var regex = new Regex(regexPattern);
			var match = regex.Match(expected);
			Assert.AreEqual(expected, match.Value);
			foreach (var expectedCapture in expectedCaptures) {
				if (expectedCapture.Item2 == null)
					Assert.IsFalse(match.Groups[expectedCapture.Item1].Success);
				else
					Assert.AreEqual(expectedCapture.Item2, match.Groups[expectedCapture.Item1]?.Value);
			}
		}

		public static void RegexNotMatch(string badInput, string regexPattern, params Tuple<string, string>[] expectedCaptures) {
			var regex = new Regex(regexPattern);
			var match = regex.Match(badInput);
			Assert.AreNotEqual(badInput, match.Value);
			if (expectedCaptures.Any())
				Assert.IsFalse(expectedCaptures.All(c => c.Item2 == match.Groups[c.Item1]?.Value));

		}


		public static void AssertSame2DArrays<T>(IEnumerable<IEnumerable<T>> expectedRows, IEnumerable<IEnumerable<T>> actualRows, string actualName = "Actual", string expectedName = "Expected") {
			var expectedRowsArr = expectedRows as T[][] ?? expectedRows.Select(i => i as T[] ?? i.ToArray()).ToArray();
			var actualRowsArr = actualRows as T[][] ?? actualRows.Select(i => i as T[] ?? i.ToArray()).ToArray();

			var preText = String.Format("{0}{1}{2}{0}", Environment.NewLine, Tools.NUnit.Convert2DArrayToString(expectedName, expectedRowsArr), Tools.NUnit.Convert2DArrayToString(actualName, actualRowsArr));

			Assert.AreEqual(expectedRowsArr.Count(), actualRowsArr.Count(), "{4}{0} has {1} row(s) but {2} has {3} row(s)", actualName, expectedRowsArr.Count(), expectedName, actualRowsArr.Count(), preText);
			foreach (var rowExpectation in expectedRowsArr.WithDescriptions().Zip(actualRowsArr, Tuple.Create)) {
				Assert.AreEqual(rowExpectation.Item1.Item.Count(), rowExpectation.Item2.Count(), "{5}{0} row {1} had {2} column(s) but {3} row {1} had {4} column(s)", expectedName, rowExpectation.Item1.Index, rowExpectation.Item1.Item.Count(), actualName, rowExpectation.Item2.Count(), preText);
				foreach (var colExpectation in rowExpectation.Item1.Item.WithDescriptions().Zip(rowExpectation.Item2, Tuple.Create)) {
					Assert.AreEqual(colExpectation.Item1.Item, colExpectation.Item2, "{6}{0} row {1} col {2} had value {3} but {4} row {1} col {2} had value {5}", expectedName, rowExpectation.Item1.Index, colExpectation.Item1.Index, colExpectation.Item1.Item, actualName, colExpectation.Item2, preText);
				}
			}
		}

		public static void ApproxEqual(System.DateTime expected, System.DateTime actual, TimeSpan? tolerance = null, string errorMessage = null) {
			var approxEqual = expected.ApproxEqual(actual, tolerance);
			if (!approxEqual)
				Assert.Fail(errorMessage ?? "Dates not approximately equal.{0}Expected: {1:yyyy-MM-dd HH:mm:ss.fff}{0}Actual: {2:yyyy-MM-dd HH:mm:ss.fff}", Environment.NewLine, expected, actual);
		}

		public static void ApproxEqual<T>(T expected, T actual, T tolerance, string message = null) {
			var lowerBound = Tools.OperatorTool.Subtract(expected, tolerance);
			var upperBound = Tools.OperatorTool.Add(expected, tolerance);
			var inRange = Tools.OperatorTool.LessThanOrEqual(lowerBound, actual) && Tools.OperatorTool.LessThanOrEqual(actual, upperBound);
			Assert.IsTrue(inRange, message ?? $"Value '{actual}' was not approx equal to '{expected}' (tolerance '{tolerance}')");
		}


		public static void HasLoadedPages<TItem>(PagedListBase<TItem> list, params int[] pageNos) {
			Assert.IsEmpty(list.Pages.Where(p => p.State == PageState.Loaded).Select(p => p.Number).Except(pageNos), "Unexpected pages were open");
		}

		public static void HasDirtyPages<TItem, TPage>(PagedListBase<TItem> list, params int[] pageNos) {
			Assert.IsEmpty(list.Pages.Where(p => p.Dirty).Select(p => p.Number).Except(pageNos), "Unexpected pages were dirty");
		}

	}
}