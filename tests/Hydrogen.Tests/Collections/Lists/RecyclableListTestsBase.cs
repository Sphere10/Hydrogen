// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Hydrogen.NUnit;
using NUnit.Framework;
using NUnit.Framework.Legacy;

namespace Hydrogen.Tests;

public abstract class RecyclableListTestsBase {

	protected abstract IDisposable CreateList<T>(IItemSerializer<T> serializer, IEqualityComparer<T> comparer, out IRecyclableList<T> list);

	[Test]
	public void WalkThrough() {
		using var disposables = CreateList(new StringSerializer(), StringComparer.InvariantCultureIgnoreCase, out var rlist);

		// verify empty
		Assert.That(rlist.Count, Is.EqualTo(0));
		Assert.That(rlist.ListCount, Is.EqualTo(0));
		Assert.That(rlist.RecycledCount, Is.EqualTo(0));

		// Enumerate empty
		ClassicAssert.AreEqual(rlist, Array.Empty<string>());

		// add "A"
		rlist.Add("A");
		Assert.That(rlist.Count, Is.EqualTo(1));
		Assert.That(rlist.ListCount, Is.EqualTo(1));
		Assert.That(rlist.RecycledCount, Is.EqualTo(0));

		// try insert
		Assert.That(() => rlist.Insert(0, "text"), Throws.InstanceOf<NotSupportedException>());

		// add "B" and "C"
		rlist.AddRange(new[] { "B", "C" });
		Assert.That(rlist.Count, Is.EqualTo(3));
		Assert.That(rlist.ListCount, Is.EqualTo(3));
		Assert.That(rlist.RecycledCount, Is.EqualTo(0));

		// remove "B"
		Assert.That(rlist.Remove("B"), Is.True);
		Assert.That(rlist.Count, Is.EqualTo(2));
		Assert.That(rlist.ListCount, Is.EqualTo(3));
		Assert.That(rlist.RecycledCount, Is.EqualTo(1));
		
		// Ensure B is not found
		var xxx = rlist.IndexOf("B");
		Assert.That(rlist.IndexOf("B"), Is.EqualTo(-1));

		// try read recycled
		Assert.That(() => rlist.Read(1), Throws.ArgumentException);
		Assert.That(() => rlist.Update(1, "X"), Throws.ArgumentException);
		Assert.That(() => rlist.RemoveAt(1), Throws.ArgumentException);

		// Enumerate 
		ClassicAssert.AreEqual(rlist, new[] { "A", "C" });
		Assert.That(rlist.Count, Is.EqualTo(2));
		Assert.That(rlist.ListCount, Is.EqualTo(3));
		Assert.That(rlist.RecycledCount, Is.EqualTo(1));

		// add "B1" (verify used recycled index)
		rlist.Add("B1");
		Assert.That(rlist.Count, Is.EqualTo(3));
		Assert.That(rlist.ListCount, Is.EqualTo(3));
		Assert.That(rlist.RecycledCount, Is.EqualTo(0));
		Assert.That(rlist.IndexOf("B1"), Is.EqualTo(1));
		Assert.That(rlist.Read(1), Is.EqualTo("B1"));

		// Update B1 to B2
		rlist.Update(1, "B2");
		Assert.That(rlist.IndexOf("B2"), Is.EqualTo(1));
		Assert.That(rlist.Count, Is.EqualTo(3));
		Assert.That(rlist.ListCount, Is.EqualTo(3));
		Assert.That(rlist.RecycledCount, Is.EqualTo(0));
		Assert.That(rlist.IndexOf("B2"), Is.EqualTo(1));
		Assert.That(rlist.Read(1), Is.EqualTo("B2"));

		// Enumeration check
		ClassicAssert.AreEqual(rlist, new[] { "A", "B2", "C" });

		// add another "A" (verify used new index)
		rlist.Add("A");
		Assert.That(rlist.ListCount, Is.EqualTo(4));
		Assert.That(rlist.RecycledCount, Is.EqualTo(0));
		Assert.That(rlist.Count, Is.EqualTo(4));

		// verify index of first "A"
		Assert.That(rlist.IndexOf("A"), Is.EqualTo(0));

		// Remove first A
		rlist.RemoveAt(0);
		Assert.That(rlist.ListCount, Is.EqualTo(4));
		Assert.That(rlist.RecycledCount, Is.EqualTo(1));
		Assert.That(rlist.Count, Is.EqualTo(3));

		// Verify index of second "A"
		Assert.That(rlist.IndexOf("A"), Is.EqualTo(3));

		// Remove second "A"
		rlist.RemoveAt(3);
		Assert.That(rlist.ListCount, Is.EqualTo(4));
		Assert.That(rlist.RecycledCount, Is.EqualTo(2));
		Assert.That(rlist.Count, Is.EqualTo(2));

		// Verify recycled indices
		Assert.That(rlist.IsRecycled(0), Is.True);
		Assert.That(rlist.IsRecycled(1), Is.False);
		Assert.That(rlist.IsRecycled(2), Is.False);
		Assert.That(rlist.IsRecycled(3), Is.True);

		// Verify not "A" is found
		Assert.That(rlist.IndexOf("A"), Is.EqualTo(-1));
		Assert.That(rlist.Contains("A"), Is.False);


		// Verify index of B2 and C are (1, 2) respectively and other indices are recycled
		Assert.That(rlist.IndexOf("B2"), Is.EqualTo(1));
		Assert.That(rlist.IndexOf("C"), Is.EqualTo(2));

		// Enumerate "B2" and "C"
		ClassicAssert.AreEqual(rlist, new[] { "B2", "C" });

		// Clear
		rlist.Clear();
		Assert.That(rlist.ListCount, Is.EqualTo(0));
		Assert.That(rlist.RecycledCount, Is.EqualTo(0));
		Assert.That(rlist.Count, Is.EqualTo(0));
	}


	//[Test]
	//public void IntegrationTest() {

	//	var serializer = new StringSerializer();
	//	var comparer = StringComparer.InvariantCultureIgnoreCase;
	//	using var disposables = CreateList(serializer, comparer, out var rlist);

	//	AssertEx.RecyclableListIntegrationTest(
	//		rlist,
	//		100,
	//		(rng, i) => Tools.Collection.RangeL(0, i)
	//			.Select(x => rng.NextString(1, 100))
	//			.ToArray(),
	//		expected: new RecyclableListAdapter<string>(new ExtendedList<string>(comparer), new StackList<long>())
	//	);
	//}
}
