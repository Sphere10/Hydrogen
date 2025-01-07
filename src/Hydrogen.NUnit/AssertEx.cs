// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using NUnit.Framework;
using NUnit.Framework.Legacy;

namespace Hydrogen.NUnit;

public static class AssertEx {


	public static void ListIntegrationTest<T>(
		IExtendedList<T> list,
		int maxCapacity,
		Func<Random, int, T[]> randomItemGenerator,
		bool mutateFromEndOnly = false,
		int iterations = 100,
		IList<T> expected = null,
		Action endOfIterTest = null,
		IEqualityComparer<T> itemComparer = null
	) {
		var RNG = new Random(31337);
		expected ??= new List<T>();
		itemComparer ??= EqualityComparer<T>.Default;

		// Test 0: Adding via indexing operator fails
		var val = randomItemGenerator(RNG, 1).Single();
		Assert.That(() => expected[0] = val, Throws.Exception);
		Assert.That(() => list[0] = val, Throws.Exception);

		// Test 1: Add nothing
		expected.AddRangeSequentially(Enumerable.Empty<T>());
		list.AddRange(Enumerable.Empty<T>());
		Assert.That(expected, Is.EqualTo(list).Using(itemComparer));
		Assert.That(expected.Count, Is.EqualTo(list.Count));

		// Test 2: Insert nothing
		expected.InsertRangeSequentially(0, Enumerable.Empty<T>());
		list.InsertRange(0, Enumerable.Empty<T>());
		Assert.That(expected, Is.EqualTo(list).Using(itemComparer));
		Assert.That(expected.Count, Is.EqualTo(list.Count));

		if (maxCapacity >= 1) {
			// Test 3: Insert at 0 when empty 
			var item = randomItemGenerator(RNG, 1).Single();
			expected.Insert(0, item);
			list.Insert(0, item);
			Assert.That(expected, Is.EqualTo(list).Using(itemComparer));
			Assert.That(expected.Count, Is.EqualTo(list.Count));
		}

		if (maxCapacity >= 2) {
			// Test 4: Insert at end of list (same as add)
			var item = randomItemGenerator(RNG, 1).Single();
			expected.Insert(1, item);
			list.Insert(1, item);
			Assert.That(expected, Is.EqualTo(list).Using(itemComparer));
			Assert.That(expected.Count, Is.EqualTo(list.Count));

			// Test 5: Delete from beginning of list
			if (!mutateFromEndOnly) {
				expected.RemoveAt(0);
				list.RemoveAt(0);
				Assert.That(expected, Is.EqualTo(list).Using(itemComparer));
				Assert.That(expected.Count, Is.EqualTo(list.Count));
			}
		}

		if (maxCapacity >= 1) {
			// Test 6: Delete from end of list
			expected.RemoveAt(^1);
			list.RemoveAt(^1);
			Assert.That(expected, Is.EqualTo(list).Using(itemComparer));
			Assert.That(expected.Count, Is.EqualTo(list.Count));
		}

		// Test 7: AddRange half capacity
		T[] newItems = randomItemGenerator(RNG, maxCapacity / 2);
		expected.AddRangeSequentially(newItems);
		list.AddRange(newItems);
		Assert.That(expected, Is.EqualTo(list).Using(itemComparer));
		Assert.That(expected.Count, Is.EqualTo(list.Count));

		// Test 8: Enumerator consistency
		using (var expectedEnumerator = expected.GetEnumerator())
		using (var enumerator = list.GetEnumerator()) {
			// .NET 8: .net 8 has a bug/change where enumerators throw on Current when empty
			//Assert.That(expectedEnumerator.Current, Is.EqualTo(enumerator.Current).Using(itemComparer));
			bool expectedMoveNext;
			do {
				expectedMoveNext = expectedEnumerator.MoveNext();
				var moveNext = enumerator.MoveNext();
				Assert.That(expectedMoveNext, Is.EqualTo(moveNext).Using(itemComparer));
				if (expectedMoveNext)
					Assert.That(expectedEnumerator.Current, Is.EqualTo(enumerator.Current).Using(itemComparer));
			} while (expectedMoveNext);
		}
		Assert.That(expected.Count, Is.EqualTo(list.Count));

		// Test 9: Read/Remove/Update nothing from tip
		// First fill up
		newItems = randomItemGenerator(RNG, maxCapacity - expected.Count);
		expected.AddRangeSequentially(newItems);
		list.AddRange(newItems);
		// Read nothing from tip
		var expectedRead = expected.ReadRangeSequentially(expected.Count, 0);
		var actualRead = list.ReadRange(list.Count, 0);
		Assert.That(expectedRead, Is.EqualTo(actualRead).Using(itemComparer));
		Assert.That(expected.Count, Is.EqualTo(list.Count));

		// Remove nothing from tip
		expected.RemoveRangeSequentially(expected.Count, 0);
		list.RemoveRange(list.Count, 0);
		Assert.That(expected, Is.EqualTo(list).Using(itemComparer));
		Assert.That(expected.Count, Is.EqualTo(list.Count));

		// Update nothing from tip
		expected.UpdateRangeSequentially(expected.Count, Enumerable.Empty<T>());
		list.UpdateRange(list.Count, Enumerable.Empty<T>());
		Assert.That(expected, Is.EqualTo(list).Using(itemComparer));
		Assert.That(expected.Count, Is.EqualTo(list.Count));

		// Test 10: Read/Remove/Update range overflow throws
		Assert.That(() => expected.ReadRangeSequentially(expected.Count / 2, maxCapacity + 1).ToArray(), Throws.InstanceOf<ArgumentException>());
		Assert.That(() => list.ReadRange(list.Count / 2, maxCapacity + 1).ToArray(), Throws.InstanceOf<ArgumentException>());
		Assert.That(() => expected.RemoveRangeSequentially(expected.Count / 2, maxCapacity + 1), Throws.InstanceOf<ArgumentException>());
		Assert.That(() => list.RemoveRange(list.Count / 2, maxCapacity + 1), Throws.InstanceOf<ArgumentException>());
		var updateItems = Tools.Array.Gen(maxCapacity + 1, randomItemGenerator(RNG, 1)[0]);
		Assert.That(() => expected.UpdateRangeSequentially(expected.Count / 2, updateItems), Throws.InstanceOf<ArgumentException>());
		Assert.That(() => list.UpdateRange(list.Count / 2, updateItems), Throws.InstanceOf<ArgumentException>());

		// Clear items (note: inconsistency could arise from test 10 due to do Singular-lists detecting argument error after mutations whereas range-lists not)
		expected.Clear();
		list.Clear();

		// Test 11: Clear
		for (var i = 0; i < 3; i++) {
			Assert.That(expected, Is.EqualTo(list).Using(itemComparer));
			// add a random amount
			var remainingCapacity = maxCapacity - expected.Count;
			var newItemsCount = RNG.Next(0, remainingCapacity + 1);
			newItems = randomItemGenerator(RNG, newItemsCount);
			expected.AddRangeSequentially(newItems);
			list.AddRange(newItems);

			expected.Clear();
			list.Clear();
		}

		// Test 12: Iterate with random mutations
		for (var i = 0; i < iterations; i++) {

			// add a random amount
			var remainingCapacity = maxCapacity - expected.Count;
			var newItemsCount = RNG.Next(0, remainingCapacity + 1);
			newItems = randomItemGenerator(RNG, newItemsCount);
			list.AddRange(newItems);
			expected.AddRangeSequentially(newItems);
			Assert.That(expected, Is.EqualTo(list).Using(itemComparer));
			Assert.That(expected.Count, Is.EqualTo(list.Count));

			if (expected.Count > 0) {
				// update a random range
				var range = RNG.NextRange(expected.Count);
				var rangeLen = Math.Max(0, range.End - range.Start + 1);
				newItems = randomItemGenerator(RNG, rangeLen);
				list.UpdateRange(range.Start, newItems);
				expected.UpdateRangeSequentially(range.Start, newItems);
				Assert.That(expected, Is.EqualTo(list).Using(itemComparer));
				Assert.That(expected.Count, Is.EqualTo(list.Count));

				// copy/paste a random range
				range = RNG.NextRange(expected.Count);
				rangeLen = Math.Max(0, range.End - range.Start + 1);
				newItems = list.ReadRange(range.Start, rangeLen).ToArray();
				var expectedNewItems = expected.ReadRangeSequentially(range.Start, rangeLen).ToArray();

				range = RNG.NextRange(expected.Count, rangeLength: newItems.Length);
				expected.UpdateRangeSequentially(range.Start, expectedNewItems);
				list.UpdateRange(range.Start, newItems);
				Assert.That(expected, Is.EqualTo(list).Using(itemComparer));
				Assert.That(expected.Count, Is.EqualTo(list.Count));

				// remove a random amount
				range = RNG.NextRange(expected.Count, fromEndOnly: mutateFromEndOnly);
				rangeLen = Math.Max(0, range.End - range.Start + 1);
				list.RemoveRange(range.Start, rangeLen);
				expected.RemoveRangeSequentially(range.Start, rangeLen);
				Assert.That(expected, Is.EqualTo(list).Using(itemComparer));
				Assert.That(expected.Count, Is.EqualTo(list.Count));

				// PagedList specific: check page index consistency
				if (list is IPagedList<T> pagedList) {
					for (var j = 1; j < pagedList.Pages.Count; j++)
						Assert.That(pagedList.Pages[j].StartIndex, Is.EqualTo(pagedList.Pages[j - 1].EndIndex + 1));
				}

				// Custom user test
				endOfIterTest?.Invoke();
			}

			// insert a random amount
			remainingCapacity = maxCapacity - expected.Count;
			newItemsCount = RNG.Next(0, remainingCapacity + 1);
			newItems = randomItemGenerator(RNG, newItemsCount);
			var insertIX = mutateFromEndOnly ? expected.Count : RNG.Next(0, expected.Count + 1); // + 1 allows inserting from end (same as add)
			list.InsertRange(insertIX, newItems);
			expected.InsertRangeSequentially(insertIX, newItems);
			Assert.That(expected, Is.EqualTo(list).Using(itemComparer));
			Assert.That(expected.Count, Is.EqualTo(list.Count));

		}
	}

