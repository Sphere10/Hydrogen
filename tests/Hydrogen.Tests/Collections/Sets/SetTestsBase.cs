// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace Hydrogen.Tests;

// TODO: add "empty set" variations for tests
public abstract class SetTestsBase {


	[Test]
	public void AddN([Values(0, 1, 2, 10, 111)] int N) {
		var values = Enumerable.Range(0, N).Select(i => $"test{i}").ToArray();
		using (CreateSet<string>(null, out var set)) {
			for (var i = 0; i < N; i++)
				set.Add(values[i]);

			Assert.That(set.Count, Is.EqualTo(N));
		}
	}


	[Test]
	public void DuplicatesN([Values(0, 1, 2, 10, 111)] int N, [Values(1, 3, 5)] int duplicateAmount) {
		var values = Enumerable.Range(0, N).Select(i => $"test{i}").ToArray();
		var copy = new HashSet<string>();
		using (CreateSet<string>(null, out var set)) {
			for (var j = 0; j < duplicateAmount; j++) {
				for (var i = 0; i < N; i++) {
					var value = values[i];
					set.Add(value);
				}
			}

			Assert.That(set.Count, Is.EqualTo(N));
			for (var i = 0; i < N; i++)
				Assert.That(set, Contains.Item(values[i]));
		}
	}


	[Test]
	public void ContainsN([Values(0, 1, 2, 10, 111)] int N) {
		var values = Enumerable.Range(0, N).Select(i => $"test{i}").ToArray();
		using (CreateSet<string>(null, out var set)) {
			for (var i = 0; i < N; i++)
				set.Add(values[i]);

			for (var i = 0; i < N; i++)
				Assert.That(set, Contains.Item(values[i]));
		}
	}


	[Test]
	public void EnumerateN([Values(0, 1, 2, 10, 111)] int N) {
		var values = Enumerable.Range(0, N).Select(i => $"test{i}").ToArray();
		var copy = new HashSet<string>();
		using (CreateSet<string>(null, out var set)) {
			for (var i = 0; i < N; i++) {
				set.Add(values[i]);
				copy.Add(values[i]);
			}

			foreach (var value in set)
				Assert.That(copy, Contains.Item(value));
		}
	}

	[Test]
	public void RemoveN([Values(0, 1, 2, 10, 111)] int N) {
		var values = Enumerable.Range(0, N).Select(i => $"test{i}").ToArray();
		using (CreateSet<string>(null, out var set)) {
			for (var i = 0; i < N; i++)
				set.Add(values[i]);

			for (var i = 0; i < N; i++) {
				Assert.That(() => set.Remove(values[i]), Is.True);
			}

			Assert.That(set, Has.Count.Zero);
		}
	}

	[Test]
	public void ExceptWith([Values(0, 1, 2, 10, 111)] int N) {
		var values = Enumerable.Range(0, N).ToArray();
		var odds = Enumerable.Range(0, N).Where(x => x % 2 != 0).ToArray();
		var evens = Enumerable.Range(0, N).Where(x => x % 2 == 0).ToArray();
		using (CreateSet<int>(null, out var set)) {
			set.AddRangeSequentially(values);
			set.ExceptWith(odds);
			Assert.That(set.OrderBy(x => x), Is.EqualTo(evens));
		}
	}


	[Test]
	public void SymmetricExceptWith_1([Values(2, 10, 11, 111)] int N) {
		var values = Enumerable.Range(1, N);
		var odds = values.Where(x => x % 2 != 0);
		var evens = values.Where(x => x % 2 == 0);
		using (CreateSet<int>(null, out var set)) {
			set.AddRangeSequentially(values);
			set.SymmetricExceptWith(odds);
			Assert.That(set, Is.EqualTo(evens));
		}
	}

