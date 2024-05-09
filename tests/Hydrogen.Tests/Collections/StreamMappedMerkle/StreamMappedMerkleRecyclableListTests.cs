// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using FastSerialization;
using NUnit.Framework;
using NUnit.Framework.Legacy;


namespace Hydrogen.Tests;

[TestFixture]
[Parallelizable(ParallelScope.Children)]
public class StreamMappedMerkleRecyclableListTests : RecyclableListTestsBase {

	protected override IDisposable CreateList<T>(IItemSerializer<T> serializer, IEqualityComparer<T> comparer, out IRecyclableList<T> list) {
		var result = CreateList(CHF.SHA2_256, serializer, comparer, out var mlist);
		list = mlist;
		return result;
	}

	protected IDisposable CreateList<T>(CHF chf, IItemSerializer<T> serializer, IEqualityComparer<T> comparer, out StreamMappedMerkleRecyclableList<T> list) {
		var stream = new MemoryStream();
		var smrlist = new StreamMappedMerkleRecyclableList<T>(
			stream,
			chf,
			32, 
			serializer,
			autoLoad: true
		);
		list = smrlist;
		return new Disposables(smrlist, stream);
	}

	[Test]
	public void WalkThrough_CheckMerkleTree([Values(CHF.SHA2_256, CHF.Blake2b_128)] CHF chf) {
		var serializer = new StringSerializer();
		var recycledHash = Hashers.ZeroHash(chf);
		using var disposables = CreateList(chf, serializer, StringComparer.InvariantCultureIgnoreCase, out var list);

		byte[] TreeHash(params string[] text) => MerkleTree.ComputeMerkleRoot(text.Select(x => x is not null ? Hashers.Hash(chf, serializer.SerializeBytesLE(x)) : recycledHash), chf);


		// verify empty
		Assert.That(list.Count, Is.EqualTo(0));
		Assert.That(list.ListCount, Is.EqualTo(0));
		Assert.That(list.RecycledCount, Is.EqualTo(0));
		Assert.That(list.MerkleTree.Root, Is.Null);

		// Enumerate empty
		ClassicAssert.AreEqual(list, Array.Empty<string>());

		// add "A"
		list.Add("A");
		Assert.That(list.Count, Is.EqualTo(1));
		Assert.That(list.ListCount, Is.EqualTo(1));
		Assert.That(list.RecycledCount, Is.EqualTo(0));
		Assert.That(list.MerkleTree.Root, Is.EqualTo(TreeHash("A")));

		// try insert
		Assert.That(() => list.Insert(0, "text"), Throws.InstanceOf<NotSupportedException>());

		// add "B" and "C"
		list.AddRange(new[] { "B", "C" });
		Assert.That(list.Count, Is.EqualTo(3));
		Assert.That(list.ListCount, Is.EqualTo(3));
		Assert.That(list.RecycledCount, Is.EqualTo(0));
		Assert.That(list.MerkleTree.Root, Is.EqualTo(TreeHash("A", "B", "C")));

		// remove "B"
		Assert.That(list.Remove("B"), Is.True);
		Assert.That(list.Count, Is.EqualTo(2));
		Assert.That(list.ListCount, Is.EqualTo(3));
		Assert.That(list.RecycledCount, Is.EqualTo(1));
		Assert.That(list.MerkleTree.Root, Is.EqualTo(TreeHash("A", null, "C")));
		
		// Ensure B is not found
		Assert.That(list.IndexOf("B"), Is.EqualTo(-1));

		// try read recycled
		Assert.That(() => list.Read(1), Throws.ArgumentException);
		Assert.That(() => list.Update(1, "X"), Throws.ArgumentException);
		Assert.That(() => list.RemoveAt(1), Throws.ArgumentException);

		// Enumerate 
		ClassicAssert.AreEqual(list, new[] { "A", "C" });
		Assert.That(list.Count, Is.EqualTo(2));
		Assert.That(list.ListCount, Is.EqualTo(3));
		Assert.That(list.RecycledCount, Is.EqualTo(1));

		// add "B1" (verify used recycled index)
		list.Add("B1");
		Assert.That(list.Count, Is.EqualTo(3));
		Assert.That(list.ListCount, Is.EqualTo(3));
		Assert.That(list.RecycledCount, Is.EqualTo(0));
		Assert.That(list.IndexOf("B1"), Is.EqualTo(1));
		Assert.That(list.Read(1), Is.EqualTo("B1"));
		Assert.That(list.MerkleTree.Root, Is.EqualTo(TreeHash("A", "B1", "C")));

		// Update B1 to B2
		list.Update(1, "B2");
		Assert.That(list.IndexOf("B2"), Is.EqualTo(1));
		Assert.That(list.Count, Is.EqualTo(3));
		Assert.That(list.ListCount, Is.EqualTo(3));
		Assert.That(list.RecycledCount, Is.EqualTo(0));
		Assert.That(list.IndexOf("B2"), Is.EqualTo(1));
		Assert.That(list.Read(1), Is.EqualTo("B2"));
		Assert.That(list.MerkleTree.Root, Is.EqualTo(TreeHash("A", "B2", "C")));

		// Enumeration check
		ClassicAssert.AreEqual(list, new[] { "A", "B2", "C" });

		// add another "A" (verify used new index)
		list.Add("A");
		Assert.That(list.ListCount, Is.EqualTo(4));
		Assert.That(list.RecycledCount, Is.EqualTo(0));
		Assert.That(list.Count, Is.EqualTo(4));
		Assert.That(list.MerkleTree.Root, Is.EqualTo(TreeHash("A", "B2", "C", "A")));

		// verify index of first "A"
		Assert.That(list.IndexOf("A"), Is.EqualTo(0));

		// Remove first A
		list.RemoveAt(0);
		Assert.That(list.ListCount, Is.EqualTo(4));
		Assert.That(list.RecycledCount, Is.EqualTo(1));
		Assert.That(list.Count, Is.EqualTo(3));
		Assert.That(list.MerkleTree.Root, Is.EqualTo(TreeHash(null, "B2", "C", "A")));

		// Verify index of second "A"
		Assert.That(list.IndexOf("A"), Is.EqualTo(3));

		// Remove second "A"
		list.RemoveAt(3);
		Assert.That(list.ListCount, Is.EqualTo(4));
		Assert.That(list.RecycledCount, Is.EqualTo(2));
		Assert.That(list.Count, Is.EqualTo(2));
		Assert.That(list.MerkleTree.Root, Is.EqualTo(TreeHash(null, "B2", "C", null)));

		// Verify recycled indices
		Assert.That(list.IsRecycled(0), Is.True);
		Assert.That(list.IsRecycled(1), Is.False);
		Assert.That(list.IsRecycled(2), Is.False);
		Assert.That(list.IsRecycled(3), Is.True);

		// Verify not "A" is found
		Assert.That(list.IndexOf("A"), Is.EqualTo(-1));
		Assert.That(list.Contains("A"), Is.False);


		// Verify index of B2 and C are (1, 2) respectively and other indices are recycled
		Assert.That(list.IndexOf("B2"), Is.EqualTo(1));
		Assert.That(list.IndexOf("C"), Is.EqualTo(2));

		// Enumerate "B2" and "C"
		ClassicAssert.AreEqual(list, new[] { "B2", "C" });

		// Clear
		list.Clear();
		Assert.That(list.ListCount, Is.EqualTo(0));
		Assert.That(list.RecycledCount, Is.EqualTo(0));
		Assert.That(list.Count, Is.EqualTo(0));
		Assert.That(list.MerkleTree.Root, Is.Null);
	}