	/// <summary>
	/// Same as <see cref="ListIntegrationTest{T}"/> but using span mutator methods
	/// </summary>
	/// <param name="buffer">The buffer being tested</param>
	/// <param name="maxCapacity">Maximum bytes buffer can store</param>
	/// <param name="mutateFromEndOnly">Mutate only from end of buffer, not middle</param>
	/// <param name="iterations">Number of internal iterations where random operations are performed </param>
	/// <param name="expected">Buffer implementation which <see cref="buffer"/> is tested against. This is assumed to be bug-free.</param>
	public static void BufferIntegrationTest(
		IBuffer buffer, 
		int maxCapacity,
		bool mutateFromEndOnly = false, 
		int iterations = 100, 
		IBuffer expected = null
	) {
		Guard.ArgumentNotNull(buffer, nameof(buffer));

		var RNG = new Random(31337);
		expected ??= new BufferAdapter(new List<byte>());
		var itemComparer = ByteArrayEqualityComparer.Instance;

		// Test 0: Adding via indexing operator fails
		byte val = 23;
		Assert.That(() => expected[0] = val, Throws.Exception);
		Assert.That(() => buffer[0] = val, Throws.Exception);

		// Test 1: Add nothing
		expected.AddRange(Tools.Spans.Empty<byte>());
		buffer.AddRange(Tools.Spans.Empty<byte>());
		Assert.That(expected, Is.EqualTo(buffer).Using(itemComparer));

		// Test 2: Insert nothing
		expected.InsertRange(0, Tools.Spans.Empty<byte>());
		buffer.InsertRange(0, Tools.Spans.Empty<byte>());
		Assert.That(expected, Is.EqualTo(buffer).Using(itemComparer));

		if (maxCapacity >= 1) {
			// Test 3: Insert at 0 when empty 
			var item = RNG.NextBytes(1).AsSpan();
			expected.InsertRange(0, item);
			buffer.InsertRange(0, item);
			Assert.That(expected, Is.EqualTo(buffer).Using(itemComparer));
		}

		if (maxCapacity >= 2) {
			// Test 4: Insert at end of list (same as add)
			var item = RNG.NextBytes(1).AsSpan();
			expected.InsertRange(1, item);
			buffer.InsertRange(1, item);
			Assert.That(expected, Is.EqualTo(buffer).Using(itemComparer));

			// Test 5: Delete from beginning of list
			if (!mutateFromEndOnly) {
				expected.RemoveAt(0);
				buffer.RemoveAt(0);
				Assert.That(expected, Is.EqualTo(buffer).Using(itemComparer));
			}
		}

		if (maxCapacity >= 1) {
			// Test 6: Delete from end of list
			expected.RemoveAt(^1);
			buffer.RemoveAt(^1);
			Assert.That(expected, Is.EqualTo(buffer).Using(itemComparer));
		}

		// Test 7: AddRange half capacity
		Span<byte> newItems = RNG.NextBytes(maxCapacity / 2);
		expected.AddRange(newItems);
		buffer.AddRange(newItems);
		Assert.That(expected, Is.EqualTo(buffer).Using(itemComparer));

		// Test 8: Enumerator consistency
		using (var expectedEnumerator = expected.GetEnumerator())
		using (var enumerator = buffer.GetEnumerator()) {
			// .NET 8: .net 8 has a bug/change where enumerators throw on Current when empty
			//Assert.That(expectedEnumerator.Current, Is.EqualTo(enumerator.Current).Using(itemComparer));
			bool expectedMoveNext;
			do {
				expectedMoveNext = expectedEnumerator.MoveNext();
				var moveNext = enumerator.MoveNext();
				Assert.That(expectedMoveNext, Is.EqualTo(moveNext).Using(itemComparer));
				if (expectedMoveNext)
					Assert.That(expectedEnumerator.Current, Is.EqualTo(enumerator.Current));
			} while (expectedMoveNext);
		}

		// Test 9: Clear
		for (var i = 0; i < 3; i++) {
			Assert.That(expected, Is.EqualTo(buffer).Using(itemComparer));
			// add a random amount
			var remainingCapacity = checked((int)(maxCapacity - expected.Count));
			var newItemsCount = RNG.Next(0, remainingCapacity + 1);
			newItems = RNG.NextBytes(newItemsCount);
			expected.AddRange(newItems);
			buffer.AddRange(newItems);

			expected.Clear();
			buffer.Clear();
		}

		// Test 10: Iterate with random mutations
		for (var i = 0; i < iterations; i++) {

			// add a random amount
			var remainingCapacity = checked((int)(maxCapacity - expected.Count));
			var newItemsCount = RNG.Next(0, remainingCapacity + 1);
			newItems = RNG.NextBytes(newItemsCount);
			buffer.AddRange(newItems);
			expected.AddRange(newItems);
			Assert.That(expected, Is.EqualTo(buffer).Using(itemComparer));

			if (buffer.Count > 0) {
				// update a random range
				var range = RNG.NextRange(checked((int)expected.Count));
				var rangeLen = Math.Max(0, range.End - range.Start + 1);
				newItems = RNG.NextBytes(rangeLen);
				buffer.UpdateRange(range.Start, newItems);
				expected.UpdateRange(range.Start, newItems);
				Assert.That(expected, Is.EqualTo(buffer).Using(itemComparer));

				// copy/paste a random range
				range = RNG.NextRange(checked((int)expected.Count));
				rangeLen = Math.Max(0, range.End - range.Start + 1);
				newItems = buffer.ReadRange(range.Start, rangeLen).ToArray();
				var expectedNewItems = expected.ReadRangeSequentially(range.Start, rangeLen).ToArray();

				range = RNG.NextRange(checked((int)expected.Count), rangeLength: newItems.Length);
				expected.UpdateRangeSequentially(range.Start, expectedNewItems);
				buffer.UpdateRange(range.Start, newItems);
				Assert.That(expected, Is.EqualTo(buffer).Using(itemComparer));

				// remove a random amount
				range = RNG.NextRange(checked((int)expected.Count), fromEndOnly: mutateFromEndOnly);
				rangeLen = Math.Max(0, range.End - range.Start + 1);
				buffer.RemoveRange(range.Start, rangeLen);
				expected.RemoveRangeSequentially(range.Start, rangeLen);
				Assert.That(expected, Is.EqualTo(buffer).Using(itemComparer));
			}

			// insert a random amount
			remainingCapacity = checked((int)(maxCapacity - expected.Count));
			newItemsCount = RNG.Next(0, remainingCapacity + 1);
			newItems = RNG.NextBytes(newItemsCount);
			var insertIX = mutateFromEndOnly ? buffer.Count : RNG.Next(0, checked((int)expected.Count) + 1); // + 1 allows inserting from end (same as add)
			buffer.InsertRange(insertIX, newItems);
			expected.InsertRange(insertIX, newItems);
			Assert.That(expected, Is.EqualTo(buffer).Using(itemComparer));
		}

	}