	[Test]
	public void SymmetricExceptWith_2([Values(2, 10, 11, 111)] int N) {
		var values = Enumerable.Range(1, N);
		var odds = values.Where(x => x % 2 != 0);
		var evens = values.Where(x => x % 2 == 0);
		using (CreateSet<int>(null, out var set)) {
			set.AddRangeSequentially(odds);
			set.SymmetricExceptWith(evens);
			Assert.That(set.OrderBy(x => x), Is.EqualTo(values));
		}
	}

	[Test]
	public void UnionWith_Empty() {
		using (CreateSet<int>(null, out var set)) {
			set.UnionWith(Enumerable.Empty<int>());
			Assert.That(set, Is.Empty);
		}
	}

	[Test]
	public void UnionWith_1([Values(2, 10, 11, 111)] int N) {
		var values = Enumerable.Range(1, N);
		var odds = values.Where(x => x % 2 != 0);
		var evens = values.Where(x => x % 2 == 0);
		using (CreateSet<int>(null, out var set)) {
			set.AddRangeSequentially(odds);
			set.UnionWith(evens);
			Assert.That(set.OrderBy(x => x), Is.EqualTo(values));
		}
	}


	[Test]
	public void UnionWith_2([Values(2, 10, 11, 111)] int N) {
		var values = Enumerable.Range(1, N);
		var odds = values.Where(x => x % 2 != 0);
		var evens = values.Where(x => x % 2 == 0);
		using (CreateSet<int>(null, out var set)) {
			set.AddRangeSequentially(odds);
			set.UnionWith(odds);
			Assert.That(set.OrderBy(x => x), Is.EqualTo(odds));
		}
	}

	[Test]
	public void IntersectWith([Values(2, 10, 11, 111)] int N) {
		var values = Enumerable.Range(1, N);
		var odds = values.Where(x => x % 2 != 0);
		var evens = values.Where(x => x % 2 == 0);
		using (CreateSet<int>(null, out var set)) {
			set.AddRangeSequentially(values);
			set.IntersectWith(odds);
			Assert.That(set.OrderBy(x => x), Is.EqualTo(odds));
		}
	}

	[Test]
	public void IsSubsetOf_Empty() {
		using (CreateSet<int>(null, out var set)) {
			Assert.That(set.IsSubsetOf(Enumerable.Empty<int>()), Is.True);
		}
	}

	[Test]
	public void IsSubsetOf([Values(2, 10, 11, 111)] int N) {
		var values = Enumerable.Range(1, N);
		var odds = values.Where(x => x % 2 != 0);
		var evens = values.Where(x => x % 2 == 0);
		using (CreateSet<int>(null, out var set)) {
			set.AddRangeSequentially(odds);
			Assert.That(set.IsSubsetOf(values), Is.True);
			Assert.That(set.IsSubsetOf(odds), Is.True);
			Assert.That(set.IsSubsetOf(evens), Is.False);
		}
	}

	[Test]
	public void IsProperSubsetOf_Empty() {
		using (CreateSet<int>(null, out var set)) {
			Assert.That(set.IsProperSubsetOf(Enumerable.Empty<int>()), Is.False);
		}
	}

	[Test]
	public void IsProperSubsetOf([Values(2, 10, 11, 111)] int N) {
		var values = Enumerable.Range(1, N);
		var odds = values.Where(x => x % 2 != 0);
		var evens = values.Where(x => x % 2 == 0);
		using (CreateSet<int>(null, out var set)) {
			set.AddRangeSequentially(odds);
			Assert.That(set.IsProperSubsetOf(values), N > 0 ? Is.True : Is.False);
			Assert.That(set.IsProperSubsetOf(odds), Is.False);
			Assert.That(set.IsProperSubsetOf(evens), Is.False);
		}
	}

	[Test]
	public void IsSupersetOf([Values(2, 10, 11, 111)] int N) {
		var values = Enumerable.Range(1, N);
		var odds = values.Where(x => x % 2 != 0);
		var evens = values.Where(x => x % 2 == 0);
		using (CreateSet<int>(null, out var set)) {
			set.AddRangeSequentially(values);
			Assert.That(set.IsSupersetOf(values), Is.True);
			Assert.That(set.IsSupersetOf(odds), Is.True);
			Assert.That(set.IsSupersetOf(evens), Is.True);
		}
	}