	[Test]
	public void Remove_Bug_1([Values(CHF.SHA2_256, CHF.Blake2b_128)] CHF chf) {
		var serializer = new StringSerializer();
		using var disposables = CreateList(chf, serializer, StringComparer.InvariantCultureIgnoreCase, out var list);
		list.Add("alpha");
		list.RemoveAt(0);
		Assert.That(list.MerkleTree.Root, Is.EqualTo(Hashers.ZeroHash(chf)));
	}
	
	[Test]
	public void Special_Remove([Values(CHF.SHA2_256, CHF.Blake2b_128)] CHF chf) {
		var serializer = new StringSerializer();
		using var disposables = CreateList(chf, serializer, StringComparer.InvariantCultureIgnoreCase, out var list);
		list.Add("alpha");
		list.Add("beta");
		list.Add("gamma");
		list.RemoveAt(0);
		list.Add("delta");
		list.RemoveAt(1);
		list.RemoveAt(2);
		Assert.That(list.MerkleTree.Root, Is.EqualTo(MerkleTree.ComputeMerkleRoot(new[] { "delta", null, null }, chf, serializer)));
	}


	[Test]
	public void Special_RemoveAll([Values(CHF.SHA2_256, CHF.Blake2b_128)] CHF chf) {
		var serializer = new StringSerializer();
		using var disposables = CreateList(chf, new StringSerializer(), StringComparer.InvariantCultureIgnoreCase, out var list);
		list.Add("alpha");
		list.Add("beta");
		list.Add("gamma");
		list.RemoveAt(0);
		list.Add("delta");
		list.RemoveAt(0);
		list.RemoveAt(1);
		list.RemoveAt(2);
		Assert.That(list.MerkleTree.Root, Is.EqualTo(MerkleTree.ComputeMerkleRoot(new string[] { null, null, null }, chf, serializer)));
	}
}