	public static void RecyclableListIntegrationTest<T>(
		IRecyclableList<T> list,
		int maxCapacity,
		Func<Random, long, T[]> randomItemGenerator,
		bool mutateFromEndOnly = false,
		int iterations = 100,
		IRecyclableList<T> expected = null,
		Action endOfIterTest = null,
		IEqualityComparer<T> itemComparer = null
	) {
		throw new NotImplementedException(); // below needs to be refactored for RecyclableList
		var RNG = new Random(31337);
		itemComparer ??= EqualityComparer<T>.Default;
		expected ??= new RecyclableListAdapter<T>(itemComparer);

		// Test 1: Add nothing
		expected.AddRangeSequentially(Enumerable.Empty<T>());
		list.AddRange(Enumerable.Empty<T>());
		Assert.That(expected, Is.EqualTo(list).Using(itemComparer));
		Assert.That(expected.Count, Is.EqualTo(list.Count));

		// Test 2: Insert nothing
		// REMOVED

		if (maxCapacity >= 1) {
			// Test 3: Add at 0 when empty 
			var item = randomItemGenerator(RNG, 1).Single();
			expected.Add(item);
			list.Add(item);
			Assert.That(expected, Is.EqualTo(list).Using(itemComparer));
			Assert.That(expected.Count, Is.EqualTo(list.Count));
		}

		if (maxCapacity >= 2) {
			// Test 4: Add at end of list (same as add)
			var item = randomItemGenerator(RNG, 1).Single();
			expected.Add( item);
			list.Add( item);
			Assert.That(expected, Is.EqualTo(list).Using(itemComparer));
			Assert.That(expected.Count, Is.EqualTo(list.Count));

			// Test 5: Delete from beginning of list
			if (!mutateFromEndOnly) {
				expected.RemoveAt(0);
				list.RemoveAt(0);
				Assert.That(expected, Is.EqualTo(list).Using(itemComparer));
				Assert.That(expected.Count, Is.EqualTo(list.Count));
			}
		}

		if (maxCapacity >= 1) {
			// Test 6: Delete from end of list
			expected.RemoveAt(^1);
			list.RemoveAt(^1);
			Assert.That(expected, Is.EqualTo(list).Using(itemComparer));
			Assert.That(expected.Count, Is.EqualTo(list.Count));
		}

		// Test 7: AddRange half capacity
		T[] newItems = randomItemGenerator(RNG, maxCapacity / 2);
		expected.AddRangeSequentially(newItems);
		list.AddRange(newItems);
		Assert.That(expected, Is.EqualTo(list).Using(itemComparer));
		Assert.That(expected.Count, Is.EqualTo(list.Count));

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
		Assert.That(expected.Count, Is.EqualTo(list.Count));

		// Test 9: Read/Remove/Update nothing from tip
		// First fill up
		newItems = randomItemGenerator(RNG, maxCapacity - expected.Count);
		expected.AddRangeSequentially(newItems);
		list.AddRange(newItems);
		// Read nothing from tip
		var expectedRead = expected.ReadRangeSequentially(checked((int)expected.Count), 0);
		var actualRead = list.ReadRange(list.Count, 0);
		Assert.That(expectedRead, Is.EqualTo(actualRead).Using(itemComparer));
		Assert.That(expected.Count, Is.EqualTo(list.Count));

		// Remove nothing from tip
		expected.RemoveRangeSequentially(checked((int)expected.Count), 0);
		list.RemoveRange(list.Count, 0);
		Assert.That(expected, Is.EqualTo(list).Using(itemComparer));
		Assert.That(expected.Count, Is.EqualTo(list.Count));

		// Update nothing from tip
		expected.UpdateRangeSequentially(checked((int)expected.Count), Enumerable.Empty<T>());
		list.UpdateRange(list.Count, Enumerable.Empty<T>());
		Assert.That(expected, Is.EqualTo(list).Using(itemComparer));
		Assert.That(expected.Count, Is.EqualTo(list.Count));

		// Test 10: Read/Remove/Update range overflow throws
		Assert.That(() => expected.ReadRangeSequentially(checked((int)expected.Count) / 2, maxCapacity + 1).ToArray(), Throws.InstanceOf<ArgumentException>());
		Assert.That(() => list.ReadRange(list.Count / 2, maxCapacity + 1).ToArray(), Throws.InstanceOf<ArgumentException>());
		Assert.That(() => expected.RemoveRangeSequentially(checked((int)expected.Count) / 2, maxCapacity + 1), Throws.InstanceOf<ArgumentException>());
		Assert.That(() => list.RemoveRange(list.Count / 2, maxCapacity + 1), Throws.InstanceOf<ArgumentException>());
		var updateItems = Tools.Array.Gen(maxCapacity + 1, randomItemGenerator(RNG, 1)[0]);
		Assert.That(() => expected.UpdateRangeSequentially(checked((int)expected.Count) / 2, updateItems), Throws.InstanceOf<ArgumentException>());
		Assert.That(() => list.UpdateRange(list.Count / 2, updateItems), Throws.InstanceOf<ArgumentException>());

		// Clear items (note: inconsistency could arise from test 10 due to do Singular-lists detecting argument error after mutations whereas range-lists not)
		expected.Clear();
		list.Clear();

		// Test 11: Clear
		for (var i = 0; i < 3; i++) {
			Assert.That(expected, Is.EqualTo(list).Using(itemComparer));
			// add a random amount
			var remainingCapacity = maxCapacity - expected.Count;
			var newItemsCount = RNG.Next(0, checked((int)remainingCapacity) + 1);
			newItems = randomItemGenerator(RNG, newItemsCount);
			expected.AddRangeSequentially(newItems);
			list.AddRange(newItems);

			expected.Clear();
			list.Clear();
		}

		// Test 12: Iterate with random mutations
		for (var i = 0; i < iterations; i++) {

			// add a random amount
			var remainingCapacity = Math.Min(1, maxCapacity - expected.Count); // only one item
			var newItemsCount = RNG.Next(0, checked((int)remainingCapacity) + 1);
			newItems = randomItemGenerator(RNG, newItemsCount);
			list.AddRange(newItems);
			expected.AddRangeSequentially(newItems);
			Assert.That(expected, Is.EqualTo(list).Using(itemComparer));
			Assert.That(expected.Count, Is.EqualTo(list.Count));

			if (expected.Count > 0) {
				// update a random range
				var range = RNG.NextRange(checked((int)expected.Count));
				var rangeLen = Math.Max(0, range.End - range.Start + 1);
				newItems = randomItemGenerator(RNG, rangeLen);
				list.UpdateRange(range.Start, newItems);
				expected.UpdateRangeSequentially(range.Start, newItems);
				Assert.That(expected, Is.EqualTo(list).Using(itemComparer));
				Assert.That(expected.Count, Is.EqualTo(list.Count));

				// copy/paste a random range
				range = RNG.NextRange(checked((int)expected.Count));
				rangeLen = Math.Max(0, range.End - range.Start + 1);
				newItems = list.ReadRange(range.Start, rangeLen).ToArray();
				var expectedNewItems = expected.ReadRangeSequentially(range.Start, rangeLen).ToArray();

				range = RNG.NextRange(checked((int)expected.Count), rangeLength: newItems.Length);
				expected.UpdateRangeSequentially(range.Start, expectedNewItems);
				list.UpdateRange(range.Start, newItems);
				Assert.That(expected, Is.EqualTo(list).Using(itemComparer));
				Assert.That(expected.Count, Is.EqualTo(list.Count));

				// remove a random amount
				range = RNG.NextRange(checked((int)expected.Count), fromEndOnly: mutateFromEndOnly);
				rangeLen = Math.Max(0, range.End - range.Start + 1);
				list.RemoveRange(range.Start, rangeLen);
				expected.RemoveRangeSequentially(range.Start, rangeLen);
				Assert.That(expected, Is.EqualTo(list).Using(itemComparer));
				Assert.That(expected.Count, Is.EqualTo(list.Count));

				// PagedList specific: check page index consistency
				if (list is IPagedList<T> pagedList) {
					for (var j = 1; j < pagedList.Pages.Count; j++)
						Assert.That(pagedList.Pages[j].StartIndex, Is.EqualTo(pagedList.Pages[j - 1].EndIndex + 1));
				}

				// Custom user test
				endOfIterTest?.Invoke();
			}

			// insert a random amount
			// REMOVED

		}
	}