	[Test]
	public void IsProperSupersetOf([Values(2, 10, 11, 111)] int N) {
		var values = Enumerable.Range(1, N);
		var odds = values.Where(x => x % 2 != 0);
		var evens = values.Where(x => x % 2 == 0);
		using (CreateSet<int>(null, out var set)) {
			set.AddRangeSequentially(values);
			Assert.That(set.IsProperSupersetOf(values), Is.False);
			Assert.That(set.IsProperSupersetOf(odds), Is.True);
			Assert.That(set.IsProperSupersetOf(evens), Is.True);
		}
	}

	[Test]
	public void Overlaps_1([Values(2, 10, 11, 111)] int N) {
		var values = Enumerable.Range(1, N);
		var odds = values.Where(x => x % 2 != 0);
		var evens = values.Where(x => x % 2 == 0);
		using (CreateSet<int>(null, out var set)) {
			set.AddRangeSequentially(values);
			Assert.That(set.Overlaps(values), Is.True);
			Assert.That(set.Overlaps(odds), Is.True);
			Assert.That(set.Overlaps(evens), Is.True);
		}
	}

	[Test]
	public void Overlaps_2([Values(2, 10, 11, 111)] int N) {
		var values = Enumerable.Range(1, N);
		var odds = values.Where(x => x % 2 != 0);
		var evens = values.Where(x => x % 2 == 0);
		using (CreateSet<int>(null, out var set)) {
			set.AddRangeSequentially(odds);
			Assert.That(set.Overlaps(values), Is.True);
			Assert.That(set.Overlaps(odds), Is.True);
			Assert.That(set.Overlaps(evens), Is.False);
		}
	}


	[Test]
	public void SetEquals_Empty() {
		using (CreateSet<int>(null, out var set)) {
			set.SetEquals(Enumerable.Empty<int>());
		}
	}

	[Test]
	public void SetEquals_1([Values(2, 10, 11, 111)] int N) {
		var rng = new Random(31337);
		var values = Enumerable.Range(1, N);
		var odds = values.Where(x => x % 2 != 0);
		var evens = values.Where(x => x % 2 == 0);
		using (CreateSet<int>(null, out var set)) {
			set.AddRangeSequentially(values);
			Assert.That(() => set.SetEquals(values), Is.True);
			Assert.That(() => set.SetEquals(values.Randomize(rng)), Is.True);
			Assert.That(() => set.SetEquals(odds), Is.False);
			Assert.That(() => set.SetEquals(evens), Is.False);
		}
	}

	[Test]
	public void SetEquals_2([Values(2, 10, 11, 111)] int N) {
		var rng = new Random(31337);
		var values = Enumerable.Range(1, N);
		var odds = values.Where(x => x % 2 != 0);
		var evens = values.Where(x => x % 2 == 0);
		using (CreateSet<int>(null, out var set)) {
			set.AddRangeSequentially(odds);
			Assert.That(() => set.SetEquals(values), Is.False);
			Assert.That(() => set.SetEquals(odds), Is.True);
			Assert.That(() => set.SetEquals(odds.Randomize(rng)), Is.True);
			Assert.That(() => set.SetEquals(evens), Is.False);
		}
	}

	[Test]
	public void CopyTo() {
		const int N = 10;
		var values = Enumerable.Range(1, N);
		using (CreateSet<int>(null, out var set)) {
			set.AddRangeSequentially(values);
			var array = new int[N + 2];
			array[0] = -1;
			array[^1] = -1;
			set.CopyTo(array, 1);
			Array.Sort(array, 1, 10);
			Assert.That(array, Is.EqualTo(new[] { -1 }.Concat(values).Concat(-1)));
		}
	}

	protected abstract IDisposable CreateSet<TValue>(IEqualityComparer<TValue> comparer, out ISet<TValue> set);

}
