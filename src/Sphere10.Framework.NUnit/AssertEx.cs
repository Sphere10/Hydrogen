using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using NUnit.Framework;
using NUnit.Framework.Internal;
using Sphere10.Framework;
using Tools;

namespace Sphere10.Framework.NUnit {

	public static class AssertEx {

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