	public static void DictionaryIntegrationTest<TKey, TValue>(
		IDictionary<TKey, TValue> dictionary, 
		int maxCapacity, 
		Func<Random, (TKey, TValue)> randomItemGenerator, 
		int iterations = 100, 
		IDictionary<TKey, TValue> expected = null,
		IEqualityComparer<TKey> keyComparer = null, 
		IEqualityComparer<TValue> valueComparer = null, 
		Action endOfIterTest = null
	) {
		var rng = new Random(31337);
		expected ??= new Dictionary<TKey, TValue>();
		keyComparer ??= EqualityComparer<TKey>.Default;
		valueComparer ??= EqualityComparer<TValue>.Default;
		var kvpComparer = new KeyValuePairEqualityComparer<TKey, TValue>(keyComparer, valueComparer);


		// Add half capacity
		var newItems = Enumerable.Repeat(randomItemGenerator(rng), maxCapacity / 2).Select(x => x.ToKeyValuePair()).ToArray();
		expected.AddRange(newItems);
		dictionary.AddRange(newItems);

		// Enumerable comparer
		Assert.That(expected, Is.EqualTo(dictionary).Using(kvpComparer));

		// Enumerator consistency
		using (var expectedEnumerator = expected.GetEnumerator())
		using (var enumerator = dictionary.GetEnumerator()) {

			Assert.That(expectedEnumerator.Current, Is.EqualTo(enumerator.Current).Using(kvpComparer));
			bool expectedMoveNext;
			do {
				expectedMoveNext = expectedEnumerator.MoveNext();
				var moveNext = enumerator.MoveNext();
				Assert.That(expectedMoveNext, Is.EqualTo(moveNext).Using(kvpComparer));
				if (expectedMoveNext)
					Assert.That(expectedEnumerator.Current, Is.EqualTo(enumerator.Current).Using(kvpComparer));
			} while (expectedMoveNext);
		}

		// Ensure adding duplicate kvp fails
		if (newItems.Length > 0) {
			var existing = newItems[0];
			var newItem = randomItemGenerator(rng).ToKeyValuePair();
			var duplicate = new KeyValuePair<TKey, TValue>(existing.Key, newItem.Value);
			Assert.That(() => expected.Add(duplicate), Throws.Exception);
			Assert.That(() => dictionary.Add(duplicate), Throws.Exception);
			Assert.That(() => expected.Add(duplicate.Key, duplicate.Value), Throws.Exception);
			Assert.That(() => dictionary.Add(duplicate.Key, duplicate.Value), Throws.Exception);
		}

		// Clear
		expected.Clear();
		dictionary.Clear();
		Assert.That(dictionary.Count, Is.EqualTo(expected.Count));
		Assert.That(dictionary, Is.EqualTo(expected).Using(kvpComparer));

		for (var i = 0; i < iterations; i++) {
			var remainingCapacity = maxCapacity - expected.Count;

			// add a bunch of random items
			var toAdd = rng.Next(0, remainingCapacity + 1);
			for (var j = 0; j < toAdd; j++) {
				var item = randomItemGenerator(rng);
				dictionary.Add(item.Item1, item.Item2);
				expected.Add(item.Item1, item.Item2);
				Assert.That(dictionary.ContainsKey(item.Item1), Is.EqualTo(expected.ContainsKey(item.Item1)));
			}

			Assert.That(dictionary.Count, Is.EqualTo(expected.Count));

			// update a bunch of random items
			var toUpdate = rng.Next(0, expected.Count);
			for (var j = 0; j < toUpdate; j++) {
				var randomIndex = rng.Next(0, expected.Count);
				var oldItem = expected.ToArray()[randomIndex];
				var newItem = randomItemGenerator(rng);
				var updatedItem = KeyValuePair.Create(oldItem.Key, newItem.Item2);
				dictionary[updatedItem.Key] = updatedItem.Value;
				expected[updatedItem.Key] = updatedItem.Value;
				Assert.That(dictionary.Contains(updatedItem), Is.EqualTo(expected.Contains(updatedItem)));
			}

			// remove a bunch of items
			var toRemove = rng.Next(0, expected.Count);

			// remove by key
			var toRemoveByKey = (int)Math.Ceiling(toRemove / 2.0);
			var keys = expected.Keys.Randomize().Take(toRemoveByKey).ToArray();
			foreach (var key in keys) {
				var removeResult = dictionary.Remove(key);
				var expectedRemoveResult = expected.Remove(key);
				Assert.That(dictionary.Count, Is.EqualTo(expected.Count));
				Assert.That(removeResult, Is.EqualTo(expectedRemoveResult));
				Assert.That(dictionary.ContainsKey(key), Is.EqualTo(expected.ContainsKey(key)));
				
			}

			//remove by kvp
			var toRemoveByKVP = (int)Math.Floor(toRemove / 2.0);
			Debug.Assert(toRemoveByKey + toRemoveByKVP == toRemove);

			var kvps = expected.Randomize().Take(toRemoveByKVP).ToArray();
			foreach (var kvp in kvps) {
				var removeResult = dictionary.Remove(kvp);
				var expectedRemoveResult = expected.Remove(kvp);
				Assert.That(removeResult, Is.EqualTo(expectedRemoveResult));
				Assert.That(dictionary.ContainsKey(kvp.Key), Is.EqualTo(expected.ContainsKey(kvp.Key)));
			}

			// Clear list every quarter iteration
			if (i == iterations / 2) {
				dictionary.Clear();
				expected.Clear();
			}

			CheckConsistent();
			endOfIterTest?.Invoke();
		}


		void CheckConsistent() {
			Assert.That(dictionary.Count, Is.EqualTo(expected.Count));
			Assert.That(expected.ToHashSet(kvpComparer).SetEquals(dictionary.ToHashSet(kvpComparer)));
			Assert.That(expected.Keys.ToHashSet(keyComparer).SetEquals(dictionary.Keys.ToHashSet(keyComparer)));
			Assert.That(expected.Values.ToHashSet(valueComparer).SetEquals(dictionary.Values.ToHashSet(valueComparer)));
		}
	}

	[Test]
	public static void StreamIntegrationTests(int maxSize, Stream actualStream, Stream expectedStream = null, int iterations = 100, Random RNG = null, bool runAsserts = true, Action extraTest = null) {
		Guard.ArgumentInRange(maxSize, 0, int.MaxValue, nameof(maxSize));
		Guard.ArgumentNotNull(actualStream, nameof(actualStream));
		Guard.ArgumentNot(actualStream.Length > 0 && expectedStream == null, nameof(actualStream), "Must be empty if not supplying expected stream");
		expectedStream ??= new MemoryStream();
		RNG ??= new Random(31337);

		for (var i = 0; i < iterations; i++) {
			if (runAsserts)
				AreEqual(expectedStream, actualStream);
			extraTest?.Invoke();

			// 1. random seek
			var seekParam = GenerateRandomSeekParameters(RNG, actualStream.Position, actualStream.Length);
			actualStream.Seek(seekParam.Item1, seekParam.Item2);
			expectedStream.Seek(seekParam.Item1, seekParam.Item2);
			if (runAsserts)
				AreEqual(expectedStream, actualStream);
			extraTest?.Invoke();

			// 2. write random bytes
			var remainingCapacity = (int)(maxSize - actualStream.Position);
			if (remainingCapacity > 0) {
				var fromBufferSize = RNG.Next(1, remainingCapacity * 2); // allow from buffer to be 0..2*remaining
				var fromBuffer = RNG.NextBytes(fromBufferSize);
				// Copy from a random segment of fromBuffer into stream
				var segment = RNG.NextRange(fromBufferSize, rangeLength: RNG.Next(1, Math.Min(fromBufferSize, remainingCapacity)));
				if (segment.End >= segment.Start) {
					expectedStream.Write(fromBuffer, segment.Start, segment.End - segment.Start + 1);
					actualStream.Write(fromBuffer, segment.Start, segment.End - segment.Start + 1);
				}
				if (runAsserts)
					AreEqual(expectedStream, actualStream);
				extraTest?.Invoke();
			}

			// 3. random read
			if (actualStream.Length > 0) {
				var segment = RNG.NextRange((int)actualStream.Length, rangeLength: Math.Max(1, RNG.Next(0, (int)actualStream.Length)));
				var count = segment.End - segment.Start + 1;
				expectedStream.Seek(segment.Start, SeekOrigin.Begin);
				actualStream.Seek(segment.Start, SeekOrigin.Begin);
				if (runAsserts) {
					ClassicAssert.AreEqual(expectedStream.ReadBytes(count), actualStream.ReadBytes(count));
					AreEqual(expectedStream, actualStream);
				}
				extraTest?.Invoke();
			}

			// 4. resize 
			var newLength = RNG.Next(0, maxSize);
			expectedStream.SetLength(newLength);
			actualStream.SetLength(newLength);

			if (runAsserts)
				AreEqual(expectedStream, actualStream);
			extraTest?.Invoke();
		}
	}

	public static Tuple<long, SeekOrigin> GenerateRandomSeekParameters(Random rng, long position, long length) {
		switch (rng.Next(0, 3)) {
			case 0:
				return Tuple.Create((long)rng.Next((int)-position, (int)(Math.Max(0, length - position))), SeekOrigin.Current);
			case 1:
				return Tuple.Create((long)rng.Next(0, (int)Math.Max(0, length)), SeekOrigin.Begin);
			case 2:
				return Tuple.Create((long)rng.Next((int)-length, 0 + 1), SeekOrigin.End);
			default:
				throw new InvalidOperationException();
		}
	}

	public static void AreEqual(Stream expected, Stream actual) {
		Assert.That(expected.Length, Is.EqualTo(actual.Length));
		Assert.That(expected.Position, Is.EqualTo(actual.Position));
		if (expected.Length == 0)
			return;
		var oldPos = expected.Position;
		expected.Seek(0L, SeekOrigin.Begin);
		actual.Seek(0L, SeekOrigin.Begin);
		var actualBytes = actual.ReadAll();
		var expectedBytes = expected.ReadAll();
		Assert.That(actualBytes, Is.EqualTo(expectedBytes).Using(new ByteArrayEqualityComparer()));
		expected.Seek(oldPos, SeekOrigin.Begin);
		actual.Seek(oldPos, SeekOrigin.Begin);
	}


	public static void AreEqual(IDynamicMerkleTree expected, IDynamicMerkleTree actual) {
		ClassicAssert.AreEqual(expected.Size, actual.Size);
		ClassicAssert.AreEqual(expected.Leafs.Count, actual.Leafs.Count);
		ClassicAssert.AreEqual(expected.Leafs.ToArray(), actual.Leafs.ToArray());
		ClassicAssert.AreEqual(expected.Root, actual.Root);
	}

	public static void NotHasFlags<T>(T expected, T actual) where T : Enum {
		ClassicAssert.IsFalse(actual.HasFlag(expected));
	}

	public static void HasFlags<T>(T expected, T actual) where T : Enum {
		ClassicAssert.IsTrue(actual.HasFlag(expected));
	}

	public static void HasAllFlags<T>(T actual, params T[] flags) where T : Enum {
		foreach (var flag in flags)
			HasFlags(flag, actual);
	}

	public static void RegexMatch(string expected, string regexPattern, params Tuple<string, string>[] expectedCaptures) {
		var regex = new Regex(regexPattern);
		var match = regex.Match(expected);
		ClassicAssert.AreEqual(expected, match.Value);
		foreach (var expectedCapture in expectedCaptures) {
			if (expectedCapture.Item2 == null)
				ClassicAssert.IsFalse(match.Groups[expectedCapture.Item1].Success);
			else
				ClassicAssert.AreEqual(expectedCapture.Item2, match.Groups[expectedCapture.Item1]?.Value);
		}
	}

	public static void RegexNotMatch(string badInput, string regexPattern, params Tuple<string, string>[] expectedCaptures) {
		var regex = new Regex(regexPattern);
		var match = regex.Match(badInput);
		ClassicAssert.AreNotEqual(badInput, match.Value);
		if (expectedCaptures.Any())
			ClassicAssert.IsFalse(expectedCaptures.All(c => c.Item2 == match.Groups[c.Item1]?.Value));

	}

	public static void AssertSame2DArrays<T>(IEnumerable<IEnumerable<T>> expectedRows, IEnumerable<IEnumerable<T>> actualRows, string actualName = "Actual", string expectedName = "Expected") {
		var expectedRowsArr = expectedRows as T[][] ?? expectedRows.Select(i => i as T[] ?? i.ToArray()).ToArray();
		var actualRowsArr = actualRows as T[][] ?? actualRows.Select(i => i as T[] ?? i.ToArray()).ToArray();

		var preText = String.Format("{0}{1}{2}{0}", Environment.NewLine, Tools.NUnit.Convert2DArrayToString(expectedName, expectedRowsArr), Tools.NUnit.Convert2DArrayToString(actualName, actualRowsArr));

		ClassicAssert.AreEqual(expectedRowsArr.Count(), actualRowsArr.Count(), "{4}{0} has {1} row(s) but {2} has {3} row(s)", actualName, expectedRowsArr.Count(), expectedName, actualRowsArr.Count(), preText);
		foreach (var rowExpectation in expectedRowsArr.WithDescriptions().Zip(actualRowsArr, Tuple.Create)) {
			ClassicAssert.AreEqual(rowExpectation.Item1.Item.Count(),
				rowExpectation.Item2.Count(),
				"{5}{0} row {1} had {2} column(s) but {3} row {1} had {4} column(s)",
				expectedName,
				rowExpectation.Item1.Index,
				rowExpectation.Item1.Item.Count(),
				actualName,
				rowExpectation.Item2.Count(),
				preText);
			foreach (var colExpectation in rowExpectation.Item1.Item.WithDescriptions().Zip(rowExpectation.Item2, Tuple.Create)) {
				ClassicAssert.AreEqual(colExpectation.Item1.Item,
					colExpectation.Item2,
					"{6}{0} row {1} col {2} had value {3} but {4} row {1} col {2} had value {5}",
					expectedName,
					rowExpectation.Item1.Index,
					colExpectation.Item1.Index,
					colExpectation.Item1.Item,
					actualName,
					colExpectation.Item2,
					preText);
			}
		}
	}

	public static void ApproxEqual(System.DateTime expected, System.DateTime actual, TimeSpan? tolerance = null, string errorMessage = null) {
		var approxEqual = expected.ApproxEqual(actual, tolerance);
		if (!approxEqual)
			ClassicAssert.Fail(errorMessage ?? ("Dates not approximately equal.{0}Expected: {1:yyyy-MM-dd HH:mm:ss.fff}{0}Actual: {2:yyyy-MM-dd HH:mm:ss.fff}").FormatWith(Environment.NewLine, expected, actual));
	}

	public static void ApproxEqual<T>(T expected, T actual, T tolerance, string message = null) {
		var lowerBound = Tools.OperatorTool.Subtract(expected, tolerance);
		var upperBound = Tools.OperatorTool.Add(expected, tolerance);
		var inRange = Tools.OperatorTool.LessThanOrEqual(lowerBound, actual) && Tools.OperatorTool.LessThanOrEqual(actual, upperBound);
		ClassicAssert.IsTrue(inRange, message ?? $"Value '{actual}' was not approx equal to '{expected}' (tolerance '{tolerance}')");
	}

	public static void HasLoadedPages<TItem>(PagedListBase<TItem> list, params long[] pageNos) {
		ClassicAssert.IsEmpty(list.Pages.Where(p => p.State == PageState.Loaded).Select(p => p.Number).Except(pageNos), "Unexpected pages were open");
	}

	public static void HasDirtyPages<TItem, TPage>(PagedListBase<TItem> list, params long[] pageNos) {
		ClassicAssert.IsEmpty(list.Pages.Where(p => p.Dirty).Select(p => p.Number).Except(pageNos), "Unexpected pages were dirty");
	}

}
